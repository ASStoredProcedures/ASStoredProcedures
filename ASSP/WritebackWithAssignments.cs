/*============================================================================
  File:    WritebackWithAssignments.cs

  Summary: Implements string filtering functions for use in MDX Queries.

  Date:    September 20, 2006

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
using Microsoft.AnalysisServices;
using System.Collections;

namespace ASStoredProcs
{
    public class WritebackWithAssignments
    {

        public static void AssignValue(string subCube, MDXValue valueToAssign)
        {
            Server mServer;
            Database mDB;
            Cube mCube;
            MdxScript mMdxScript;
            mServer = new Server();
            try
            {
                mServer.Connect("*");
                mDB = mServer.Databases.GetByName(Context.CurrentDatabaseName);
                mCube = mDB.Cubes.GetByName(Context.CurrentCube.Name);

                mMdxScript = mCube.DefaultMdxScript;

                mMdxScript.Commands.Add(new Command(scriptComment() + subCube + " = " + valueToAssign.ToString() + ";" + System.Environment.NewLine));
                mMdxScript.Update();
            }
            finally
            {
                mServer.Disconnect();
            }
        }

        public static void AssignMDXExpression(string subCube, string expressionToAssign)
        {
            Server mServer;
            Database mDB;
            Cube mCube;
            MdxScript mMdxScript;
            mServer = new Server();
            try
            {
                mServer.Connect("*");
                mDB = mServer.Databases.GetByName(Context.CurrentDatabaseName);
                mCube = mDB.Cubes.GetByName(Context.CurrentCube.Name);

                mMdxScript = mCube.DefaultMdxScript;

                mMdxScript.Commands.Add(new Command(scriptComment() + subCube + " = " + expressionToAssign.ToString() + ";" + System.Environment.NewLine));
                mMdxScript.Update();
            }
            finally
            {
                mServer.Disconnect();
            }
        }

        private static string scriptComment()
        {
            string comment;
            comment = System.Environment.NewLine;
            comment += "--Assignment Made By Stored Procedure ";
            comment += System.Environment.NewLine;
            comment += "--Date: " + DateTime.Now.Date.ToString("F");
            comment += System.Environment.NewLine;
            Expression e = new Expression("UserName");
            comment += "--User: " + e.Calculate(null).ToString();
            comment += System.Environment.NewLine;
            return comment;
        }

    }
}
