/*============================================================================
  File:    XmlaDiscoverParser.cs

  Summary: The primary purpose of this class is to execute XMLA discover
           commands and to return the results as a DataTable. It also has some
           secondary methods that execute the Cancel and ClearCache commands.

  Date:    May 19, 2007

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
using System.Xml;
using System.Data;
using Microsoft.AnalysisServices.AdomdServer;

namespace ASStoredProcs
{

    // This is an internal class, only used by the DiscoverXmlMetadata() function
    // to "flatten" the hierarchical data returned from the DISCOVER_XML_METADATA
    // command.
    internal class XmlaDiscoverParser
    {
        XmlDocument doc;
        public XmlDocument Document
        {
            get { return doc; }
            set { doc = value; }
        }

        string mfilter;
        public string Filter
        {
            get { return mfilter; }
            set { mfilter = value; }
        }

        public DataTable Parse(XmlDocument doc, string filter, bool prepareOnly, string whereClause)
        {
            DiscoverQuery dq;
            DataTable dt = new DataTable();
            string[] elements;
            elements = filter.Split((@"\").ToCharArray());
            dq = new DiscoverQuery(elements);
            XmlNodeList e;

            // We basically grab all the elements that match the last
            // item type in the path. So if you ask for '\Database\Dimensions\Dimension'
            // we get all the 'Dimension' elements and then in the next step we 
            // spin through them to see if they are under the appropriate
            // parent elements.
            //e = doc.GetElementsByTagName(elements[elements.Length - 1]);
            e = doc.GetElementsByTagName(dq.Elements.Last.Value.Name);

            /*
             * Get elements matching last item in the filter 
             * work up the parentNode properties, making sure
             * that they match the path.
             */
            List<XmlNode> finalList = new List<XmlNode>();

            for (int i = 0; i < e.Count; i++)
            {
                Context.CheckCancelled();

                XmlNode node = e.Item(i);
                XmlNode parentNode = node;
                xmlaElement xe = dq.Elements.Last.Value;

                while (xe != null)
                {
                    parentNode = parentNode.ParentNode;
                    if (xe.Previous != null 
                        && string.Compare(parentNode.Name, xe.Previous.Name, true) != 0) // Case insensitive compare
                    {
                        break;
                    }
                    else if (xe.Previous == null)
                    {
                        finalList.Add(node);
                    }
                    xe = xe.Previous;
                }
            }

            // Now we generate the data table based on the data in one of the nodes
            bool tableBuilt = false;
            foreach (XmlNode n in finalList)
            {
                if (!tableBuilt)
                {
                    // Only build the table based on this node if it is not empty
                    // or if it is the last node in the collection
                    if ((n.HasChildNodes) || (finalList.IndexOf(n) == finalList.Count))
                    {
                        foreach (XmlNode fld in n.ChildNodes)
                        {
                            Context.CheckCancelled();

                            //TODO - coonverting the .Contains call to BinarySearch would 
                            //       allow for the injection of a custom implementation of IComparer
                            //       in order to do a case insensitive search.
                            if (fld.NodeType == XmlNodeType.Element
                                && (!fld.HasChildNodes
                                    || (fld.HasChildNodes
                                        && (fld.ChildNodes.Count == 1)
                                        && (fld.FirstChild.NodeType == XmlNodeType.Text)
                                        )
                                    )
                                && (!dt.Columns.Contains(fld.Name)))
                            {
                                // Only add nodes that are elements and have 0 or 1 children (do not add collections)
                                dt.Columns.Add(new DataColumn(fld.Name));
                            }
                        }

                        // Add attributes from last node
                        if (n.Attributes.Count > 0)
                        {
                            foreach (XmlAttribute xa in n.Attributes)
                            {
                                dt.Columns.Add(n.LocalName + "." + xa.LocalName);
                            }
                        }
                        addParentIdColumns(dq, dt, n);
                        tableBuilt = true;
                        // If this is a prepare call, we only need to return an empty table
                        if (prepareOnly)
                        {
                            return dt;
                        }
                    }
                }
            }

            // loop throught the collection of nodes again to populate the table
            foreach (XmlNode n in finalList)
            {
                DataRow dr = dt.NewRow();
                string[] data = new string[dr.ItemArray.Length];

                foreach (XmlNode fld in n.ChildNodes)
                {
                    Context.CheckCancelled();

                    if ((dr.Table.Columns.IndexOf(fld.Name) >= 0) 
                        && (fld.HasChildNodes && fld.ChildNodes.Count == 1))
                    {
                        data[dr.Table.Columns.IndexOf(fld.Name)] = fld.FirstChild.InnerText;
                    }
                    addParentIdValues(dq, dt, data, fld);
                }
                if (!n.HasChildNodes)
                {
                    // we have found a empty node, just add any ancestor fields
                    addParentIdValues(dq, dt, data, n);
                }

                // Extract Attributes
                if (n.Attributes.Count > 0)
                {
                    foreach (XmlAttribute xa in n.Attributes)
                    {
                        data[dr.Table.Columns.IndexOf(n.LocalName + "." + xa.LocalName)] = xa.Value;
                    }
                }
                dr.ItemArray = data;
                dt.Rows.Add(dr);
            }

            if (whereClause.Length > 0)
            {
                dt.DefaultView.RowFilter = whereClause;
                dt = dt.DefaultView.ToTable();
            }
            return dt;
        }

        private void addParentIdColumns(DiscoverQuery dq, DataTable dt, XmlNode n)
        {
            XmlNode myNode = n;
            xmlaElement e;
            while (myNode.ParentNode != null)
            {
                myNode = myNode.ParentNode;
                e = dq.Elements.FindByName(myNode.Name);
                foreach (XmlNode innerNode in myNode.ChildNodes)
                {
                    if (innerNode.Name == "ID" )
                    {
                        // added underscore "Parent" to prevent name clashes.
                        dt.Columns.Add("Parent" + myNode.Name + "ID");
                    }
                    //TODO - coonverting the .Contains call to BinarySearch would 
                    //       allow for the injection of a custom implementation of IComparer
                    //       in order to do a case insensitive search.
                    if (e != null 
                        && e.Fields.Contains(innerNode.Name) 
                        && !dt.Columns.Contains(myNode.Name + innerNode.Name))
                    {
                        dt.Columns.Add(myNode.Name + innerNode.Name);
                    }
                }
            }
        }

        private void addParentIdValues(DiscoverQuery dq, DataTable dt, string[] data, XmlNode fld)
        {
            int colIdx = -1;
            XmlNode myNode;
            myNode = fld;
            xmlaElement e;
            while (myNode.ParentNode != null)
            {

                myNode = myNode.ParentNode;
                e = dq.Elements.FindByName(myNode.Name);
                foreach (XmlNode innerNode in myNode.ChildNodes)
                {
                    if (innerNode.Name == "ID")
                    {
                        colIdx = dt.Columns.IndexOf("Parent" + myNode.Name + "ID");
                        if (colIdx > -1)
                        {
                            data[colIdx] = innerNode.FirstChild.Value;
                        }
                    }
                    //TODO - coonverting the .Contains call to BinarySearch would 
                    //       allow for the injection of a custom implementation of IComparer
                    //       in order to do a case insensitive search.
                    if (e != null && e.Fields.Contains(innerNode.Name))
                    {
                        colIdx = dt.Columns.IndexOf(myNode.Name + innerNode.Name);
                        if (colIdx > -1)
                        {
                            data[colIdx] = innerNode.FirstChild.Value;
                        }
                    }
                }
            }
        }



        private class xmlaElement
        {
            private List<string> mFields;
            private List<string> mAttributes;
            private string mName;
            private int mFieldCount;
            private LinkedList<xmlaElement> mParentList;

            public xmlaElement(LinkedList<xmlaElement> parentList, string element)
            {
                mParentList = parentList;
                string[] chunks;
                chunks = element.Split("|".ToCharArray());
                mName = chunks[0];
                mFields = new List<string>();
                mAttributes = new List<string>();
                mFieldCount = chunks.Length;
                if (mFieldCount > 1)
                {
                    string[] fldList = chunks[1].Split(",".ToCharArray());
                    for (int i = 0; i < fldList.Length; i++)
                    {
                        if (fldList[i].StartsWith("@"))
                        {
                            mAttributes.Add(fldList[i].Substring(1).Trim());
                        }
                        else
                        {
                            mFields.Add(fldList[i].Trim());
                        }
                    }
                }
            }

            public string Name
            {
                get { return mName; }
            }

            public List<string> Fields
            {
                get { return mFields; }
            }

            public List<string> Attributes
            {
                get { return mAttributes;}
            }

            //public int FieldCount
            //{
            //    get { return mFieldCount; }
            //}

            public xmlaElement Next
            {
                get
                {
                    LinkedListNode<xmlaElement> me = mParentList.Find(this);
                    if (me != null)
                    {
                        if (me.Next != null)
                        {
                            return mParentList.Find(this).Next.Value;
                        }
                    }
                    return null;
                }
            }

            public xmlaElement Previous
            {
                get
                {
                    LinkedListNode<xmlaElement> me = mParentList.Find(this);
                    if (me != null)
                    {
                        if (me.Previous != null)
                        {

                            return mParentList.Find(this).Previous.Value;
                        }
                    }
                    return null;
                }
            }
        }

        private class DiscoverQuery
        {
            private xmlaElementList mElements = new xmlaElementList();

            public DiscoverQuery(string[] elements)
            {

                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].Length > 0)
                    {
                        mElements.AddLast(new xmlaElement(mElements, elements[i]));
                    }
                }
            }

            public xmlaElementList Elements
            {
                get { return mElements; }
            }

        }


        private class xmlaElementList : LinkedList<xmlaElement>
        {
            public xmlaElement FindByName(string name)
            {
                foreach (xmlaElement e in this)
                {
                    if (e.Name == name)
                    {
                        return e;
                    }
                }
                return null;
            }
        }

    }
}
