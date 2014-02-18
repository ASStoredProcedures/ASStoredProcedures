/*============================================================================
  File:    CurrentCommand.cs

  Summary: Implements stored procedures that detect the command that is currently executing.

  Date:    February 18, 2014

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
using System.Text;
using Microsoft.AnalysisServices.AdomdServer;
using AdomdClient = Microsoft.AnalysisServices.AdomdClient;
using System.Data;


namespace ASStoredProcs
{

    public class CurrentCommand
    {

        public static string GetCurrentCommand()
        {
            AdomdClient.AdomdConnection conn = TimeoutUtility.ConnectAdomdClient("Data Source=" + Context.CurrentServerID + ";Initial Catalog=" + Context.CurrentDatabaseName + ";Application Name=ASSP");
            try
            {
                AdomdClient.AdomdRestrictionCollection restrictions = new AdomdClient.AdomdRestrictionCollection();
                //a restriction on SESSION_ID causes it to return no rows: http://msdn.microsoft.com/en-us/library/ee301976(v=sql.105).aspx#id253
                string sSessionID = Context.CurrentConnection.SessionID;
                
                System.Data.DataSet dataSet = TimeoutUtility.GetSchemaDataSet(conn, "DISCOVER_SESSIONS", restrictions);
                if (dataSet != null
                    && dataSet.Tables.Count > 0
                    && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in dataSet.Tables[0].Rows)
                    {
                        if (string.Compare(sSessionID, Convert.ToString(row["SESSION_ID"]), true) == 0)
                        {
                            return Convert.ToString(row["SESSION_LAST_COMMAND"]);
                        }
                    }
                    throw new Exception("Can't find the current command");
                }
                else
                {
                    throw new Exception("Can't get the current command");
                }
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool CurrentCommandIsDiscover()
        {
            string sCmd = GetCurrentCommand();
            Context.TraceEvent(1, 1, sCmd);
            if (sCmd.ToUpper().StartsWith("DISCOVER_")
                || sCmd.ToUpper().StartsWith("MDSCHEMA_"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
