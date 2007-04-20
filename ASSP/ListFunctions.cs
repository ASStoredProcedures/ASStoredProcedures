/*============================================================================
  File:    ListFunctions.cs

  Summary: Lists functions available in assemblies that are located either
           on the server or in a specific database.

  Date:    December 4, 2006

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/Wiki/View.aspx?ProjectName=ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/

using Microsoft.AnalysisServices.AdomdServer;
using System.Data;
using System;
using Microsoft.AnalysisServices;
using System.Reflection;

namespace ASStoredProcs
{
    public class FunctionLister
    {
        [SafeToPrepare(true)]
        public DataTable ListFunctions()
        {
            Microsoft.AnalysisServices.Server svr = new Microsoft.AnalysisServices.Server();
            svr.Connect(Context.CurrentServerID);
            try
            {
                return getFunctionList(svr.Assemblies);
            }
            finally
            {
                svr.Disconnect();
            }
        }

        [SafeToPrepare(true)]
        public DataTable ListFunctions(string databaseName)
        {
            Microsoft.AnalysisServices.Server svr = new Microsoft.AnalysisServices.Server();
            svr.Connect(Context.CurrentServerID);
            Microsoft.AnalysisServices.Database db = svr.Databases.GetByName(databaseName);
            if (db == null)
            {
                throw new Exception(string.Format("Unable to find a database called '{0}'", databaseName));
            }
            else
            {
                try
                {
                    return getFunctionList(db.Assemblies);
                }
                finally
                {
                    svr.Disconnect();
                }
            }
        }


        private DataTable getFunctionList(AssemblyCollection assColl)
        {
            Context.TraceEvent(100, 0, "ListFunctions: Starting");
            // build the structure for the datatable which is returned
            DataTable dtFuncs = new DataTable("dtFunctions");
            dtFuncs.Columns.Add("Assembly", typeof(String));
            dtFuncs.Columns.Add("Class", typeof(String));
            dtFuncs.Columns.Add("Method", typeof(String));
            dtFuncs.Columns.Add("ReturnType", typeof(String));
            dtFuncs.Columns.Add("Parameters", typeof(String));

            if (Context.ExecuteForPrepare)
            {
                // we can exit after building the table function if this is only
                // being executed for a prepare.
                Context.TraceEvent(100, 0, "ListFunctions: Finished (ExecuteForPrepare)");
                return dtFuncs;
            }

            foreach (Microsoft.AnalysisServices.Assembly ass in assColl)
            {
                Context.CheckCancelled();

                if (ass is ClrAssembly) // we can only use reflection against .Net assemblies
                {
                    Type t = ass.GetType();

                    ClrAssembly clrAss = (ClrAssembly)ass;

                    Context.TraceEvent(100, 0, "ListFunctions: Processing the " + clrAss.Name + " Assembly");

                    foreach (ClrAssemblyFile f in clrAss.Files) // an assembly can have multiple files
                    {
                        Context.CheckCancelled();

                        // We only want to get the "main" asembly file and only files which have data
                        // (Some of the system assemblies appear to be registrations only and do not
                        // have any data.
                        if (f.Data.Count > 0 && f.Type == ClrAssemblyFileType.Main) 
                        {
                            // assembly the assembly back into a single byte from the blocks of base64 strings
                            byte[] rawAss = new byte[0];
                            int iPos = 0;
                            byte[] buff = new byte[0];

                            foreach (string block in f.Data)
                            {
                                Context.CheckCancelled();

                                buff = System.Convert.FromBase64String(block);
                                System.Array.Resize(ref rawAss, rawAss.Length + buff.Length);
                                buff.CopyTo(rawAss, iPos);
                                iPos += buff.Length;
                            }

                            // use reflection to extract the public types and methods from the 
                            // re-assembled assembly.
                            Context.TraceEvent(100, 0, "ListFunctions: Starting reflection against " + f.Name);
                            System.Reflection.Assembly asAss = System.Reflection.Assembly.Load(rawAss);
                            Type[] assTypes = asAss.GetTypes();
                            for (int i = 0; i < assTypes.Length; i++)
                            {
                                Type t2 = assTypes[i];
                                if (t2.IsPublic)
                                {
                                    MethodInfo[] methods;
                                    methods = t2.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                                    
                                    int paramCnt = 0;
                                    foreach (MethodInfo meth in methods)
                                    {
                                        Context.CheckCancelled();

                                        // build the parameter signature as a string
                                        ParameterInfo[] Params = meth.GetParameters();
                                        System.Text.StringBuilder paramList = new System.Text.StringBuilder();
                                        paramCnt = Params.Length;
                                        string[] paramArray = new string[paramCnt];
                                        // add the first parameter
                                        if (paramCnt > 0)
                                        {
                                            paramList.Append(Params[0].Name);
                                            paramList.Append(" as ");
                                            paramList.Append(StripNamespace(Params[0].ParameterType.ToString()));
                                        }
                                        // add subsequent parameters, inserting a comma before each new one.
                                        for (int j = 1; j < paramCnt; j++)
                                        {
                                            paramList.Append(", ");
                                            paramList.Append(Params[j].Name);
                                            paramList.Append(" as ");
                                            paramList.Append(StripNamespace(Params[j].ParameterType.ToString()));
                                        }

                                        DataRow rowFunc = dtFuncs.NewRow();
                                        Object[] items = new Object[5];
                                        items[0] = ass.Name;
                                        items[1] = t2.Name;
                                        items[2] = meth.Name;
                                        items[3] = StripNamespace(meth.ReturnType.ToString());
                                        items[4] = paramList.ToString();

                                        rowFunc.ItemArray = items;
                                        dtFuncs.Rows.Add(rowFunc);
                                        rowFunc.AcceptChanges();
                                    } // foreach meth
                                } // if t2.IsPublic
                            } // assTypes.Length
                            Context.TraceEvent(100, 0, "ListFunctions: Finished reflecting against " + f.Name);
                        } // if f.data.count > 0 && f.Type == main

                    } // foreach f
                } // if ass is clrAssembly
            } // foreach ass
            dtFuncs.AcceptChanges();
            Context.TraceEvent(100, dtFuncs.Rows.Count, "ListFunctions: Finished (" + dtFuncs.Rows.Count.ToString() + " function signatures)");
            return dtFuncs;
        }

        // This function strips off any leading namespaces from a type name
        // to make it more concise.
        private string StripNamespace(string typeName)
        {
            int lastDot = typeName.LastIndexOf(".");
            if (lastDot > 0)
                return typeName.Substring(lastDot + 1);
            else
                return typeName;
        }

    } //public class ListFunctions
} //namespace ASStoredProcs
