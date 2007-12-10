using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.AnalysisServices.Xmla;
using System.Collections.Specialized;
using Microsoft.AnalysisServices.AdomdServer;

namespace ASStoredProcs.DMVParser
{

    internal class SelectParser
    {
#region Private instance variables
        private string mFrom = "";
        private string mWhere = "";
        private string mOrder = "";
        private string[] mCols;
        private bool mDistinct = false;
        private bool canMatchRestrictions = true;
        private List<WherePredicate> whereList = new List<WherePredicate>();
#endregion
        
        public void Parse(string statement)
        {
            // break the statement into tokens    
            List<Tokenizer.Token> tList = new List<ASStoredProcs.DMVParser.Tokenizer.Token>();
            Tokenizer.Tokenizer t = new ASStoredProcs.DMVParser.Tokenizer.Tokenizer(statement);
            tList = t.Tokens;
            int ipos = 1;

            List<string> cols = new List<string>();

            // do not proceed if the first token is not the work "SELECT"
            if (tList[0].Text.ToUpper() == "SELECT")
            {
                // check for a DISTINCT query
                if (tList.Count >= 2 && tList[1].Text.ToUpper() == "DISTINCT")
                {
                    mDistinct = true;
                    ipos++;
                }
                // collect columns
                while (tList[ipos].Text.ToUpper() != "FROM")
                {
                    if (tList[ipos].Type != ASStoredProcs.DMVParser.Tokenizer.TokenType.Comma
                        && tList[ipos].Text != "["
                        && tList[ipos].Text != "]")
                    {
                        cols.Add(tList[ipos].Text);
                    }
                    ipos++;
                }

                mCols = cols.ToArray();

                ipos++;
                // Get the token for the schema that we are querying
                mFrom = tList[ipos].Text;
                ipos += 1;

                // look for the WHERE clause (if it exists)
                if (ipos < tList.Count && tList[ipos].Text.ToUpper() == "WHERE")
                {
                    ipos++;
                }

                List<Tokenizer.Token> whereTokens = new List<ASStoredProcs.DMVParser.Tokenizer.Token>();
                // where clause
                while (ipos < tList.Count 
                    && !(tList[ipos].Text.ToUpper() == "ORDER" 
                        && tList[ipos<tList.Count-2?ipos+1:ipos].Text.ToUpper() == "BY" ))
                {
                    if (tList[ipos].Type == ASStoredProcs.DMVParser.Tokenizer.TokenType.String)
                    {
                        mWhere += "'" + tList[ipos].Text + "' ";
                    }
                    else if (tList[ipos].Text != "[" && tList[ipos].Text != "]")
                    {
                        mWhere += tList[ipos].Text + " ";
                    }
                    whereTokens.Add(tList[ipos]);
                    ipos++;
                }

                // build a list of where predicates to see if we can match them to the 
                // restrictions for the rowset.
                int iwherePos = 0;
                while (iwherePos <= (whereTokens.Count - 3))
                { 
                    WherePredicate w = new WherePredicate();
                    w.Operand1 = whereTokens[iwherePos];
                    w.Operator = whereTokens[iwherePos+1];
                    w.Operand2 = whereTokens[iwherePos+2];

                    // restriction matching can only be done with the "=" operator
                    if ( w.Operand1.Type == ASStoredProcs.DMVParser.Tokenizer.TokenType.Bracket
                        || w.Operand2.Type == ASStoredProcs.DMVParser.Tokenizer.TokenType.Bracket
                        || w.Operator.Text != "=")
                    {
                        canMatchRestrictions = false;
                        whereList.Clear();
                        break;
                    }
                    whereList.Add(w);
                    iwherePos += 3;

                    if (iwherePos < whereTokens.Count)
                    {
                        // multiple predicates must be joined with a logical "AND"
                        // currently nested operations with brackets are not supported
                        if (whereTokens[iwherePos].Text.ToUpper() == "AND")
                        {
                            iwherePos++;
                        }
                        else
                        {
                            canMatchRestrictions = false;
                            whereList.Clear();
                            break;
                        }
                    }

                }
                // if we have left over tokens, something has gone wrong
                if (iwherePos != whereTokens.Count)
                {
                    whereList.Clear();
                    canMatchRestrictions = false;
                }

                if (canMatchRestrictions)
                {
                    string xmlaRestr = DiscoverRestrictions(Context.CurrentServerID, this.FromClause);
                    List<string> restr = extractRestrictions(xmlaRestr);
                    mRestrictions = MatchRestrictions(restr);
                }

                ipos += 2;
                // Order By clause
                while (ipos < tList.Count)
                {
                    mOrder += tList[ipos].Text + " ";
                    ipos++;
                }
            }
        }

#region Private Helper Functions
        private string DiscoverRestrictions(string serverName, string rowset)
        {
            Microsoft.AnalysisServices.Xmla.XmlaClient xc = new XmlaClient();
            string res = "";
            xc.Connect(serverName);
            try
            {
                xc.Discover("DISCOVER_SCHEMA_ROWSETS"
                    , string.Format("<SchemaName>{0}</SchemaName>", rowset)
                    , string.Empty
                    , out res
                    , false, false, false);
            }
            finally
            {
                xc.Disconnect();
            }

            return res;

        }

