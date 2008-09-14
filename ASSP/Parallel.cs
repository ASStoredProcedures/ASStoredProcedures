/*============================================================================
  File:    Parallel.cs

  Summary: Implements a set of functions which allow expensive calculations to be multithreaded

  Date:   9th January 2007

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
using System.Threading;
using Microsoft.AnalysisServices.AdomdServer;
using Microsoft.AnalysisServices.AdomdClient;

namespace ASStoredProcs
{
    public class Parallel
    {

        public static Microsoft.AnalysisServices.AdomdServer.Set ParallelGenerate(Microsoft.AnalysisServices.AdomdServer.Set IterationSet, string SetExpression)
        {
            List<ParallelQueryThreadInfo> threadInfos = new List<ParallelQueryThreadInfo>();
            string connectionString = "Data Source=" + Context.CurrentServerID + ";Provider=msolap.3;initial catalog=" + Context.CurrentDatabaseName + ";";

            foreach (Microsoft.AnalysisServices.AdomdServer.Tuple t in IterationSet)
            {
                //build the text of current tuple
                string tupleText = "(";
                for (int n = 1; n <= t.Members.Count; n++)
                {
                    tupleText += t.Members[n - 1].UniqueName;
                    if (n < t.Members.Count)
                        tupleText += ",";
                }
                tupleText += ")";

                //build the object that will be passed to the worker thread
                ParallelQueryThreadInfo info = new ParallelQueryThreadInfo();
                info.connectionString = connectionString;
                info.query = "with member measures.internalcalc as SetToStr(" + SetExpression + ") select measures.internalcalc on 0 from [" + Context.CurrentCube.Name + "] where(" + tupleText + ")";
                info.autoEvent = new AutoResetEvent(false);
                threadInfos.Add(info);
                ThreadPool.QueueUserWorkItem(new WaitCallback(RunAQuery), info);
            }

            StringBuilder sFinalSet = new StringBuilder("{");
            for (int i = 0; i < threadInfos.Count; i++)
            {
                //wait until they've finished
                threadInfos[i].autoEvent.WaitOne();
                if (threadInfos[i].ex != null) throw threadInfos[i].ex;
                if (i > 0) sFinalSet.Append(" + ");
                sFinalSet.Append(threadInfos[i].returnValue);
            }
            sFinalSet.Append("}");

            //union the sets they return
            return MDX.StrToSet(sFinalSet.ToString());
        }

        public static Microsoft.AnalysisServices.AdomdServer.Set ParallelUnion(string SetExpression1, string SetExpression2)
        {
            List<ParallelQueryThreadInfo> threadInfos = new List<ParallelQueryThreadInfo>();
            string connectionString = "Data Source=" + Context.CurrentServerID + ";Provider=msolap.3;initial catalog=" + Context.CurrentDatabaseName + ";";

            for (int n = 0; n < 2; n++)
            {
                //build the object that will be passed to the worker thread
                ParallelQueryThreadInfo info = new ParallelQueryThreadInfo();
                info.connectionString = connectionString;
                if (n == 0)
                    info.query = "with member measures.internalcalc as SetToStr(" + SetExpression1 + ") select measures.internalcalc on 0 from [" + Context.CurrentCube.Name + "]";
                else
                    info.query = "with member measures.internalcalc as SetToStr(" + SetExpression2 + ") select measures.internalcalc on 0 from [" + Context.CurrentCube.Name + "]";
                info.autoEvent = new AutoResetEvent(false);
                threadInfos.Add(info);
                ThreadPool.QueueUserWorkItem(new WaitCallback(RunAQuery), info);
            }

            StringBuilder sFinalSet = new StringBuilder("{");
            for (int i = 0; i < threadInfos.Count; i++)
            {
                //wait until they've finished
                threadInfos[i].autoEvent.WaitOne();
                if (threadInfos[i].ex != null) throw threadInfos[i].ex;
                if (i > 0) sFinalSet.Append(" + ");
                sFinalSet.Append(threadInfos[i].returnValue);
            }
            sFinalSet.Append("}");

            return MDX.StrToSet(sFinalSet.ToString());
        }

        private static void RunAQuery(object o)
        {
            ParallelQueryThreadInfo info = null;
            try
            {
                info = (ParallelQueryThreadInfo)o;
                Microsoft.AnalysisServices.AdomdClient.AdomdConnection conn = new Microsoft.AnalysisServices.AdomdClient.AdomdConnection(info.connectionString);
                conn.Open();
                try
                {
                    CellSet queryCellset;
                    Microsoft.AnalysisServices.AdomdClient.AdomdCommand queryCommand = new Microsoft.AnalysisServices.AdomdClient.AdomdCommand();
                    queryCommand.CommandText = info.query;
                    queryCommand.Connection = conn;
                    queryCellset = queryCommand.ExecuteCellSet();
                    info.returnValue = queryCellset[0].Value.ToString();
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                info.ex = ex;
            }
            finally
            {
                info.autoEvent.Set();
            }
        }

        private class ParallelQueryThreadInfo
        {
            public string query;
            public string connectionString;
            public string returnValue;
            public AutoResetEvent autoEvent;
            public Exception ex;
        }

    }
}
