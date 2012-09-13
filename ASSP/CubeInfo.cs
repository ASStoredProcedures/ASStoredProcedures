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
        [SafeToPrepare(true)]
        public static DateTime GetCubeLastProcessedDate()
        {
            string sServerName = Context.CurrentServerID;
            string sDatabaseName = Context.CurrentDatabaseName;
            string sCubeName = AMOHelpers.GetCurrentCubeName();

            DateTime dtTemp = DateTime.MinValue;
            Exception exDelegate = null;

            System.Threading.Thread td = new System.Threading.Thread(delegate() 
            {
                try
                {
                    Microsoft.AnalysisServices.Server oServer = new Microsoft.AnalysisServices.Server();
                    oServer.Connect("Data Source=" + sServerName);
                    Database db = oServer.Databases.GetByName(sDatabaseName);
                    Cube cube =  db.Cubes.FindByName(sCubeName);

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

        [SafeToPrepare(true)]
        public static DateTime GetMeasureGroupLastProcessedDate(string MeasureGroupName)
        {
            string sServerName = Context.CurrentServerID;
            string sDatabaseName = Context.CurrentDatabaseName;
            string sCubeName = AMOHelpers.GetCurrentCubeName();

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

        /// <summary>
        /// Max() of last processed dates of all partitions and measure groups in the current cube
        /// </summary>
        [SafeToPrepare(true)]
        public static DateTime GetLastProcessedDateOverPartitions()
        {
            return GetLastProcessedDateOverPartitions(true);
        }

        /// <summary>
        /// Max() of last processed dates of all partitions in the current cube. 
        /// Additionaly last processed dates of measure groups can be considered.
        /// </summary>
        /// <param name="IncludeMeasureGroupLastProcessed">
        /// If a dimension was processed by ProcessUpdate and no aggregations were dropped in partitions,
        /// then last processed timestamps of individual partitions will not be updated,
        /// but the timestamps of measure groups will. 
        /// Set this parameter to true if you also want to monitor dimension processing.
        /// </param>
        [SafeToPrepare(true)]
        public static DateTime GetLastProcessedDateOverPartitions(bool IncludeMeasureGroupLastProcessed)
        {
            return GetLastProcessedDateOverPartitions(string.Empty, IncludeMeasureGroupLastProcessed);
        }
       
        /// <summary>
        /// Max() of last processed dates of all partitions in the current cube for a given measure group.
        /// </summary>
        /// <param name="MeasureGroupName">Name of the measure group. 
        /// If null or empty - all measure groups will be considered</param>
        /// <param name="IncludeMeasureGroupLastProcessed">
        /// If a dimension was processed by ProcessUpdate and no aggregations were dropped in partitions,
        /// then last processed timestamps of individual partitions will not be updated,
        /// but the timestamps of measure groups will. 
        /// Set this parameter to true if you also want to monitor dimension processing.
        /// </param>
        [SafeToPrepare(true)]
        public static DateTime GetLastProcessedDateOverPartitions(string MeasureGroupName, bool IncludeMeasureGroupLastProcessed)
        {
            return GetLastProcessedDateOverPartitions(MeasureGroupName, string.Empty, IncludeMeasureGroupLastProcessed);
        }

        /// <summary>
        /// Last processed date for a given partition in a given measure group in the current cube
        /// or Max() if MeasureGroupName/PartitionName is null or empty.
        /// </summary>
        /// <param name="MeasureGroupName">Name of the measure group. 
        /// If null or empty - all measure groups will be considered.
        /// May not be null or empty if PartitionName is not null or empty</param>
        /// <param name="PartitionName">Name of the partition.
        /// If null or empty - all partitions (of a measure group) will be considered.</param>
        /// <param name="IncludeMeasureGroupLastProcessed">
        /// If a dimension was processed by ProcessUpdate and no aggregations were dropped in partitions,
        /// then last processed timestamps of individual partitions will not be updated,
        /// but the timestamps of measure groups will. 
        /// Set this parameter to true if you also want to monitor dimension processing.
        /// </param>
        [SafeToPrepare(true)]
        public static DateTime GetLastProcessedDateOverPartitions(string MeasureGroupName, string PartitionName, bool IncludeMeasureGroupLastProcessed)
        {
            return GetLastProcessedDateOverPartitions(string.Empty, MeasureGroupName, PartitionName, IncludeMeasureGroupLastProcessed);
        }

        /// <summary>
        /// Last processed date for a given partition in a given measure group in the given cube
        /// or Max() if MeasureGroupName/PartitionName is null or empty.
        /// </summary>
        /// <param name="CubeName">Cube to look at. If null or empty, current cube will be used</param>
        /// <param name="MeasureGroupName">Name of the measure group. 
        /// If null or empty - all measure groups will be considered.
        /// May not be null or empty if PartitionName is not null or empty</param>
        /// <param name="PartitionName">Name of the partition.
        /// If null or empty - all partitions (of a measure group) will be considered.</param>
        /// <param name="IncludeMeasureGroupLastProcessed">
        /// If a dimension was processed by ProcessUpdate and no aggregations were dropped in partitions,
        /// then last processed timestamps of individual partitions will not be updated,
        /// but the timestamps of measure groups will. 
        /// Set this parameter to true if you also want to monitor dimension processing.
        /// </param>
        [SafeToPrepare(true)]
        public static DateTime GetLastProcessedDateOverPartitions(string CubeName, string MeasureGroupName, string PartitionName, bool IncludeMeasureGroupLastProcessed)
        {
            string sServerName = Context.CurrentServerID;
            string sDatabaseName = Context.CurrentDatabaseName;

            string sCubeName;

            if (string.IsNullOrEmpty(CubeName))
            {
                sCubeName = AMOHelpers.GetCurrentCubeName();
            }
            else
            {
                sCubeName = CubeName;
            }

            if (!string.IsNullOrEmpty(PartitionName) && string.IsNullOrEmpty(MeasureGroupName))
            {
                throw new Exception("Measure group may not be empty if partition is explicitly specified");
            }

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

                    //If measure group is specified - get it. Otherwise iterate over all measure groups
                    if (!string.IsNullOrEmpty(MeasureGroupName))
                    {
                        dtTemp = FindMaxLastProcessedDateInMeasureGroup(cube.MeasureGroups.GetByName(MeasureGroupName), PartitionName, IncludeMeasureGroupLastProcessed);
                    }
                    else
                    {
                        foreach (MeasureGroup curMeasureGroup in cube.MeasureGroups)
                        {
                            DateTime curLastProcessedDate = FindMaxLastProcessedDateInMeasureGroup(curMeasureGroup, PartitionName, IncludeMeasureGroupLastProcessed);

                            if (dtTemp < curLastProcessedDate)
                            {
                                dtTemp = curLastProcessedDate;
                            }
                        }
                    }
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

        /// <summary>
        /// Helper method for iteration in a measure group
        /// </summary>
        /// <param name="MeasureGroup">Measure group object (may not be null)</param>
        /// <param name="PartitionName">Name of the partition.
        /// If null or empty - all partitions (of a measure group) will be considered.</param>
        /// <param name="IncludeMeasureGroupLastProcessed">
        /// If a dimension was processed by ProcessUpdate and no aggregations were dropped in partitions,
        /// then last processed timestamps of individual partitions will not be updated,
        /// but the timestamps of measure groups will. 
        /// Set this parameter to true if you also want to monitor dimension processing.
        /// </param>
        [SafeToPrepare(true)]
        private static DateTime FindMaxLastProcessedDateInMeasureGroup(MeasureGroup MeasureGroup, string PartitionName, bool IncludeMeasureGroupLastProcessed)
        {
            DateTime result = DateTime.MinValue;

            if (IncludeMeasureGroupLastProcessed)
            {
                result = MeasureGroup.LastProcessed;
            }
            
            //Look at one specific partition
            if (!string.IsNullOrEmpty(PartitionName))
            {
                Partition partition = MeasureGroup.Partitions.GetByName(PartitionName);

                if (result < partition.LastProcessed)
                {
                    result = partition.LastProcessed;
                }
            }
            else
            {
                //Iterate over all partitions and search for max()
                foreach (Partition curPartition in MeasureGroup.Partitions)
                {
                    if (result < curPartition.LastProcessed)
                    {
                        result = curPartition.LastProcessed;
                    }
                }
            }

            return result;
        }
    }
}