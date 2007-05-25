/*============================================================================
  File:    DimensionHealthCheck.cs

  Summary: Provides functions which allow you to check that the attribute
           relationships and key/name pairs of your dimension attributes hold
           true according to the current data.

  Date:    May 19, 2007

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
using System.Data;
using AdomdServer = Microsoft.AnalysisServices.AdomdServer;

namespace ASStoredProcs
{
    public class DimensionHealthCheck
    {
        [AdomdServer.SafeToPrepare(true)]
        public static DataTable ListDimensionsWithErrors()
        {
            DataTable tableReturn = new DataTable();
            tableReturn.Columns.Add("Dimension");
            if (AdomdServer.Context.ExecuteForPrepare) return tableReturn;

            Server server = new Server();
            server.Connect("*");
            foreach (Dimension d in server.Databases.GetByName(AdomdServer.Context.CurrentDatabaseName).Dimensions)
            {
                DimensionError[] errors = Check(d);
                if (errors.Length > 0)
                {
                    tableReturn.Rows.Add(new object[] { d.Name });
                }
            }
            server.Disconnect();
            return tableReturn;
        }

        [AdomdServer.SafeToPrepare(true)]
        public static DataTable ListDimensionErrors(string DimensionName)
        {
            DataTable tableReturn = new DataTable();
            tableReturn.Columns.Add("Dimension");
            tableReturn.Columns.Add("ErrorNumber", typeof(int));
            tableReturn.Columns.Add("ErrorDescription");
            tableReturn.Columns.Add("NumColumns", typeof(int));
            int iNumPriorColumns = tableReturn.Columns.Count;
            int iMaxNumColumns = 9;
            for (int i = 1; i <= iMaxNumColumns; i++)
            {
                tableReturn.Columns.Add("Column" + i + "Name");
                tableReturn.Columns.Add("Column" + i);
            }

            if (AdomdServer.Context.ExecuteForPrepare) return tableReturn;

            Server server = new Server();
            server.Connect("*");
            Dimension d = server.Databases.GetByName(AdomdServer.Context.CurrentDatabaseName).Dimensions.GetByName(DimensionName);
            DimensionError[] errors = Check(d);
            server.Disconnect();

            for (int i = 0; i < errors.Length; i++)
            {
                DimensionError error = errors[i];
                if (error.ErrorTable == null || (error.ErrorTable != null && error.ErrorTable.Rows.Count == 0))
                {
                    DataRow row = tableReturn.NewRow();
                    row["Dimension"] = d.Name;
                    row["ErrorNumber"] = i + 1;
                    row["ErrorDescription"] = error.ErrorDescription;
                    row["NumColumns"] = 0;
                    tableReturn.Rows.Add(row);
                }
                else
                {
                    foreach (DataRow errorRow in error.ErrorTable.Rows)
                    {
                        DataRow row = tableReturn.NewRow();
                        row["Dimension"] = d.Name;
                        row["ErrorNumber"] = i + 1;
                        row["ErrorDescription"] = error.ErrorDescription;
                        row["NumColumns"] = error.ErrorTable.Columns.Count;
                        if (iMaxNumColumns < error.ErrorTable.Columns.Count)
                            throw new Exception("Dimension error dataset contained more than " + iMaxNumColumns + " columns which is not allowed with the current code. Change the code of the sproc to allow more, refresh the report dataset, and change the report layout to allow more.");
                        for (int j = 0; j < error.ErrorTable.Columns.Count; j++)
                        {
                            row[j * 2 + iNumPriorColumns] = error.ErrorTable.Columns[j].ColumnName;
                            if (!Convert.IsDBNull(errorRow[j]))
                                row[j * 2 + iNumPriorColumns + 1] = errorRow[j].ToString();
                        }
                        tableReturn.Rows.Add(row);
                    }
                }
            }
            return tableReturn;
        }

        private static DimensionError[] Check(Dimension d)
        {
            if (d.MiningModelID != null) return new DimensionError[] { };
            List<DimensionError> problems = new List<DimensionError>();

            IDbConnection openedDataSourceConnection = GetOpenedDbConnectionFromDataSource(d.DataSource);
            String sql = "";
            bool bGotSQL = false;
            foreach (DimensionAttribute da in d.Attributes)
            {
                try
                {
                    bGotSQL = false;
                    if (da.Usage != AttributeUsage.Parent)
                        sql = GetQueryToValidateKeyUniqueness(da);
                    else
                        sql = null;
                    if (sql != null)
                    {
                        bGotSQL = true;
                        DataSet ds = new DataSet();
                        IDbConnectionFill(openedDataSourceConnection, ds, sql);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string problem = "Attribute [" + da.Name + "] has key values with multiple names.";
                            DimensionError err = new DimensionError();
                            err.ErrorDescription = problem;
                            err.ErrorTable = ds.Tables[0];
                            problems.Add(err);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string problem = "Attempt to validate key and name relationship for attribute [" + da.Name + "] failed:" + ex.Message + ex.StackTrace + (bGotSQL ? "\r\nSQL query was: " + sql : "");
                    DimensionError err = new DimensionError();
                    err.ErrorDescription = problem;
                    problems.Add(err);
                }
            }
            foreach (DimensionAttribute da in d.Attributes)
            {
                foreach (AttributeRelationship r in da.AttributeRelationships)
                {
                    try
                    {
                        bGotSQL = false;
                        if (da.Usage != AttributeUsage.Parent)
                            sql = GetQueryToValidateRelationship(r);
                        else
                            sql = null;
                        if (sql != null)
                        {
                            bGotSQL = true;
                            DataSet ds = new DataSet();
                            IDbConnectionFill(openedDataSourceConnection, ds, sql);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                string problem = "Attribute relationship [" + da.Name + "] -> [" + r.Attribute.Name + "] is not valid because it results in a many-to-many relationship.";
                                DimensionError err = new DimensionError();
                                err.ErrorDescription = problem;
                                err.ErrorTable = ds.Tables[0];
                                problems.Add(err);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string problem = "Attempt to validate attribute relationship [" + da.Name + "] -> [" + r.Attribute.Name + "] failed:" + ex.Message + ex.StackTrace + (bGotSQL ? "\r\nSQL query was: " + sql : "");
                        DimensionError err = new DimensionError();
                        err.ErrorDescription = problem;
                        problems.Add(err);
                    }
                }
            }
            openedDataSourceConnection.Close();
            return problems.ToArray();
        }

        private static System.Data.Common.DbConnection GetOpenedDbConnectionFromDataSource(DataSource ds)
        {
            string providerInvariantName = ds.ManagedProvider;
            if (string.IsNullOrEmpty(providerInvariantName))
            {
                providerInvariantName = "System.Data.OleDb";
            }
            System.Data.Common.DbConnection conn = System.Data.Common.DbProviderFactories.GetFactory(providerInvariantName).CreateConnection();
            string sConnectionString = String.Empty;
            bool bUsesSQLSecurity = false;
            foreach (string part in ds.ConnectionString.Split(';'))
            {
                //we can't retrieve the password, so remove the User ID and make it a trusted connection
                if (!part.ToLower().StartsWith("user id=") && !part.ToLower().StartsWith("uid="))
                {
                    sConnectionString += part + ";";
                    bUsesSQLSecurity = true;
                }
                else
                {
                    sConnectionString += "Trusted_Connection=Yes;";
                }
            }
            conn.ConnectionString = sConnectionString;
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                if (bUsesSQLSecurity)
                    throw new Exception("Could not open connection to data source " + ds.Name + ". Because the password for a SQL security connection cannot be retreived from SSAS, they are not supported. Register this assembly with impersonation settings which have access to connect to the data source using Integrated Security. Exception was: " + ex.Message);
                else
                    throw new Exception("Could not open connection to data source " + ds.Name + ". Register this assembly with impersonation settings which have access to connect to the data source using Integrated Security. Exception was: " + ex.Message);
            }
            return conn;
        }

        private static void IDbConnectionFill(IDbConnection conn, DataSet ds, string sql)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = sql;
            IDataReader reader = null;
            try
            {
                reader = command.ExecuteReader(CommandBehavior.KeyInfo);
                DataTable table = new DataTable("Table");
                table.Load(reader);
                ds.Tables.Add(table);
            }
            finally
            {
                if ((reader != null) && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
        }

        private static bool CompareDataItems(DataItem a, DataItem b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            else if (a != null && b != null)
            {
                if (a.Source.GetType().FullName == "Microsoft.AnalysisServices.ColumnBinding" && b.Source.GetType().FullName == "Microsoft.AnalysisServices.ColumnBinding")
                {
                    ColumnBinding colA = (ColumnBinding)a.Source;
                    ColumnBinding colB = (ColumnBinding)b.Source;
                    if ("[" + colA.TableID + "].[" + colA.ColumnID + "]" == "[" + colB.TableID + "].[" + colB.ColumnID + "]")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static string GetQueryToValidateKeyUniqueness(DimensionAttribute da)
        {
            if (da.KeyColumns.Count == 0 && (da.NameColumn == null || da.ValueColumn == null)) return null; // no need
            if (da.NameColumn == null && da.ValueColumn == null) return null; // no need
            if (da.KeyColumns.Count == 1 && CompareDataItems(da.KeyColumns[0], da.NameColumn) && da.ValueColumn == null) return null; // no need
            if (da.KeyColumns.Count == 1 && CompareDataItems(da.KeyColumns[0], da.ValueColumn) && da.NameColumn == null) return null; // no need

            DataSourceView oDSV = da.Parent.DataSourceView;
            DataItem[] keyCols;
            DataItem[] nameAndValueCols;
            if (da.KeyColumns.Count == 0)
            {
                keyCols = new DataItem[] { da.NameColumn };
                nameAndValueCols = new DataItem[] { da.ValueColumn }; //value column will be there because of previous check
            }
            else if (da.NameColumn == null)
            {
                if (da.KeyColumns.Count > 1)
                {
                    throw new Exception("If no name column is defined, you may not have more than one key column.");
                }
                keyCols = new DataItem[] { da.KeyColumns[0] };

                //use value as "name" column when checking uniqueness
                nameAndValueCols = new DataItem[] { da.ValueColumn }; //value column will be there because of previous check
            }
            else
            {
                keyCols = new DataItem[da.KeyColumns.Count];
                for (int i = 0; i < da.KeyColumns.Count; i++)
                {
                    keyCols[i] = da.KeyColumns[i];
                }
                if (da.ValueColumn == null)
                {
                    nameAndValueCols = new DataItem[] { da.NameColumn };
                }
                else
                {
                    nameAndValueCols = new DataItem[] { da.NameColumn, da.ValueColumn };
                }
            }
            return GetQueryToValidateUniqueness(oDSV, keyCols, nameAndValueCols);
        }

        private static string GetQueryToValidateRelationship(AttributeRelationship r)
        {
            DataSourceView oDSV = r.ParentDimension.DataSourceView;
            DataItem[] cols1 = new DataItem[r.Parent.KeyColumns.Count];
            DataItem[] cols2 = new DataItem[r.Attribute.KeyColumns.Count];
            for (int i = 0; i < r.Parent.KeyColumns.Count; i++)
            {
                cols1[i] = r.Parent.KeyColumns[i];
            }
            for (int i = 0; i < r.Attribute.KeyColumns.Count; i++)
            {
                cols2[i] = r.Attribute.KeyColumns[i];
            }
            return GetQueryToValidateUniqueness(oDSV, cols1, cols2);
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

        private static string GetQueryToValidateUniqueness(DataSourceView dsv, DataItem[] child, DataItem[] parent)
        {
            //need to do GetColumnBindingForDataItem
            StringBuilder select = new StringBuilder();
            StringBuilder outerSelect = new StringBuilder();
            StringBuilder groupBy = new StringBuilder();
            StringBuilder join = new StringBuilder();
            StringBuilder topLevelColumns = new StringBuilder();
            Dictionary<DataTable, JoinedTable> tables = new Dictionary<DataTable, JoinedTable>();
            List<string> previousColumns = new List<string>();
            foreach (DataItem di in child)
            {
                ColumnBinding col = GetColumnBindingForDataItem(di);
                if (!previousColumns.Contains("[" + col.TableID + "].[" + col.ColumnID + "]"))
                {
                    string colAlias = System.Guid.NewGuid().ToString();
                    DataColumn dc = dsv.Schema.Tables[col.TableID].Columns[col.ColumnID];
                    groupBy.Append((select.Length == 0 ? "group by " : ","));
                    outerSelect.Append((select.Length == 0 ? "select " : ","));
                    select.Append((select.Length == 0 ? "select distinct " : ","));
                    string sIsNull = "";
                    //select.Append(" /*" + dc.DataType.FullName + "*/ "); //for troubleshooting data types
                    if (dc.DataType == typeof(string))
                    {
                        if (di.NullProcessing == NullProcessing.Preserve)
                            sIsNull = "'__BIDS_HELPER_DIMENSION_HEALTH_CHECK_UNIQUE_STRING__'"; //a unique value that shouldn't ever occur in the real data
                        else
                            sIsNull = "''";
                    }
                    else //numeric
                    {
                        if (di.NullProcessing == NullProcessing.Preserve)
                            sIsNull = "-987654321.123456789"; //a unique value that shouldn't ever occur in the real data
                        else
                            sIsNull = "0";
                    }
                    join.Append((join.Length == 0 ? "on " : "and ")).Append("coalesce(y.[").Append(colAlias).Append("],").Append(sIsNull).Append(") = coalesce(z.[").Append(colAlias).Append("],").Append(sIsNull).AppendLine(")");
                    groupBy.Append("[").Append(colAlias).AppendLine("]");
                    outerSelect.Append("[").Append(colAlias).AppendLine("]");
                    if (topLevelColumns.Length > 0) topLevelColumns.Append(",");
                    topLevelColumns.Append("y.[").Append(colAlias).Append("] as [").Append(dc.ColumnName).AppendLine("]");
                    if (!dc.ExtendedProperties.ContainsKey("ComputedColumnExpression"))
                    {
                        select.Append("[").Append(colAlias).Append("] = [").Append(dsv.Schema.Tables[col.TableID].ExtendedProperties["FriendlyName"].ToString()).Append("].[").Append((dc.ExtendedProperties["DbColumnName"] ?? dc.ColumnName).ToString()).AppendLine("]");
                    }
                    else
                    {
                        select.Append("[").Append(colAlias).Append("]");
                        select.Append(" = ").AppendLine(dc.ExtendedProperties["ComputedColumnExpression"].ToString());
                    }

                    if (!tables.ContainsKey(dsv.Schema.Tables[col.TableID]))
                    {
                        tables.Add(dsv.Schema.Tables[col.TableID], new JoinedTable(dsv.Schema.Tables[col.TableID]));
                    }
                    previousColumns.Add("[" + col.TableID + "].[" + col.ColumnID + "]");
                }
            }

            foreach (DataItem di in parent)
            {
                ColumnBinding col = GetColumnBindingForDataItem(di);
                if (!previousColumns.Contains("[" + col.TableID + "].[" + col.ColumnID + "]"))
                {
                    string colAlias = System.Guid.NewGuid().ToString();
                    DataColumn dc = dsv.Schema.Tables[col.TableID].Columns[col.ColumnID];
                    select.Append(",");
                    //use the __PARENT__ prefix in case there's a column with the same name but different table
                    if (!dc.ExtendedProperties.ContainsKey("ComputedColumnExpression"))
                    {
                        select.Append("[").Append(colAlias).Append("] = [").Append(dsv.Schema.Tables[col.TableID].ExtendedProperties["FriendlyName"].ToString()).Append("].[").Append((dc.ExtendedProperties["DbColumnName"] ?? dc.ColumnName).ToString()).AppendLine("]");
                    }
                    else
                    {
                        select.Append("[").Append(colAlias).Append("]");
                        select.Append(" = ").AppendLine(dc.ExtendedProperties["ComputedColumnExpression"].ToString());
                    }
                    if (topLevelColumns.Length > 0) topLevelColumns.Append(",");
                    topLevelColumns.Append("y.[").Append(colAlias).Append("] as [").Append(dc.ColumnName).AppendLine("]");

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

            outerSelect.AppendLine("\r\nfrom (").Append(select).AppendLine(") x").Append(groupBy).AppendLine("\r\nhaving count(*)>1");

            string invalidValuesInner = outerSelect.ToString();
            outerSelect = new StringBuilder();
            outerSelect.Append("select ").AppendLine(topLevelColumns.ToString()).AppendLine(" from (").Append(select).AppendLine(") as y");
            outerSelect.AppendLine("join (").AppendLine(invalidValuesInner).AppendLine(") z").AppendLine(join.ToString());
            return outerSelect.ToString();
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

        class DimensionError
        {
            public string ErrorDescription;
            public DataTable ErrorTable;
        }
    }
}