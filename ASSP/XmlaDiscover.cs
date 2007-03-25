/*============================================================================
  File:    Discover.cs

  Summary: The primary purpose of this class is to execute XMLA discover
           commands and to return the results as a DataTable. It also has some
           secondary methods that execute the Cancel and ClearCache commands.

  Date:    March 25, 2007

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/Wiki/View.aspx?ProjectName=ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/
using System;
using System.Data;
using Microsoft.AnalysisServices.Xmla;
using Microsoft.AnalysisServices.AdomdServer;
using Microsoft.AnalysisServices;
using System.Xml;

namespace ASStoredProcs
{
    public class XmlaDiscover
    {
        #region Discover

        [SafeToPrepare(true)]
        public DataTable Discover(string request)
        {
            return Discover(request, "", "");
        }

        [SafeToPrepare(true)]
        public DataTable Discover(string request, string restrictions)
        {
            return Discover(request, restrictions, "");
        }

        [SafeToPrepare(true)]
        public DataTable Discover(string request, string restrictions, string properties) 
        {
            XmlaClient client = createXmlaClientAndConnect();
            DataTable dt = new DataTable();
            // if no properties restriction is specified, default to using
            // the current database
            if (properties.Length == 0)
            {
                properties = string.Format("<CATALOG>{0}</CATALOG>", Context.CurrentDatabaseName);
            }
            try
            {
                string res;
                client.Discover(request, restrictions, properties, out res, false, false, false);

                dt = createDataTableFromXmla(res);
            }
            finally
            {
                client.Disconnect();
            }
            return dt;

        }

        private XmlaClient createXmlaClientAndConnect()
        {
            string connectionString = "Data Source=" + Context.CurrentServerID + ";Provider=msolap.3;initial catalog=" + Context.CurrentDatabaseName + ";";
            XmlaClient client;
            client = new XmlaClient();
            client.Connect(Context.CurrentServerID);
            return client;
        }

        // This routine is fairly "brute force", creating an XmlDocument and 
        // then iterating through the nodes
        private DataTable createDataTableFromXmla(string xmlaResult)
        {
            DataTable dt = null;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlaResult);
            
            //               return         root       schema/rows       Look for element with attribute "name" = row
            //                  ^             ^             ^             ^
            //xmlDom.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].Attributes.Count
            foreach (XmlNode n in doc.ChildNodes[0].ChildNodes[0].ChildNodes)
            {
                if ((n.NodeType == XmlNodeType.Element) && (n.LocalName == "schema"))
                {
                    dt = buildTableFromSchema(n);
                }
                if ((n.NodeType == XmlNodeType.Element) && (n.LocalName == "row"))
                {
                    // processRows
                    addTableRow(dt, n);
                }
            }            
            return dt;
        }

        private DataTable buildTableFromSchema(XmlNode n)
        {
            DataTable dt = new DataTable();

            foreach (XmlNode n2 in n.ChildNodes)
            {
                System.Diagnostics.Debug.WriteLine(n.Value);
                if ((n2.NodeType == XmlNodeType.Element) && (n2.LocalName == "complexType"))
                {
                    foreach (XmlAttribute a in n2.Attributes)
                    {
                        if ((a.Name == "name") && (a.Value == "row"))
                        {
                            // we have the row definition
                            //                                  Sequence
                            //                                   ^
                            foreach (XmlNode n3 in n2.ChildNodes[0].ChildNodes)
                            {
                                string fld = "";
                                Type typ = null;
                                foreach (XmlAttribute a2 in n3.Attributes)
                                {
                                    switch (a2.Name)
                                    {
                                        case "sql:field":
                                            fld = a2.Value;
                                            break;
                                        case "type":
                                            switch (a2.Value)
                                            {
                                                case "int":
                                                    typ = typeof(int);
                                                    break;
                                                case "string":
                                                    typ = typeof(string);
                                                    break;
                                                case "long":
                                                    typ = typeof(long);
                                                    break;
                                                case "datetime":
                                                    typ = typeof(DateTime);
                                                    break;
                                                default:
                                                    typ = typeof(string);
                                                    break;
                                            }


                                            break;
                                    }

                                }
                                if ((typ != null) && (fld != null))
                                {
                                    dt.Columns.Add(fld, typ);
                                }
                                // reset field and type variables
                                fld = null;
                                typ = null;
                            }
                        }
                    }
                }
            }
            return dt;
        }//buildTableFromSchema

        private void addTableRow(DataTable dt, XmlNode n)
        {
            DataRow dr = dt.NewRow();
            foreach (XmlNode e in n.ChildNodes)
            {
                if (dt.Columns.Contains(e.LocalName))
                {
                    dr[e.LocalName] = e.InnerText;
                }
            }
            dt.Rows.Add(dr);
        }
        #endregion

        #region Common Discover Functions
        [SafeToPrepare(true)]
        public DataTable DiscoverSessions()
        {
            return Discover("DISCOVER_SESSIONS");
        }

        [SafeToPrepare(true)]
        public DataTable DiscoverConnections()
        {
            return Discover("DISCOVER_CONNECTIONS");
        }

        [SafeToPrepare(true)]
        public DataTable DiscoverRowsets()
        {
            return Discover("DISCOVER_SCHEMA_ROWSETS");
        }

        #endregion

        #region ClearCache
        const string CLEARCACHE_TEMPLATE = "<Batch xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\"><ClearCache><Object><DatabaseID>{0}</DatabaseID>{1}</Object></ClearCache></Batch>";
        const string CUBEID_TEMPLATE = "<CubeID>{0}</CubeID>";

        public void ClearCache()
        {
            ClearCache("");
        }

        // Clears the cache for the current database
        public void ClearCache(string cubeName)
        {
            // only way to get a DatabaseID from a Database name appears to be to use AMO
            string dbId = "";
            Microsoft.AnalysisServices.Server svr = new Server();
            using (svr)
            {
                svr.Connect(Context.CurrentServerID);
                try
                {
                    Database db = svr.Databases.FindByName(Context.CurrentDatabaseName);
                    dbId = db.ID;
                    if (cubeName.Length != 0)
                    {
                        Cube c = db.Cubes.FindByName(cubeName);
                        if (c != null)
                        {
                            cubeName = "<CubeID>" + c.ID + "</CubeID>";
                        }
                        else
                        {
                            throw new Exception("The cube '" + cubeName + "' does not exist in the " + Context.CurrentDatabaseName + " database.");
                        }
                    }

                    // execute clear cache based on the ID
                    string clearCmd = string.Format(CLEARCACHE_TEMPLATE, dbId, cubeName);
                    svr.Execute(clearCmd);
                }
                catch 
                {   // re-throw any exception
                    throw;
                }
                finally
                {
                    // clean up
                    svr.Disconnect();
                }
            }
        }

        #endregion

        #region Cancel

        const string CANCEL_TEMPLATE = "<Cancel xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">{0}</Cancel>";
        
        // Cancel based on SPID
        public void CancelSPID(int spid)
        {
            string cancelCmd = string.Format(CANCEL_TEMPLATE, "<SPID>" + spid.ToString() + "</SPID>");
            executeCancel(cancelCmd);
        }

        // Cancel based on session guid
        public void CancelSession(string sessionGuid)
        {
            string cancelCmd = string.Format(CANCEL_TEMPLATE, "<SessionID>" + sessionGuid + "</SessionID>");
            executeCancel(cancelCmd);
        }
        
        //Cancel based on connection id
        public void CancelConnection(int connectionId)
        {
            string cancelCmd = string.Format(CANCEL_TEMPLATE, "<ConnectionID>" + connectionId.ToString() + "</ConnectionID>");
            executeCancel(cancelCmd);
        }

        // Cancel Helper function
        private void executeCancel(string cancelCmd)
        {
            XmlaClient client = createXmlaClientAndConnect();
            string res = string.Empty;
            try
            {
                client.Execute(cancelCmd, string.Empty, out res, false, true);
            }
            finally
            {
                client.Disconnect();
            }
        }

        #endregion

        
    } // MetaDataQueries class

}

