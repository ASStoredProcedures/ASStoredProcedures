/*============================================================================
  File:    PartitionHealthCheck.cs

  Summary: Provides functions which allow you to check that partition slices
           do not overlap and that indexes have been created

  Date:    November 2nd, 2007

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.AdomdServer;
using System.Data;
using ASStoredProcs;
using System.Collections ;


namespace ASStoredProcs
{
    
    public class PartitionHealthCheck
    {
        [SafeToPrepare(true)]
        public DataTable DiscoverPartitionSlices(string cubeName, string measureGroupName)
        {
            DataTable dt;
            DataTable dtTemp;
            string props;
            XmlaDiscover xd = new XmlaDiscover();
            int dimCount;
            int i;
            DataRow[] sameDimRows;
            
            string overlapText;
            bool notFirstDim = false;
            

            Server server = new Server();
            server.Connect("*");
            Database db = server.Databases.GetByName(Context.CurrentDatabaseName);
            Cube cube = db.Cubes.GetByName(cubeName);
            MeasureGroup mg = cube.MeasureGroups.GetByName(measureGroupName);
            props = "<DATABASE_NAME>" + Context.CurrentDatabaseName + "</DATABASE_NAME>";
            props += "<CUBE_NAME>" + cubeName + "</CUBE_NAME><MEASURE_GROUP_NAME>" + measureGroupName + "</MEASURE_GROUP_NAME>";

            //get info for the first partition in the measure group
            dt = xd.Discover("DISCOVER_PARTITION_DIMENSION_STAT", props + "<PARTITION_NAME>" + mg.Partitions[0].Name + "</PARTITION_NAME>");
            dt.Columns.Add("Overlap", System.Type.GetType("System.String"));
            dt.AcceptChanges();

            dimCount = dt.Rows.Count;

            //get info for other partitions, if they exist
            if (mg.Partitions.Count > 1)
            {
                for (i = 1; i < mg.Partitions.Count; i++)
                {
                    Context.CheckCancelled();

                    dtTemp = xd.Discover("DISCOVER_PARTITION_DIMENSION_STAT", props + "<PARTITION_NAME>" + mg.Partitions[i].Name + "</PARTITION_NAME>");
                    dtTemp.Columns.Add("Overlap", System.Type.GetType("System.String"));
                    dtTemp.AcceptChanges();

                    dt.Merge(dtTemp);
                }
                //work out if partitions overlap
                foreach(DataRow currentRow in dt.Rows)
                {
                    Context.CheckCancelled();

                    if (currentRow["ATTRIBUTE_INDEXED"].ToString() == "true")
                    {
                        overlapText = "";
                        sameDimRows = dt.Select("DIMENSION_NAME='" + currentRow["DIMENSION_NAME"] + "' AND ATTRIBUTE_NAME='" + currentRow["ATTRIBUTE_NAME"] + "' AND PARTITION_NAME<>'" + currentRow["PARTITION_NAME"] + "' AND ATTRIBUTE_INDEXED='true'");
                        notFirstDim = false;
                        foreach (DataRow dr in sameDimRows)
                        {
                            Context.CheckCancelled();

                            if (
                                (
                                (Int32.Parse(dr["ATTRIBUTE_COUNT_MIN"].ToString()) <= Int32.Parse(currentRow["ATTRIBUTE_COUNT_MIN"].ToString()))
                                &&
                                (Int32.Parse(currentRow["ATTRIBUTE_COUNT_MIN"].ToString()) <= Int32.Parse(dr["ATTRIBUTE_COUNT_MAX"].ToString()))
                                )
                                ||
                                (
                                (Int32.Parse(dr["ATTRIBUTE_COUNT_MIN"].ToString()) <= Int32.Parse(currentRow["ATTRIBUTE_COUNT_MAX"].ToString()))
                                &&
                                (Int32.Parse(currentRow["ATTRIBUTE_COUNT_MAX"].ToString()) <= Int32.Parse(dr["ATTRIBUTE_COUNT_MAX"].ToString()))
                                )
                                ||
                                (
                                (Int32.Parse(currentRow["ATTRIBUTE_COUNT_MIN"].ToString()) <= Int32.Parse(dr["ATTRIBUTE_COUNT_MIN"].ToString())) && (Int32.Parse(currentRow["ATTRIBUTE_COUNT_MAX"].ToString()) >= Int32.Parse(dr["ATTRIBUTE_COUNT_MAX"].ToString()))
                                )
                               )
                            {
                                if (notFirstDim)
                                    overlapText+=", ";
                                overlapText+=dr["PARTITION_NAME"];
                                notFirstDim = true;
                            }


                        }

                        currentRow["Overlap"] = overlapText;
                        

                    }
                    dt.AcceptChanges();
                }

            }

            server.Disconnect();



            return dt;
        }
    }
}