        private List<string> extractRestrictions(string xmlaResult)
        {
            XPathDocument xpDoc = new XPathDocument(new System.IO.StringReader(xmlaResult));
            XPathNavigator nav;
            XPathNodeIterator nodeIter;

            nav = xpDoc.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(nav.NameTable);
            manager.AddNamespace("aa", "urn:schemas-microsoft-com:xml-analysis");
            manager.AddNamespace("rt", "urn:schemas-microsoft-com:xml-analysis:rowset");

            nodeIter = nav.Select("//aa:return/rt:root/rt:row/rt:Restrictions/rt:Name", manager);
            List<string> lst = new List<string>();
            while (nodeIter.MoveNext())
            {
                lst.Add( nodeIter.Current.Value);
            }
            return lst;
        }

        private string[] MatchRestrictions(List<string> xmlaRestrictions)
        {
            List<string> res = new List<string>();
            if (canMatchRestrictions)
            {
                foreach (WherePredicate wp in whereList)
                {
                    if (xmlaRestrictions.Contains(wp.Operand1.Text))
                        res.Add(string.Format("<{0}>{1}</{0}>", wp.Operand1.Text, wp.Operand2.Text));
                    else if (xmlaRestrictions.Contains(wp.Operand2.Text))
                        res.Add(string.Format("<{0}>{1}</{0}>", wp.Operand2.Text, wp.Operand1.Text));
                }
            }
            return res.ToArray();

        }
#endregion

#region Public Properties

        public bool Distinct
        {
            get { return mDistinct; }
        }

        public string[] Columns
        {
            get
            {
                if (mCols.Length == 1 && mCols[0] == "*")
                {
                    return new string[0];
                }
                return mCols;
            }
        }

        public string WhereClause
        {
            get
            {
                return mWhere;
            }
        }

        public string OrderByClause
        {
            get
            {
                return mOrder;
            }
        }

        public string FromClause
        {
            get
            {
                if (mFrom.StartsWith(@"$system.", StringComparison.InvariantCultureIgnoreCase))
                {
                    return mFrom.Substring(8);
                }
                else
                {
                    return mFrom;
                }
            }
        }

        private string[] mRestrictions = null;
        public string Restrictions
        {
            get
            {
                if (mRestrictions == null)
                {
                    return "";
                }
                else
                {
                    return string.Join("", mRestrictions);
                }
            }
        }
#endregion
    }

    internal class WherePredicate
    {
        public Tokenizer.Token Operand1;
        public Tokenizer.Token Operator;
        public Tokenizer.Token Operand2;
    }
}
