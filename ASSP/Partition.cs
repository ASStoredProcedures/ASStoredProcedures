/*============================================================================
  File:    Partition.cs

  Summary: Provides the CreatePartitions utility function enables you to
           quickly prototype various partitioning schemes.

  Date:    March 02, 2007

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
using AdomdServer = Microsoft.AnalysisServices.AdomdServer;
using AdomdClient = Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AnalysisServices;
using System.Data;

namespace ASStoredProcs
{
    public class PartitionCreator
    {
        [AdomdServer.SafeToPrepare(true)]
        public static void CreatePartitions(string CubeName, string MeasureGroupName, string SetString)
        {
            CreatePartitions(CubeName, MeasureGroupName, SetString, null);
        }

        [AdomdServer.SafeToPrepare(true)]
        public static void CreatePartitions(string CubeName, string MeasureGroupName, string SetString, string PartitionGrouperExpressionString)
        {
            if (AdomdServer.Context.ExecuteForPrepare) return;

            AdomdClient.Set s = null;
            AdomdClient.AdomdConnection conn = new AdomdClient.AdomdConnection("Data Source=" + AdomdServer.Context.CurrentServerID + ";Initial Catalog=" + AdomdServer.Context.CurrentDatabaseName);
            conn.Open();
            Server server = new Server();
            server.Connect("*"); //connect to the current session... important to connect this way or else you will get a deadlock when you go to save the partition changes
            
            AdomdServer.Context.CheckCancelled(); //could be a bit long running, so allow user to cancel
            
            try
            {
                AdomdServer.Context.TraceEvent(0, 0, "Retrieving Template Partition");
                Database db = server.Databases.GetByName(AdomdServer.Context.CurrentDatabaseName);
                Cube cube = db.Cubes.GetByName(CubeName);
                MeasureGroup mg = cube.MeasureGroups.GetByName(MeasureGroupName);
                Partition template = mg.Partitions[0];

                AdomdServer.Context.TraceEvent(0, 0, "Resolving Set"); //will show up under the User Defined trace event which is selected by default
                AdomdClient.CellSet cs;
                AdomdClient.AdomdCommand cmd = new AdomdClient.AdomdCommand();
                cmd.Connection = conn;
                if (String.IsNullOrEmpty(PartitionGrouperExpressionString))
                {
                    cmd.CommandText = "select {} on 0, {" + SetString + "} on 1 "
                     + "from [" + CubeName + "]";
                    try
                    {
                        cs = cmd.ExecuteCellSet();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("The set specified was not valid: " + ex.Message);
                    }
                }
                else
                {
                    cmd.CommandText = "with member [Measures].[_ASSP_PartitionGrouper_] as " + PartitionGrouperExpressionString + " "
                     + "select [Measures].[_ASSP_PartitionGrouper_] on 0, "
                     + "{" + SetString + "} on 1 "
                     + "from [" + CubeName + "]";
                    try
                    {
                        cs = cmd.ExecuteCellSet();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("The set or partition grouper specified was not valid: " + ex.Message);
                    }
                }
                s = cs.Axes[1].Set;

                AdomdServer.Context.TraceEvent(0, 0, "Determining Partition Scheme");
                Dictionary<string, PartitionMetadata> dictPartitions = new Dictionary<string, PartitionMetadata>();
                List<string> listPartitionNames = new List<string>(dictPartitions.Count);
                for (int iTuple = 0; iTuple < s.Tuples.Count; iTuple++)
                {
                    AdomdServer.Context.CheckCancelled(); //could be a bit long running, so allow user to cancel

                    AdomdClient.Tuple t = s.Tuples[iTuple];
                    string tostring = t.ToString();
                    string sTupleUniqueName = GetTupleUniqueName(t);
                    string sTupleName = GetTupleName(t);
                    string sGrouper = sTupleUniqueName;

                    if (!String.IsNullOrEmpty(PartitionGrouperExpressionString))
                    {
                        //if a partition grouper has been specified, then group by it
                        sTupleName = sGrouper = cs.Cells[0, iTuple].Value.ToString();
                    }
                    if (!dictPartitions.ContainsKey(sGrouper))
                    {
                        string sPartitionName = mg.Name + " - " + sTupleName;
                        if (String.IsNullOrEmpty(PartitionGrouperExpressionString))
                        {
                            //make sure partition name is unique
                            int i = 1;
                            while (listPartitionNames.Contains(sPartitionName))
                            {
                                sPartitionName = mg.Name + " - " + sTupleName + " " + (++i);
                            }
                        }
                        dictPartitions.Add(sGrouper, new PartitionMetadata(sPartitionName, t));
                        listPartitionNames.Add(sPartitionName);
                    }
                    else
                    {
                        dictPartitions[sGrouper].tuples.Add(t);
                    }
                }

                //remove all existing partitions except template
                for (int iPartition = mg.Partitions.Count - 1; iPartition > 0; iPartition--)
                {
                    mg.Partitions.RemoveAt(iPartition);
                }

                bool bNeedToDeleteTemplate = true;
                foreach (PartitionMetadata pm in dictPartitions.Values)
                {
                    AdomdServer.Context.CheckCancelled(); //could be a bit long running, so allow user to cancel

                    AdomdServer.Context.TraceEvent(0, 0, "Building Partition: " + pm.PartitionName);
                    if (template.ID == pm.PartitionName)
                    {
                        pm.Partition = template;
                        bNeedToDeleteTemplate = false;
                        pm.Partition.Process(ProcessType.ProcessClear); //unprocess it
                    }
                    else
                    {
                        pm.Partition = template.Clone();
                    }
                    pm.Partition.Slice = pm.PartitionSlice;
                    pm.Partition.Name = pm.PartitionName;
                    pm.Partition.ID = pm.PartitionName;
                    if (template.ID != pm.PartitionName) mg.Partitions.Add(pm.Partition);

                    //if we're only building one partition, it must be the All member
                    if (s.Tuples.Count == 1) pm.TupleMustBeOnlyAllMembers = true;

                    string sQuery = "";
                    sQuery = pm.OldQueryDefinition;
                    string sWhereClause = pm.NewPartitionWhereClause;
                    sQuery += "\r\n" + sWhereClause;
                    pm.Partition.Source = new QueryBinding(pm.Partition.DataSource.ID, sQuery);
                }
                if (bNeedToDeleteTemplate) mg.Partitions.Remove(template);

                AdomdServer.Context.TraceEvent(0, 0, "Saving changes");
                mg.Update(UpdateOptions.ExpandFull);
                AdomdServer.Context.TraceEvent(0, 0, "Done creating partitions");
            }
            finally
            {
                try
                {
                    conn.Close();
                }
                catch { }

                try
                {
                    server.Disconnect();
                }
                catch { }
            }
        }


        private static char[] invalidChars = new char[] { '.', ',', ';', '\'', '`', ':', '/', '\\', '*', '|', '?', '"', '&', '%', '$', '!', '+', '=', '(', ')', '[', ']', '{', '}', '<', '>' };
        private static string GetTupleName(AdomdClient.Tuple t)
        {
            StringBuilder sName = new StringBuilder();
            foreach (AdomdClient.Member m in t.Members)
            {
                sName.Append((sName.Length == 0 ? "" : " - ")).Append(m.Caption);
            }
            foreach (char c in invalidChars)
            {
                sName.Replace(c,' ');
            }
            return sName.ToString();
        }

        private static string GetTupleUniqueName(AdomdClient.Tuple t)
        {
            string sUniqueName = "";
            foreach (AdomdClient.Member m in t.Members)
            {
                if (m.UniqueName.ToUpper().EndsWith(".UNKNOWNMEMBER"))
                {
                    sUniqueName += (sUniqueName.Length == 0 ? "" : ", ") + m.UniqueName.Substring(0, m.UniqueName.Length - "UNKNOWNMEMBER".Length) + "[" + m.Caption + "]";
                }
                else
                {
                    sUniqueName += (sUniqueName.Length == 0 ? "" : ", ") + m.UniqueName;
                }
            }
            return "(" + sUniqueName + ")";
        }


        private class PartitionMetadata
        {
            public PartitionMetadata(string partitionName, AdomdClient.Tuple t)
            {
                this.PartitionName = partitionName;
                this.tuples.Add(t);
            }

            public List<AdomdClient.Tuple> tuples = new List<AdomdClient.Tuple>();
            public string PartitionName;
            public bool TupleMustBeOnlyAllMembers = false;

            public string PartitionSlice
            {
                get
                {
                    string sSet = "";
                    foreach (AdomdClient.Tuple t in tuples)
                    {
                        sSet += (sSet.Length == 0 ? "" : ", ") + GetTupleUniqueName(t);
                    }
                    return "{ " + sSet + " }";
                }
            }

            public string NewPartitionWhereClause
            {
                get
                {
                    if (Partition == null) throw new Exception("Partition is not set");
                    string sWhere = "";
                    foreach (AdomdClient.Tuple t in tuples)
                    {
                        string sWhereForTuple = "";
                        bool bHasNonAllMember = false;
                        foreach (AdomdClient.Member m in t.Members)
                        {
                            string sWhereForMember = "";

                            //check if this member is the all member
                            if (m.UniqueName != m.ParentLevel.ParentHierarchy.Properties["ALL_MEMBER"].Value.ToString())
                            {
                                m.FetchAllProperties();
                                AdomdClient.Dimension clientDimension = m.ParentLevel.ParentHierarchy.ParentDimension;
                                CubeDimension cd = Partition.ParentCube.Dimensions.GetByName(clientDimension.Name);
                                Dimension d = cd.Dimension;
                                if (m.ParentLevel.ParentHierarchy.HierarchyOrigin == AdomdClient.HierarchyOrigin.UserHierarchy)
                                {
                                    Hierarchy h = d.Hierarchies.GetByName(m.ParentLevel.ParentHierarchy.Name);
                                    if (!IsHierarchyNatural(h))
                                    {
                                        throw new Exception("The " + h.Name + " hierarchy is not natural (i.e. there are not attribute relationships defined from every child to its parent). Use the attribute hierarchy instead.");
                                    }
                                }
                                else if (m.ParentLevel.ParentHierarchy.HierarchyOrigin == AdomdClient.HierarchyOrigin.ParentChildHierarchy)
                                {
                                    throw new Exception("Parent-child hierarchy not supported");
                                }

                                DimensionAttribute a = d.Attributes.GetByName(m.ParentLevel.Properties["LEVEL_ATTRIBUTE_HIERARCHY_NAME"].Value.ToString());
                                bHasNonAllMember = true;

                                //Note: can't think of a scenario when we would need to check a.IsAggregatable

                                List<DataItem> columnsNeeded = new List<DataItem>();
                                object[] memberKeyValues = GetMemberKeys(m, a.KeyColumns.Count);
                                for (int iKey = 0; iKey < a.KeyColumns.Count; iKey++)
                                {
                                    DataItem key = a.KeyColumns[iKey];
                                    columnsNeeded.Add(key);
                                    if (sWhereForMember.Length != 0) sWhereForMember += " AND ";
                                    sWhereForMember += "[" + GetColumnBindingForDataItem(key).ColumnID + "]";

                                    if (memberKeyValues[iKey] == null)
                                    {
                                        sWhereForMember += " is null";
                                    }
                                    else
                                    {
                                        sWhereForMember += " = '" + memberKeyValues[iKey].ToString().Replace("'", "''") + "'";
                                    }
                                }

                                MeasureGroupAttribute granularity = GetGranularityAttribute(Partition.Parent.Dimensions[cd.ID]);
                                if (granularity.KeyColumns.Count != 1)
                                {
                                    throw new Exception("The granularity attribute " + granularity.CubeAttribute.Attribute.Name + " has a composite key which isn't supported.");
                                }
                                else if (granularity.Attribute.ID != a.ID || m.UniqueName.ToUpper().EndsWith(".UNKNOWNMEMBER"))
                                {
                                    //this branch handles two scenarios...
                                    //1. the slice isn't being made at the granularity attribute, so we'll need an IN clause
                                    //2. or this member is the Unknown member, so we'll need a NOT IN clause

                                    string sDimensionQuery = "";
                                    //check whether this is a snowflaked dimension
                                    if (GetColumnBindingForDataItem(a.KeyColumns[0]).TableID != GetColumnBindingForDataItem(granularity.Attribute.KeyColumns[0]).TableID)
                                    {
                                        ColumnBinding[] child = new ColumnBinding[] { (ColumnBinding)granularity.Attribute.KeyColumns[0].Source };
                                        
                                        ColumnBinding[] parent = new ColumnBinding[columnsNeeded.Count];
                                        for (int i = 0; i < columnsNeeded.Count; i++)
                                        {
                                            parent[i] = (ColumnBinding)columnsNeeded[i].Source;
                                        }

                                        sDimensionQuery = GetSnowflakeQuery(d.DataSourceView, child, parent);
                                    }
                                    else //not snowflaked
                                    {
                                        columnsNeeded.Add(granularity.Attribute.KeyColumns[0]);
                                        sDimensionQuery = GetQueryDefinition(d.Parent, granularity.Attribute, granularity.Attribute.KeyColumns[0].Source, columnsNeeded);
                                    }

                                    if (sWhereForTuple.Length > 0) sWhereForTuple += " AND ";
                                    sWhereForTuple += "[" + GetColumnBindingForDataItem(granularity.KeyColumns[0]).ColumnID + "] ";
                                    if (m.UniqueName.ToUpper().EndsWith(".UNKNOWNMEMBER"))
                                    {
                                        sWhereForTuple += "NOT IN (\r\n";
                                        sWhereForTuple += "select [" + GetColumnBindingForDataItem(granularity.Attribute.KeyColumns[0]).ColumnID + "] from (\r\n";
                                        sWhereForTuple += sDimensionQuery;
                                        sWhereForTuple += ") z\r\n)\r\n";
                                    }
                                    else
                                    {
                                        sWhereForTuple += "IN (\r\n";
                                        sWhereForTuple += "select [" + GetColumnBindingForDataItem(granularity.Attribute.KeyColumns[0]).ColumnID + "] from (\r\n";
                                        sWhereForTuple += sDimensionQuery;
                                        sWhereForTuple += ") z \r\nWHERE (" + sWhereForMember + ")\r\n)\r\n";
                                    }
                                }
                                else
                                {
                                    //slice is at granularity
                                    if (sWhereForTuple.Length > 0) sWhereForTuple += " AND ";
                                    sWhereForTuple += sWhereForMember;
                                }
                            } //end if ensuring it was not the all member
                        } //end of looping through members

                        if (!bHasNonAllMember && !TupleMustBeOnlyAllMembers)
                        {
                            throw new Exception("The following tuple was entirely composed of All members: " + GetTupleUniqueName(t) + ". This is only allowed if there is only one tuple in the set.");
                        }
                        else if (bHasNonAllMember && TupleMustBeOnlyAllMembers)
                        {
                            //if the one member happens to be the only member in a non-aggregatable attribute, then you can just as easily partition by another attribute at the All level
                            throw new Exception("The following tuple was NOT entirely composed of All members: " + GetTupleUniqueName(t) + ". This is required if there is only one tuple in the set.");
                        }

                        if (sWhereForTuple.Length > 0)
                        {
                            if (sWhere.Length > 0) sWhere += "\r\nOR ";
                            sWhere += "(" + sWhereForTuple + ")";
                        }
                    } //end of looping through tuples
                    return (sWhere.Length > 0 ? "\r\nWHERE " + sWhere : "");
                }
            }

            private static bool IsHierarchyNatural(Hierarchy h)
            {
                int count = h.Levels.Count;
                if (count >= 2)
                {
                    DimensionAttribute to = h.Levels[0].SourceAttribute;
                    DimensionAttribute from = null;
                    for (int i = 1; i < count; i++)
                    {
                        from = h.Levels[i].SourceAttribute;
                        if ((to == null) || !HasRelationship(from, to, new System.Collections.Hashtable()))
                        {
                            return false;
                        }
                        to = from;
                    }
                }
                return true;
            }

            private static bool HasRelationship(DimensionAttribute from, DimensionAttribute to, System.Collections.Hashtable visitedAttributes)
            {
                if (from != null)
                {
                    if (from == to)
                    {
                        return true;
                    }
                    if (visitedAttributes.Contains(from))
                    {
                        return false;
                    }
                    visitedAttributes.Add(from, null);
                    foreach (AttributeRelationship relationship in from.AttributeRelationships)
                    {
                        if (HasRelationship(relationship.Attribute, to, visitedAttributes))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            
            private static MeasureGroupAttribute GetGranularityAttribute(MeasureGroupDimension mgdim)
            {
                if (mgdim is RegularMeasureGroupDimension)
                {
                    foreach (MeasureGroupAttribute a in ((RegularMeasureGroupDimension)mgdim).Attributes)
                    {
                        if (a.Type == MeasureGroupAttributeType.Granularity)
                            return a;
                    }
                    throw new Exception("Granularity attribute not found in measure group dimension " + mgdim.CubeDimension.Name);
                }
                else
                {
                    throw new Exception("Only measure group dimensions which are regular are supported. " + mgdim.CubeDimension.Name + " is type " + mgdim.GetType().Name);
                }
            }

            private static ColumnBinding GetColumnBindingForDataItem(DataItem di)
            {
                if (di.Source is ColumnBinding)
                {
                    return (ColumnBinding)di.Source;
                }
                else
                {
                    throw new Exception("Binding for column was unexpected type.");
                }
            }

            private static string GetTableIdForDataItem(DataItem di)
            {
                if (di.Source is ColumnBinding)
                {
                    return ((ColumnBinding)di.Source).TableID;
                }
                else if (di.Source is RowBinding)
                {
                    return ((RowBinding)di.Source).TableID;
                }
                else
                {
                    throw new Exception("Binding for column was unexpected type.");
                }
            }

            private Partition _Partition;
            public Partition Partition
            {
                get { return _Partition; }
                set { _Partition = value; }
            }

            public string OldQueryDefinition
            {
                get
                {
                    if (Partition == null) throw new Exception("Partition is not set");
                    Partition p = this.Partition;
                    if (p.Parent.Measures.Count == 0) throw new Exception(p.Parent.Name + " has no measures.");
                    return GetQueryDefinition(p.ParentDatabase, p.Parent, new DsvTableBinding(p.ParentCube.DataSourceView.ID, GetTableIdForDataItem(p.Parent.Measures[0].Source)), null);
                }
            }

            private static string GetQueryDefinition(Database d, NamedComponent nc, Binding b, List<DataItem> columnsNeeded)
            {
                StringBuilder sQuery = new StringBuilder();
                if (b is DsvTableBinding)
                {
                    DsvTableBinding oDsvTableBinding = (DsvTableBinding)b;
                    DataSourceView oDSV = d.DataSourceViews[oDsvTableBinding.DataSourceViewID];
                    DataTable oTable = oDSV.Schema.Tables[oDsvTableBinding.TableID];
                    if (!oTable.ExtendedProperties.ContainsKey("QueryDefinition") && oTable.ExtendedProperties.ContainsKey("DbTableName"))
                    {
                        foreach (DataColumn oColumn in oTable.Columns)
                        {
                            bool bFoundColumn = false;
                            if (columnsNeeded == null)
                            {
                                bFoundColumn = true;
                            }
                            else
                            {
                                foreach (DataItem di in columnsNeeded)
                                {
                                    if (GetColumnBindingForDataItem(di).TableID == oTable.TableName && GetColumnBindingForDataItem(di).ColumnID == oColumn.ColumnName)
                                    {
                                        bFoundColumn = true;
                                    }
                                }
                            }
                            if (bFoundColumn)
                            {
                                if (sQuery.Length == 0)
                                {
                                    sQuery.Append("select ");
                                }
                                else
                                {
                                    sQuery.Append(",");
                                }
                                if (!oColumn.ExtendedProperties.ContainsKey("ComputedColumnExpression"))
                                {
                                    sQuery.Append("[").Append(oColumn.ExtendedProperties["DbColumnName"].ToString()).AppendLine("]");
                                }
                                else
                                {
                                    sQuery.Append("[").Append(oColumn.ExtendedProperties["DbColumnName"].ToString()).Append("] = ").AppendLine(oColumn.ExtendedProperties["ComputedColumnExpression"].ToString());
                                }
                            }
                        }
                        if (sQuery.Length == 0)
                        {
                            throw new Exception("There was a problem constructing the query.");
                        }
                        sQuery.Append("from [").Append(oTable.ExtendedProperties["DbTableName"].ToString()).Append("] as [").Append(oTable.ExtendedProperties["FriendlyName"].ToString()).AppendLine("]");
                    }
                    else if (oTable.ExtendedProperties.ContainsKey("QueryDefinition"))
                    {
                        sQuery.AppendLine("select *");
                        sQuery.AppendLine("from (");
                        sQuery.AppendLine(oTable.ExtendedProperties["QueryDefinition"].ToString());
                        sQuery.AppendLine(") x");
                    }
                    else
                    {
                        throw new Exception("Current the code does not support this type of query.");
                    }
                }
                else if (b is QueryBinding)
                {
                    QueryBinding oQueryBinding = (QueryBinding)b;
                    sQuery.Append(oQueryBinding.QueryDefinition);
                }
                else if (b is ColumnBinding)
                {
                    ColumnBinding cb = (ColumnBinding)b;
                    object parent = cb.Parent;
                    DataTable dt = d.DataSourceViews[0].Schema.Tables[cb.TableID];
                    if (nc is DimensionAttribute)
                    {
                        DimensionAttribute da = (DimensionAttribute)nc;

                        if (da.Parent.KeyAttribute.KeyColumns.Count != 1)
                        {
                            throw new Exception("Attribute " + da.Parent.KeyAttribute.Name + " has a composite key. This is not supported for a key attribute of a dimension.");
                        }

                        string sDsvID = ((DimensionAttribute)nc).Parent.DataSourceView.ID;
                        columnsNeeded.Add(new DataItem(cb.Clone()));
                        columnsNeeded.Add(da.Parent.KeyAttribute.KeyColumns[0]);
                        return GetQueryDefinition(d, nc, new DsvTableBinding(sDsvID, cb.TableID), columnsNeeded);
                    }
                    else
                    {
                        throw new Exception("GetQueryDefinition does not currently support a ColumnBinding on a object of type " + nc.GetType().Name);
                    }
                }
                else
                {
                    throw new Exception("Not a supported query binding type");
                }

                return sQuery.ToString();
            }

            //as a workaround to a bug: https://connect.microsoft.com/feedback/viewfeedback.aspx?FeedbackID=260636&siteid=68&wa=wsignin1.0
            private static object[] GetMemberKeys(AdomdClient.Member m, int iNumKeys)
            {
                AdomdClient.AdomdConnection conn = new AdomdClient.AdomdConnection("Data Source=" + AdomdServer.Context.CurrentServerID + ";Initial Catalog=" + AdomdServer.Context.CurrentDatabaseName);
                conn.Open();
                try
                {
                    StringBuilder sCmd = new StringBuilder();
                    sCmd.AppendLine("with");
                    for (int i = 0; i < iNumKeys; i++)
                    {
                        sCmd.AppendLine("member [Measures].[_ASSP_MemberKey" + i + "] as " + m.ParentLevel.ParentHierarchy.UniqueName + ".CurrentMember.Properties('Key" + i + "')");
                    }
                    sCmd.Append("select {");
                    for (int i = 0; i < iNumKeys; i++)
                    {
                        if (i > 0) sCmd.Append(", ");
                        sCmd.Append("[Measures].[_ASSP_MemberKey" + i + "]");
                    }
                    sCmd.AppendLine("} on 0,");
                    sCmd.Append(m.UniqueName).AppendLine(" on 1");
                    sCmd.Append("from [").Append(m.ParentLevel.ParentHierarchy.ParentDimension.ParentCube.Name).AppendLine("]");
                    AdomdClient.AdomdCommand cmd = new AdomdClient.AdomdCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sCmd.ToString();
                    AdomdClient.CellSet cs = cmd.ExecuteCellSet();
                    List<object> keys = new List<object>(cs.Cells.Count);
                    foreach (AdomdClient.Cell c in cs.Cells)
                    {
                        keys.Add(c.CellProperties["FORMATTED_VALUE"].Value); //the FORMATTED_VALUE will be null if it should be null
                    }
                    return keys.ToArray();
                }
                finally
                {
                    try
                    {
                        conn.Close();
                    }
                    catch { }
                }
            }

            #region Snowflaked Dimension Handling
            private static string GetSnowflakeQuery(DataSourceView dsv, ColumnBinding[] child, ColumnBinding[] parent)
            {
                StringBuilder select = new StringBuilder();
                Dictionary<DataTable, JoinedTable> tables = new Dictionary<DataTable, JoinedTable>();
                List<string> previousColumns = new List<string>();
                foreach (ColumnBinding col in child)
                {
                    if (!previousColumns.Contains("[" + col.TableID + "].[" + col.ColumnID + "]"))
                    {
                        DataColumn dc = dsv.Schema.Tables[col.TableID].Columns[col.ColumnID];
                        select.Append((select.Length == 0 ? "select " : ","));
                        if (!dc.ExtendedProperties.ContainsKey("ComputedColumnExpression"))
                        {
                            select.Append("[").Append(dsv.Schema.Tables[col.TableID].ExtendedProperties["FriendlyName"].ToString()).Append("].[").Append((dc.ExtendedProperties["DbColumnName"] ?? dc.ColumnName).ToString()).AppendLine("]");
                        }
                        else
                        {
                            select.AppendLine(dc.ExtendedProperties["ComputedColumnExpression"].ToString());
                        }

                        if (!tables.ContainsKey(dsv.Schema.Tables[col.TableID]))
                        {
                            tables.Add(dsv.Schema.Tables[col.TableID], new JoinedTable(dsv.Schema.Tables[col.TableID]));
                        }
                        previousColumns.Add("[" + col.TableID + "].[" + col.ColumnID + "]");
                    }
                }

                foreach (ColumnBinding col in parent)
                {
                    if (!previousColumns.Contains("[" + col.TableID + "].[" + col.ColumnID + "]"))
                    {
                        DataColumn dc = dsv.Schema.Tables[col.TableID].Columns[col.ColumnID];
                        select.Append(",");
                        if (!dc.ExtendedProperties.ContainsKey("ComputedColumnExpression"))
                        {
                            select.Append("[").Append(dsv.Schema.Tables[col.TableID].ExtendedProperties["FriendlyName"].ToString()).Append("].[").Append((dc.ExtendedProperties["DbColumnName"] ?? dc.ColumnName).ToString()).AppendLine("]");
                        }
                        else
                        {
                            select.AppendLine(dc.ExtendedProperties["ComputedColumnExpression"].ToString());
                        }

                        if (!tables.ContainsKey(dsv.Schema.Tables[col.TableID]))
                        {
                            tables.Add(dsv.Schema.Tables[col.TableID], new JoinedTable(dsv.Schema.Tables[col.TableID]));
                        }
                        previousColumns.Add("[" + col.TableID + "].[" + col.ColumnID + "]");
                    }
                }

                int iLastTableCount = 0;
                while (iLastTableCount != tables.Values.Count)
                {
                    iLastTableCount = tables.Values.Count;
                    JoinedTable[] arrJt = new JoinedTable[iLastTableCount];
                    tables.Values.CopyTo(arrJt, 0); //because you can't iterate the dictionary keys while they are changing
                    foreach (JoinedTable jt in arrJt)
                    {
                        TraverseParentRelationshipsAndAddNewTables(tables, jt.table);
                    }
                }

                //check that all but one table have a valid join path to them
                DataTable baseTable = null;
                foreach (JoinedTable t in tables.Values)
                {
                    if (!t.Joined)
                    {
                        if (baseTable == null)
                        {
                            baseTable = t.table;
                        }
                        else
                        {
                            throw new Exception("Cannot find join path for table " + t.table.TableName + " or " + baseTable.TableName + ". Only one table can be the starting table for the joins.");
                        }
                    }
                }

                //by now, all tables needed for joins will be in the dictionary
                select.Append("\r\nfrom ").AppendLine(GetFromClauseForTable(baseTable));
                select.Append(TraverseParentRelationshipsAndGetFromClause(tables, baseTable));

                return select.ToString();
            }

            private static string GetFromClauseForTable(DataTable oTable)
            {
                if (!oTable.ExtendedProperties.ContainsKey("QueryDefinition") && oTable.ExtendedProperties.ContainsKey("DbTableName") && oTable.ExtendedProperties.ContainsKey("DbSchemaName"))
                {
                    return "[" + oTable.ExtendedProperties["DbSchemaName"].ToString() + "].[" + oTable.ExtendedProperties["DbTableName"].ToString() + "] as [" + oTable.ExtendedProperties["FriendlyName"].ToString() + "]";
                }
                else if (oTable.ExtendedProperties.ContainsKey("QueryDefinition"))
                {
                    return "(\r\n " + oTable.ExtendedProperties["QueryDefinition"].ToString() + "\r\n) as [" + oTable.ExtendedProperties["FriendlyName"].ToString() + "]\r\n";
                }
                else
                {
                    throw new Exception("Unexpected query definition for table binding.");
                }
            }

            //traverse all the parent relationships looking for connections to other tables we need
            //if a connection is found, then be sure to unwind and add all the missing intermediate tables along the way
            private static bool TraverseParentRelationshipsAndAddNewTables(Dictionary<DataTable, JoinedTable> tables, DataTable t)
            {
                bool bReturn = false;
                foreach (DataRelation r in t.ParentRelations)
                {
                    if (r.ParentTable != r.ChildTable)
                    {
                        if (!tables.ContainsKey(r.ParentTable) && TraverseParentRelationshipsAndAddNewTables(tables, r.ParentTable))
                        {
                            tables.Add(r.ParentTable, new JoinedTable(r.ParentTable));
                            tables[r.ParentTable].Joined = true;
                            bReturn = true;
                        }
                        else if (tables.ContainsKey(r.ParentTable))
                        {
                            tables[r.ParentTable].Joined = true;
                            bReturn = true;
                        }
                    }
                }
                return bReturn;
            }

            private static string TraverseParentRelationshipsAndGetFromClause(Dictionary<DataTable, JoinedTable> tables, DataTable t)
            {
                StringBuilder joins = new StringBuilder();
                foreach (DataRelation r in t.ParentRelations)
                {
                    if (r.ParentTable != r.ChildTable && tables.ContainsKey(r.ParentTable) && !tables[r.ParentTable].AddedToQuery)
                    {
                        joins.Append("join ").AppendLine(GetFromClauseForTable(r.ParentTable));
                        for (int i = 0; i < r.ParentColumns.Length; i++)
                        {
                            joins.Append((i == 0 ? " on " : " and "));
                            joins.Append("[").Append(r.ParentTable.ExtendedProperties["FriendlyName"].ToString()).Append("].[").Append(r.ParentColumns[i].ColumnName).Append("]");
                            joins.Append(" = [").Append(r.ChildTable.ExtendedProperties["FriendlyName"].ToString()).Append("].[").Append(r.ChildColumns[i].ColumnName).AppendLine("]");
                        }
                        joins.Append(TraverseParentRelationshipsAndGetFromClause(tables, r.ParentTable));
                    }
                }
                tables[t].AddedToQuery = true;
                return joins.ToString();
            }
                        
            class JoinedTable
            {
                public DataTable table;
                public bool Joined = false;
                public bool AddedToQuery = false;
                public JoinedTable(DataTable t)
                {
                    table = t;
                }
            }
            #endregion


        } //end Partition.PartitionMetadata class

    } //end Partition class
}
