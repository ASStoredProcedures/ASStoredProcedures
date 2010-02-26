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
            string sServerName = Context.CurrentServerID;
            string sDatabaseName = Context.CurrentDatabaseName;
            string sCubeName = Context.CurrentCube.Name;

            DateTime dtTemp = DateTime.MinValue;
            Exception exDelegate = null;

            System.Threading.Thread td = new System.Threading.Thread(delegate() 
            {
                try
                {
                    Microsoft.AnalysisServices.Server oServer = new Microsoft.AnalysisServices.Server();
                    oServer.Connect("Data Source=" + sServerName);
                    Database db = oServer.Databases.GetByName(sDatabaseName);
                    Cube cube = db.Cubes.FindByName(sCubeName);
                    // If the cube name is not found it must be a perspective, so we need to
                    // search all of the cubes for the perspective name
                    if (cube == null)
                    {
                        cube = FindBaseCubeForPerspective(sCubeName, db);
                    }
                    dtTemp = cube.LastProcessed;
                }
                catch (Exception ex)
                {
                    exDelegate = ex;
                }
            }
            );
            td.Start(); //run the delegate code
            while (!td.Join(1000)) //wait for up to a second for the delegate to finish
            {
                Context.CheckCancelled(); //if the delegate isn't done, check whether the parent query has been cancelled. If the parent query has been cancelled (or the ForceCommitTimeout expires) then this will immediately exit
            }

            if (exDelegate != null) throw exDelegate;

            return dtTemp;
            //return Context.CurrentCube.LastProcessed; //this doesn't work because of a bug: https://connect.microsoft.com/SQLServer/feedback/ViewFeedback.aspx?FeedbackID=124606
        }

        private static Cube FindBaseCubeForPerspective(string sCubeName, Database db)
        {
            Cube cube;
            foreach (Cube c in db.Cubes)
            {
                if (c.Perspectives.ContainsName(sCubeName))
                {
                    cube = c;
                    break;
                }
            }
            return cube;
        }

        public static DateTime GetMeasureGroupLastProcessedDate(string MeasureGroupName)
        {
            string sServerName = Context.CurrentServerID;
            string sDatabaseName = Context.CurrentDatabaseName;
            string sCubeName = Context.CurrentCube.Name;

            DateTime dtTemp = DateTime.MinValue;
            Exception exDelegate = null;

            System.Threading.Thread td = new System.Threading.Thread(delegate()
            {
                try
                {
                    Microsoft.AnalysisServices.Server oServer = new Microsoft.AnalysisServices.Server();
                    oServer.Connect("Data Source=" + sServerName);
                    Database db = oServer.Databases.GetByName(sDatabaseName);
                    Cube cube = db.Cubes.GetByName(sCubeName);
                    if (cube == null)
                    {
                        cube = FindBaseCubeForPerspective(sCubeName, db);
                    }
                    dtTemp = cube.MeasureGroups.GetByName(MeasureGroupName).LastProcessed;
                }
                catch (Exception ex)
                {
                    exDelegate = ex;
                }
            }
            );
            td.Start(); //run the delegate code
            while (!td.Join(1000)) //wait for up to a second for the delegate to finish
            {
                Context.CheckCancelled(); //if the delegate isn't done, check whether the parent query has been cancelled. If the parent query has been cancelled (or the ForceCommitTimeout expires) then this will immediately exit
            }

            if (exDelegate != null) throw exDelegate;

            return dtTemp;
        }
    }
}
