/*============================================================================
  File:    StringFilters.cs

  Summary: Implements string filtering functions for use in MDX Queries.

  Date:    July 12, 2006

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/Wiki/View.aspx?ProjectName=ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/


using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AnalysisServices.AdomdServer;
using Tuple = Microsoft.AnalysisServices.AdomdServer.Tuple; //resolves ambiguous reference in .NET 4 with System.Tuple


namespace ASStoredProcs
{
    /// <summary>
    /// 
    /// </summary>
    public class StringFilters
    {
        
        static Hashtable regExCache = new Hashtable( new RegExCacheIndexComparer());

        #region " Public 'Like' functions"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToMatch">This is a string expression which </param>
        /// <param name="pattern">This paramter uses a pattern in the same form as the T-SQL LIKE operator</param>
        /// <returns>Boolean</returns>
        [SafeToPrepare(true)]
        public Boolean IsLike(String valueToMatch, String pattern)
        {
            return IsLike(valueToMatch, pattern, false);
        }

        [SafeToPrepare(true)]
        public Boolean IsLike(String valueToMatch, String pattern, Boolean caseSensitive)
        {
            Context.TraceEvent(100, 0, "IsLike: Starting");
            // todo - cache regex objects here
            RegexOptions optRegex = RegexOptions.Compiled;
            if (!caseSensitive) {optRegex = optRegex | RegexOptions.IgnoreCase;}
            Regex r = getCachedRegEx(LikeToRegEx(pattern), optRegex);
            Context.TraceEvent(100, 0, "IsLike: Finished");
            return (Boolean) r.Match(valueToMatch).Success;
        }

        [SafeToPrepare(true)]
        public static Set Like(Set setToFilter, String pattern, Expression exp)
        {
            return Like(setToFilter, pattern, exp, false);
        }

        [SafeToPrepare(true)]
        public static Set Like(Set setToFilter, String pattern, Expression exp, Boolean caseSensitive)
        {
            return RegExFilter(setToFilter, LikeToRegEx(pattern), exp, caseSensitive);
        }
        #endregion

        #region " Public 'Regex' functions"
        [SafeToPrepare(true)]
        public static Set RegExFilter(Set setToFilter, String pattern, Expression exp)
        {
            return RegExFilter(setToFilter, pattern, exp, false);
        }

        [SafeToPrepare(true)]
        public static Set RegExFilter(Set setToFilter, String pattern, Expression exp, Boolean caseSensitive)
        {
            Context.TraceEvent(100, 0, "RegExFilter: Starting");

            // If there is no startsWith string, just return the whole set
            if (pattern.Length == 0)
            {
                Context.TraceEvent(100, 0, "RegExFilter: Finished (No pattern parameter)");
                return setToFilter;
            }

            if (setToFilter == null)
            {
                throw new ArgumentNullException("setToFilter");
            }
            else
            {
                using(SetBuilder sb = new SetBuilder())
                {
                    RegexOptions optRegex = RegexOptions.Compiled;
                    if (!caseSensitive)
                    {
                        optRegex = optRegex | RegexOptions.IgnoreCase;
                    }
                    Regex r = getCachedRegEx(pattern, optRegex);                
                    foreach (Tuple t in setToFilter.Tuples)
                    {
                        string val = (string)exp.Calculate(t);
                        if (r.Match(val).Success)
                        {
                            sb.Add(t);
                        }
                        
                    }
                    Context.TraceEvent(100, sb.Count, "RegExFilter: Finished (returning " + sb.Count.ToString() + " tuples");
                    return sb.ToSet();
                }
            }
        } // End Like
        #endregion

        #region "Private Helper Functions"
        /// <summary>
        /// This function converts a pattern from the T-SQL LIKE format
        /// into a RegEx pattern.
        /// 
        ///      % = .*
        ///      _ = .
        /// 
        /// In a regex Characters other than . $ ^ { [ ( | ) * + ? \ match themselves.
        /// There fore the above characters need to be escaped so that they are correctly
        /// matched if they are present in the "like" pattern.
        /// </summary>
        /// <param name="pattern">This is the pattern to match in T-SQL LIKE format</param>
        /// <returns>string</returns>
        public static string LikeToRegEx(string pattern)
        {
            Context.TraceEvent(100, 0, "Like: Converting Like to RegEx");
            Context.CheckCancelled(); // Check if the user has cancelled 

            StringBuilder sb = new StringBuilder(pattern);
            // the order of the following operations is important or one replacement
            // can end up corrupting a previous replacement.
            sb.Replace(@"\", @"\\"); // needs to be done first before any other backslashes are introduced
            sb.Replace("*", @"\*");  // needs to be done before the '.*' insertion
            sb.Replace(".", @"\.");  // needs to be done before the '.*' insertion
            sb.Replace("%", ".*");
            sb.Replace("$", @"\$");
            sb.Replace("(", @"\(");
            sb.Replace(")", @"\)");
            sb.Replace("|", @"\|");
            sb.Replace("+", @"\+");
            sb.Replace("?", @"\?");
            sb.Replace("^", @"\^");
            // fix up strings where the above replace was too agressive and the
            // ^ was being used as part of a 'not matching' expression.
            sb.Replace(@"[\^", @"[^");
            sb.Replace("[[]", @"\[");
            sb.Replace("_", ".");
            // the above replacement incorrectly converts both _ to . and [_] to [.]
            // the next line converts the incorrect [.] to _
            sb.Replace("[.]", @"_");

            if (!pattern.StartsWith("%")) { sb.Insert(0, @"\A"); }
            if (!pattern.EndsWith("%")) { sb.Append(@"\z"); }
            return sb.ToString();
        }

        private static Regex getCachedRegEx(string pattern, RegexOptions opt)
        {
            Context.CheckCancelled(); // Check if the user has cancelled 

            Boolean caseSensitive = (opt & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;
            RegExCacheIndex ri = new RegExCacheIndex(pattern, caseSensitive);

            if (regExCache.ContainsKey(ri))
            {
                Context.TraceEvent(100, 0, "RegExFilter: Returning Cached RegEx");
                Regex cachedRegEx;
                lock (regExCache.SyncRoot)
                {
                    cachedRegEx = (Regex)regExCache[ri];
                }
                return cachedRegEx;
            }
            else
            {
                Context.TraceEvent(100, 0, "RegExFilter: Adding RegEx to Cache");
                Regex newRegEx = new Regex(pattern, opt);
                lock(regExCache.SyncRoot)
                {
                    regExCache.Add(ri, newRegEx);
                }
                return newRegEx;
            }

        }
        #endregion "Helper Functions"

    } // End class

    #region "Helper Classes"
    internal class RegExCacheIndex
    {
        public RegExCacheIndex(string pattern, Boolean caseSensitive)
        {
            m_caseSensitive = caseSensitive;
            m_pattern = pattern;
        }

        private Boolean m_caseSensitive = false;
        private String m_pattern = "";

        public Boolean CaseSensitive
        {
            get { return m_caseSensitive; }
            //set { m_caseSensitive = value; }
        }
        public String Pattern
        {
            get { return m_pattern; }
            //set { m_pattern = value; }
        }
    }

    internal class RegExCacheIndexComparer: IEqualityComparer
    {

        #region IEqualityComparer Members

        bool IEqualityComparer.Equals(object x, object y)
        {
            RegExCacheIndex left = (RegExCacheIndex)x;
            RegExCacheIndex right = (RegExCacheIndex)y;
            if (left.CaseSensitive == right.CaseSensitive)
            {
                if (left.CaseSensitive == true)
                { return (string.Compare(left.Pattern, right.Pattern, false)==0); }
                else
                { return (string.Compare(left.Pattern, right.Pattern, true)==0); }
            }
            else
            {
                return false;
            }
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            RegExCacheIndex ri = (RegExCacheIndex)obj;
            string pat = (ri.CaseSensitive ? "T:" : "F:") + ri.Pattern;
            return pat.GetHashCode();
        }

        #endregion
    }
    #endregion
} // End Namespace


