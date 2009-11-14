/*============================================================================
  File:    TimeoutUtility.cs

  Summary: A utility class which helps with this scenario:
            1. An MDX query is executed which calls an ASSP sproc.
            2. Cube processing finishes and asks for a commit lock. The lock cannot be granted until the MDX query completes.
            3. The ASSP sproc then opens a connection to SSAS (AMO or AdomdClient). This new connection is blocked by #2 which is blocked by #1. A deadlock is reached.
           This utility class helps the sproc not hang on certain AMO or AdomdClient operations.

  Date:    November 13, 2009

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
using Microsoft.AnalysisServices; //reference to AMO
using System.Configuration;

namespace ASStoredProcs
{
    class TimeoutUtility
    {
        #region AMO
        /// <summary>
        /// Returns an open connection to an AMO server
        /// Connection is opened on a background thread
        /// Helps ensure a sproc trying to open an AMO connection does not get deadlocked
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static Microsoft.AnalysisServices.Server ConnectAMOServer(string ConnectionString)
        {
            ConnectAMOServerInfo info = new ConnectAMOServerInfo();
            info.ConnectionString = ConnectionString;
            info.autoEvent = new System.Threading.AutoResetEvent(false);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(OpenAMOServerConnection), info);

            while (!info.autoEvent.WaitOne(1000, false))
            {
                Context.CheckCancelled(); //if the parent query has been cancelled (or the ForceCommitTimeout expires) then this will immediately exit
            }

            if (info.ex != null)
            {
                throw info.ex;
            }
            else
            {
                return info.Server;
            }
        }

        private class ConnectAMOServerInfo
        {
            public string ConnectionString;
            public Microsoft.AnalysisServices.Server Server;
            public System.Threading.AutoResetEvent autoEvent;
            public Exception ex;
        }

        private static void OpenAMOServerConnection(object o)
        {
            ConnectAMOServerInfo info = null;
            try
            {
                info = (ConnectAMOServerInfo)o;
                Microsoft.AnalysisServices.Server s = new Microsoft.AnalysisServices.Server();
                s.Connect(info.ConnectionString);
                info.Server = s;
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
        #endregion


        #region AdomdClient
        /// <summary>
        /// Returns an open connection with AdomdClient
        /// Connection is opened on a background thread
        /// Helps ensure a sproc trying to open an AdomdClient connection does not get deadlocked
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static Microsoft.AnalysisServices.AdomdClient.AdomdConnection ConnectAdomdClient(string ConnectionString)
        {
            ConnectAdomdClientInfo info = new ConnectAdomdClientInfo();
            info.ConnectionString = ConnectionString;
            info.autoEvent = new System.Threading.AutoResetEvent(false);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(OpenAdomdClientConnection), info);

            while (!info.autoEvent.WaitOne(1000, false))
            {
                Context.CheckCancelled(); //if the parent query has been cancelled (or the ForceCommitTimeout expires) then this will immediately exit
            }

            if (info.ex != null)
            {
                throw info.ex;
            }
            else
            {
                return info.Server;
            }
        }

        private class ConnectAdomdClientInfo
        {
            public string ConnectionString;
            public Microsoft.AnalysisServices.AdomdClient.AdomdConnection Server;
            public System.Threading.AutoResetEvent autoEvent;
            public Exception ex;
        }

        private static void OpenAdomdClientConnection(object o)
        {
            ConnectAdomdClientInfo info = null;
            try
            {
                info = (ConnectAdomdClientInfo)o;
                Microsoft.AnalysisServices.AdomdClient.AdomdConnection s = new Microsoft.AnalysisServices.AdomdClient.AdomdConnection(info.ConnectionString);
                s.Open();
                info.Server = s;
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
        #endregion


        #region XmlaClientDiscover
        /// <summary>
        /// Fills a DataTable using the specified AdomdDataAdapter
        /// This is done on a background thread
        /// Helps ensure a sproc trying to fill a DataTable does not get deadlocked
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestType"></param>
        /// <param name="restrictions"></param>
        /// <param name="properties"></param>
        /// <param name="result"></param>
        /// <param name="skipResult"></param>
        /// <param name="restrictionsXmlIsComplete"></param>
        /// <param name="propertiesXmlIsComplete"></param>
        public static void XmlaClientDiscover(Microsoft.AnalysisServices.Xmla.XmlaClient client, string requestType, string restrictions, string properties, out string result, bool skipResult, bool restrictionsXmlIsComplete, bool propertiesXmlIsComplete)
        {
            XmlaClientDiscoverInfo info = new XmlaClientDiscoverInfo();
            info.requestType = requestType;
            info.restrictions = restrictions;
            info.properties = properties;
            info.skipResult = skipResult;
            info.restrictionsXmlIsComplete = restrictionsXmlIsComplete;
            info.propertiesXmlIsComplete = propertiesXmlIsComplete;
            info.client = client;
            info.autoEvent = new System.Threading.AutoResetEvent(false);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(XmlaClientDiscoverWorker), info);

            while (!info.autoEvent.WaitOne(1000, false))
            {
                Context.CheckCancelled(); //if the parent query has been cancelled (or the ForceCommitTimeout expires) then this will immediately exit
            }

            if (info.ex != null)
            {
                throw info.ex;
            }
            else
            {
                result = info.result;
            }
        }

        private class XmlaClientDiscoverInfo
        {
            public Microsoft.AnalysisServices.Xmla.XmlaClient client;
            public string requestType;
            public string restrictions;
            public string properties;
            public string result;
            public bool skipResult;
            public bool restrictionsXmlIsComplete;
            public bool propertiesXmlIsComplete;
            public System.Threading.AutoResetEvent autoEvent;
            public Exception ex;
        }

        private static void XmlaClientDiscoverWorker(object o)
        {
            XmlaClientDiscoverInfo info = null;
            try
            {
                info = (XmlaClientDiscoverInfo)o;
                info.client.Discover(info.requestType, info.restrictions, info.properties, out info.result, info.skipResult, info.restrictionsXmlIsComplete, info.propertiesXmlIsComplete);
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
        #endregion


        #region AdomdDataAdapter
        /// <summary>
        /// Fills a DataTable using the specified AdomdDataAdapter
        /// This is done on a background thread
        /// Helps ensure a sproc trying to fill a DataTable does not get deadlocked
        /// </summary>
        /// <param name="adp"></param>
        /// <param name="tbl"></param>
        public static void FillAdomdDataAdapter(Microsoft.AnalysisServices.AdomdClient.AdomdDataAdapter adp, System.Data.DataTable tbl)
        {
            FillAdomdDataAdapterInfo info = new FillAdomdDataAdapterInfo();
            info.adapter = adp;
            info.table = tbl;
            info.autoEvent = new System.Threading.AutoResetEvent(false);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(FillAdomdDataAdapterWorker), info);

            while (!info.autoEvent.WaitOne(1000, false))
            {
                Context.CheckCancelled(); //if the parent query has been cancelled (or the ForceCommitTimeout expires) then this will immediately exit
            }

            if (info.ex != null)
            {
                throw info.ex;
            }
        }

        private class FillAdomdDataAdapterInfo
        {
            public Microsoft.AnalysisServices.AdomdClient.AdomdDataAdapter adapter;
            public System.Data.DataTable table;
            public System.Threading.AutoResetEvent autoEvent;
            public Exception ex;
        }

        private static void FillAdomdDataAdapterWorker(object o)
        {
            FillAdomdDataAdapterInfo info = null;
            try
            {
                info = (FillAdomdDataAdapterInfo)o;
                info.adapter.Fill(info.table);
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
        #endregion

    }
}


