/*============================================================================
  File:    CubeInfo.cs

  Summary: Implements a function which returns the date when the current cube
           was last processed.

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
using System.Text;
using Microsoft.AnalysisServices.AdomdServer;
using Microsoft.AnalysisServices; //reference to AMO

namespace ASStoredProcs
{
    public class CubeInfo
    {
        //the assembly must be registered with unrestricted permissions for this function to succeed
        public static DateTime GetCubeLastProcessedDate()
        {
            Microsoft.AnalysisServices.Server oServer = new Microsoft.AnalysisServices.Server();
            oServer.Connect("Data Source=" + Context.CurrentServerID);
            DateTime dtTemp = oServer.Databases.GetByName(Context.CurrentDatabaseName).Cubes.GetByName(Context.CurrentCube.Name).LastProcessed;
            oServer.Disconnect();
            return dtTemp;
            //return Context.CurrentCube.LastProcessed; //this doesn't work because of a bug: https://connect.microsoft.com/SQLServer/feedback/ViewFeedback.aspx?FeedbackID=124606
        }

        public static DateTime GetMeasureGroupLastProcessedDate(string MeasureGroupName)
        {
            Microsoft.AnalysisServices.Server oServer = new Microsoft.AnalysisServices.Server();
            oServer.Connect("Data Source=" + Context.CurrentServerID);
            DateTime dtTemp = oServer.Databases.GetByName(Context.CurrentDatabaseName).Cubes.GetByName(Context.CurrentCube.Name).MeasureGroups.GetByName(MeasureGroupName).LastProcessed;
            oServer.Disconnect();
            return dtTemp;
            //return Context.CurrentCube.LastProcessed; //this doesn't work because of a bug: https://connect.microsoft.com/SQLServer/feedback/ViewFeedback.aspx?FeedbackID=124606
        }
    }
}
