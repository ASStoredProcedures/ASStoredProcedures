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
     
        static List<string> queries;
        static List<string> resultSets;
        static string connectionString;

        public static Microsoft.AnalysisServices.AdomdServer.Set ParallelGenerate(Microsoft.AnalysisServices.AdomdServer.Set IterationSet, string SetExpression)
        {
            //generate all the MDX queries we'll be running
            queries = new List<string>();
            List<Thread> queryThreads = new List<Thread>();
            resultSets = new List<string>();
            Thread queryThread;
            string tupleText;

            foreach (Microsoft.AnalysisServices.AdomdServer.Tuple t in IterationSet)
            {
                //build the text of current tuple
                tupleText = "(";
                for(int n=1; n<=t.Members.Count; n++)
                {
                    tupleText += t.Members[n-1].UniqueName;
                    if(n<t.Members.Count)
                        tupleText += ",";
                }
                tupleText += ")";
                queries.Add("with member measures.internalcalc as settostr(" + SetExpression + ") select measures.internalcalc on 0 from [" + Context.CurrentCube.Name + "] where(" + tupleText + ")")   ; 
            }
            

            //work out what our connection string is
            connectionString = "Data Source=" + Context.CurrentServerID + ";Provider=msolap.3;initial catalog=" + Context.CurrentDatabaseName + ";";
            
            //fire off the threads to run the queries
            for (int n = 0; n < queries.Count; n++ )
            {
                queryThread = new Thread(new ParameterizedThreadStart(RunAQuery));
                queryThread.Start(n);
                queryThreads.Add(queryThread);
            }

            //wait until they've finished
            foreach (Thread t in queryThreads)
                t.Join();

            //union the sets they return
            return MDX.StrToSet("{" + String.Join(" + ", resultSets.ToArray()) + "}");
        }

        public static Microsoft.AnalysisServices.AdomdServer.Set ParallelUnion(string SetExpression1, string SetExpression2)
        {
            queries = new List<string>();
            resultSets = new List<string>();
            List<Thread> queryThreads = new List<Thread>();
            Thread queryThread;

            queries.Add("with member measures.internalcalc as settostr(" + SetExpression1 + ") select measures.internalcalc on 0 from [" + Context.CurrentCube.Name + "]");
            queries.Add("with member measures.internalcalc as settostr(" + SetExpression2 + ") select measures.internalcalc on 0 from [" + Context.CurrentCube.Name + "]");
            
            connectionString = "Data Source=" + Context.CurrentServerID + ";Provider=msolap.3;initial catalog=" + Context.CurrentDatabaseName + ";";

            for (int n = 0; n < 2; n++)
            {
                queryThread = new Thread(new ParameterizedThreadStart(RunAQuery));
                queryThread.Start(n);
                queryThreads.Add(queryThread);
            }

            //wait until they've finished
            foreach (Thread t in queryThreads)
                t.Join();

            //union the sets they return
            return MDX.StrToSet("{" + String.Join(" + ", resultSets.ToArray()) + "}");

        }
        private static void RunAQuery(object objectIndex)
        {
            int index = (int)objectIndex;
            CellSet queryCellset;
            Microsoft.AnalysisServices.AdomdClient.AdomdCommand queryCommand = new Microsoft.AnalysisServices.AdomdClient.AdomdCommand();
            queryCommand.CommandText = queries[index];
            AdomdConnection conn = new AdomdConnection(connectionString );
            conn.Open();
            queryCommand.Connection = conn;
            queryCellset  = queryCommand.ExecuteCellSet();
            resultSets.Add(queryCellset[0].Value.ToString());
            conn.Close();
        }



    }
}
