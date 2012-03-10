/*============================================================================
  File:    Drillthrough.cs

  Summary: Implements stored procedures that aid in performing custom drillthrough.

  Date:    August 28, 2009

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
using Tuple = Microsoft.AnalysisServices.AdomdServer.Tuple; //resolves ambiguous reference in .NET 4 with System.Tuple


namespace ASStoredProcs
{

    public class Drillthrough
    {

        //in the following functions, if maxrows is not specified, it uses the DefaultDrillthroughMaxRows server setting

        public static string GetDefaultDrillthroughMDX()
        {
            return GetDrillthroughMDXInternal(null, null, null);
        }

        public static string GetDefaultDrillthroughMDX(Tuple tuple)
        {
            return GetDrillthroughMDXInternal(tuple, null, null);
        }

        public static string GetDefaultDrillthroughMDX(Tuple tuple, int iMaxRows)
        {
            return GetDrillthroughMDXInternal(tuple, null, iMaxRows);
        }

        public static string GetCustomDrillthroughMDX(string sReturnColumns)
        {
            return GetDrillthroughMDXInternal(null, sReturnColumns, null);
        }

        public static string GetCustomDrillthroughMDX(string sReturnColumns, Tuple tuple)
        {
            return GetDrillthroughMDXInternal(tuple, sReturnColumns, null);
        }

        public static string GetCustomDrillthroughMDX(string sReturnColumns, Tuple tuple, int iMaxRows)
        {
            return GetDrillthroughMDXInternal(tuple, sReturnColumns, iMaxRows);
        }

        private static string GetDrillthroughMDXInternal(Tuple tuple, string sReturnColumns, int? iMaxRows)
        {
            if (sReturnColumns != null)
            {
                //passed in a set of return columns
                return "drillthrough " + (iMaxRows == null ? "" : "maxrows " + iMaxRows) + " select (" + CurrentCellAttributes(tuple) + ") on 0 from [" + AMOHelpers.GetCurrentCubeName() + "] return " + sReturnColumns;
            }
            else
            {
                //passed in a reference to a measure, so just do the default drillthrough for it
                return "drillthrough " + (iMaxRows == null ? "" : "maxrows " + iMaxRows) + " select (" + CurrentCellAttributes(tuple) + ") on 0 from [" + AMOHelpers.GetCurrentCubeName() + "]";
            }
        }

        public static DataTable ExecuteDrillthroughAndFixColumns(string sDrillthroughMDX)
        {
            AdomdClient.AdomdConnection conn = TimeoutUtility.ConnectAdomdClient("Data Source=" + Context.CurrentServerID + ";Initial Catalog=" + Context.CurrentDatabaseName + ";Application Name=ASSP");
            try
            {
                AdomdClient.AdomdCommand cmd = new AdomdClient.AdomdCommand();
                cmd.Connection = conn;
                cmd.CommandText = sDrillthroughMDX;
                DataTable tbl = new DataTable();
                AdomdClient.AdomdDataAdapter adp = new AdomdClient.AdomdDataAdapter(cmd);
                TimeoutUtility.FillAdomdDataAdapter(adp, tbl);

                Dictionary<string, int> dictColumnNames = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

                foreach (DataColumn col in tbl.Columns)
                {
                    string sNewColumnName = col.ColumnName.Substring(col.ColumnName.LastIndexOf('.') + 1).Replace("[", "").Replace("]", "");
                    if (dictColumnNames.ContainsKey(sNewColumnName))
                        dictColumnNames[sNewColumnName]++;
                    else
                        dictColumnNames.Add(sNewColumnName, 1);
                }

                foreach (DataColumn col in tbl.Columns)
                {
                    string sNewColumnName = col.ColumnName.Substring(col.ColumnName.LastIndexOf('.') + 1).Replace("[", "").Replace("]", "");
                    if (dictColumnNames[sNewColumnName] > 1)
                        sNewColumnName = col.ColumnName.Substring(col.ColumnName.LastIndexOf('[') + 1).Replace("[", "").Replace("]", "").Replace("$", "");
                    if (!tbl.Columns.Contains(sNewColumnName))
                        col.ColumnName = sNewColumnName;
                }

                return tbl;
            }
            finally
            {
                conn.Close();
            }
        }

        public static string CurrentCellAttributes()
        {
            return CurrentCellAttributesForCube(AMOHelpers.GetCurrentCubeName());
        }

        public static string CurrentCellAttributesForCube(string CubeName)
        {
            // start with empty string
            StringBuilder coordinate = new StringBuilder();
            bool first = true;
            foreach (Dimension d in Context.Cubes[CubeName].Dimensions)
            {
                foreach (Hierarchy h in d.AttributeHierarchies)
                {
                    // skip user hierarchies - consider attribute and parent-child hierarchies
                    // (parent-child is both user and attribute hierarchy)
                    if (h.HierarchyOrigin == HierarchyOrigin.UserHierarchy)
                        continue;

                    // skip calculated measures
                    if (d.DimensionType == DimensionTypeEnum.Measure && h.CurrentMember.Type == MemberTypeEnum.Formula)
                        continue;

                    if (!first)
                        coordinate.Append("\r\n,");
                    else
                        first = false;

                    coordinate.Append(h.CurrentMember.UniqueName);
                }
            }

            return coordinate.ToString();
        }

        public static string CurrentCellAttributes(Tuple tuple)
        {
            if (tuple == null) return CurrentCellAttributes();

            // start with empty string
            StringBuilder coordinate = new StringBuilder();
            bool first = true;
            foreach (Dimension d in Context.Cubes[AMOHelpers.GetCurrentCubeName()].Dimensions)
            {
                foreach (Hierarchy h in d.AttributeHierarchies)
                {
                    // skip user hierarchies - consider attribute and parent-child hierarchies
                    // (parent-child is both user and attribute hierarchy)
                    if (h.HierarchyOrigin == HierarchyOrigin.UserHierarchy)
                        continue;

                    // skip calculated measures
                    string sOverrideMeasure = null;
                    if (d.DimensionType == DimensionTypeEnum.Measure)
                    {
                        foreach (Member m in tuple.Members)
                        {
                            if (m.ParentLevel.ParentHierarchy.ParentDimension.DimensionType == DimensionTypeEnum.Measure)
                            {
                                sOverrideMeasure = m.UniqueName;
                                break;
                            }
                        }

                        if (sOverrideMeasure == null && h.CurrentMember.Type == MemberTypeEnum.Formula)
                            continue;
                    }

                    if (!first)
                        coordinate.Append("\r\n,");
                    else
                        first = false;

                    if (sOverrideMeasure != null)
                        coordinate.Append(sOverrideMeasure);
                    else //calculate it in the context of the tuple
                        coordinate.Append(new Expression(h.UniqueName + ".CurrentMember.UniqueName").Calculate(tuple).ToString());
                }
            }

            return coordinate.ToString();
        }


        //don't use this in the MDX script or it may return calculated measures from the previous processed version or blow up if the cube was not previously processed
        public static Set GetMeasureGroupCalculatedMeasures(string sMeasureGroupName)
        {
            StringBuilder sb = new StringBuilder();
            XmlaDiscover discover = new XmlaDiscover();
            DataTable table = discover.Discover("MDSCHEMA_MEASURES", "<CUBE_NAME>" + AMOHelpers.GetCurrentCubeName() + "</CUBE_NAME><MEASUREGROUP_NAME>" + sMeasureGroupName + "</MEASUREGROUP_NAME>");
            foreach (DataRow row in table.Rows)
            {
                if (Convert.ToInt32(row["MEASURE_AGGREGATOR"]) == 127)
                {
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(row["MEASURE_UNIQUE_NAME"].ToString());
                }
            }
            return new Expression("{" + sb.ToString() + "}").CalculateMdxObject(null).ToSet();
        }

    }
}
