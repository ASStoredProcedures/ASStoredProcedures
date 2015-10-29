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
using Tuple = Microsoft.AnalysisServices.AdomdServer.Tuple;
using System.Text.RegularExpressions; //resolves ambiguous reference in .NET 4 with System.Tuple


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
            AdomdClient.AdomdConnection conn = TimeoutUtility.ConnectAdomdClient("Data Source=" + Context.CurrentServerID + ";Initial Catalog=" + Context.CurrentDatabaseName + ";Application Name=ASSP;" );
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

        public static DataTable ExecuteDrillthroughAndTranslateColumns(string sDrillthroughMDX)
        {
            Regex columnNameRegex = new Regex( @"\[(?<cube>[^]]*)]\.\[(?<level>[^]]*)]", RegexOptions.Compiled) ;
            AdomdClient.AdomdConnection conn = TimeoutUtility.ConnectAdomdClient("Data Source=" + Context.CurrentServerID + ";Initial Catalog=" + Context.CurrentDatabaseName + ";Application Name=ASSP;Locale Identifier=" + Context.CurrentConnection.ClientCulture.LCID);
            try
            {
                Dictionary<string, string> translations = new Dictionary<string, string>();
                // get level names
                var resColl = new AdomdClient.AdomdRestrictionCollection();
                resColl.Add("CUBE_SOURCE", "3"); // dimensions
                resColl.Add("LEVEL_VISIBILITY", "3"); // visible and non-visible
                resColl.Add("CUBE_NAME", Context.CurrentCube); // visible and non-visible
                var dsLevels = conn.GetSchemaDataSet("MDSCHEMA_LEVELS", resColl);
                foreach (DataRow dr in dsLevels.Tables[0].Rows)
                {
                    var sColName = string.Format("[${0}.[{1}]", dr["DIMENSION_UNIQUE_NAME"].ToString().Substring(1), dr["LEVEL_NAME"].ToString());
                    if (!translations.ContainsKey(sColName))
                    {
                        translations.Add(sColName, dr["LEVEL_CAPTION"].ToString());
                    }
                }

                // get measure names
                resColl.Clear();
                resColl.Add("CUBE_NAME", Context.CurrentCube);
                resColl.Add("MEASURE_VISIBILITY", 3); // visible and non-visible
                var dsMeasures = conn.GetSchemaDataSet("MDSCHEMA_MEASURES", resColl);
                foreach (DataRow dr in dsMeasures.Tables[0].Rows)
                {
                    if (!translations.ContainsKey(string.Format("[{0}].[{1}]", dr["MEASUREGROUP_NAME"].ToString(), dr["MEASURE_NAME"].ToString())))
                    {
                        translations.Add(string.Format("[{0}].[{1}]", dr["MEASUREGROUP_NAME"].ToString(), dr["MEASURE_NAME"].ToString()), dr["MEASURE_CAPTION"].ToString());
                    }
                }

                // get dimension names
                resColl.Clear();
                resColl.Add("CUBE_NAME",Context.CurrentCube);
                var dsDims = conn.GetSchemaDataSet("MDSCHEMA_DIMENSIONS", resColl);


                AdomdClient.AdomdCommand cmd = new AdomdClient.AdomdCommand();
                cmd.Connection = conn;
                cmd.CommandText = sDrillthroughMDX;
                DataTable tbl = new DataTable();
                AdomdClient.AdomdDataAdapter adp = new AdomdClient.AdomdDataAdapter(cmd);
                TimeoutUtility.FillAdomdDataAdapter(adp, tbl);

                // loop through the columns looking for duplicate translation names
                Dictionary<string, int> dictColumnNames = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
                foreach (DataColumn col in tbl.Columns)
                {
                    var colKey = col.ColumnName.Substring(0, col.ColumnName.LastIndexOf(']')+1);

                    if (translations.ContainsKey(colKey))
                    {
                        string sNewColumnName = translations[colKey];
                        if (dictColumnNames.ContainsKey(sNewColumnName))
                            dictColumnNames[sNewColumnName]++;
                        else
                            dictColumnNames.Add(sNewColumnName, 1);
                    }
                    else
                    {
                        Context.TraceEvent(999,0, string.Format("The translation for the column '{0}' was not found", col.ColumnName));
                    }
                }


                foreach (DataColumn col in tbl.Columns)
                {
                    var colKey = col.ColumnName.Substring(0, col.ColumnName.LastIndexOf(']')+1);
                    var suffix = col.ColumnName.Substring(col.ColumnName.LastIndexOf("]") + 1);
                    if (translations.ContainsKey(colKey))
                    {
                        string sNewName = translations[colKey];
                        if (dictColumnNames[sNewName] > 1)
                        {
                            
                            //if (string.IsNullOrWhiteSpace( suffix)){
                                //prefix with tablename
                                var m = columnNameRegex.Matches(col.ColumnName);
                                var dimName = m[0].Groups["cube"].Value.TrimStart('$');
                                var caption = dsDims.Tables[0].Select(string.Format("DIMENSION_NAME = '{0}'",dimName))[0]["DIMENSION_CAPTION"].ToString();
                                col.ColumnName = caption + "." + sNewName + suffix;
                            //}
                            //else {
                            //    col.ColumnName = sNewName + suffix;
                            //}
                        }
                        else
                        {
                            col.ColumnName = sNewName;
                        }
                    }
                }
                

               

                //foreach (DataColumn col in tbl.Columns)
                //{
                //    string sNewColumnName = col.ColumnName.Substring(col.ColumnName.LastIndexOf('.') + 1).Replace("[", "").Replace("]", "");
                //    if (dictColumnNames[sNewColumnName] > 1)
                //        sNewColumnName = col.ColumnName.Substring(col.ColumnName.LastIndexOf('[') + 1).Replace("[", "").Replace("]", "").Replace("$", "");
                //    if (!tbl.Columns.Contains(sNewColumnName))
                //        col.ColumnName = sNewColumnName;
                //}

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
                            try
                            {
                                if (m.ParentLevel.ParentHierarchy.ParentDimension.DimensionType == DimensionTypeEnum.Measure)
                                {
                                    sOverrideMeasure = m.UniqueName;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                // DPG 16 Jan 2015
                                // if we get an error trying to figure out if we are looking at the Measures Dimension
                                // we can just log it and continue. As far as I'm aware this only happens when one of the dimensions/attributes
                                // in the tuple is not visible and this should only occur for non-measures dimensions.
                                // so simply logging the exception and moving on to the next member should not do any harm.
                                int eventSubClass = 999;
                                Context.TraceEvent(eventSubClass, 0, string.Format("ERROR GetCustomDrillthroughMDX() Exception: {0}", ex.Message));
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
