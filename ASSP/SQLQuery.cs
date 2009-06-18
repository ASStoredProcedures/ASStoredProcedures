/*============================================================================
  File:    SQLQuery.cs

  Summary: Implements a function that allows for the execution of arbitrary SQL Queries
           Particularly useful for calling from a rowset action.

  Date:    May 4, 2009

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/Wiki/View.aspx?ProjectName=ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/
using System;
using System.Data;
using System.Data.OleDb;

namespace ASSP
{
    public class SQLQuery
    {
        public static DataTable ExecuteSQL(string connectionString, string sql)
        {
            OleDbConnection conn = new OleDbConnection(connectionString + ";ReadOnly=1");
            
            DataTable dt = new DataTable("Results");
            OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
            da.Fill(dt);
            return dt;
        }
    }
}
