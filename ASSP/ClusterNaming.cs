/*============================================================================
  File:    ClusterNaming.cs

  Summary: Looks at a clustering mining model and identifies what distinguishes
           each cluster from the whole population.

  Date:    January 20, 2007

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
using Microsoft.AnalysisServices.AdomdServer;
using System.Data;
using Microsoft.AnalysisServices;

namespace ASStoredProcs
{
    //no statisticians were harmed (or involved, for that matter) in the making of this sproc... as such the results should be considered unscientific
    public class ClusterNaming
    {
        private const double MIN_PROBABILITY = 0.49; //only name clusters with attributes which describe about half or more of the cases in that cluster
        private const double MIN_PERCENT_DIFFERENT_THAN_WHOLE = 0.12; //only name cluster with attributes which occur > 12% more in that cluster than in the whole population
        private static string _cachedSystemDataMiningSprocsPath = "";

        [SafeToPrepare(true)]
        public static DataTable DistinguishingCharacteristicsForClusters(string ModelName)
        {
            return DistinguishingCharacteristicsForClusters(ModelName, true);
        }

        //Return a list of the clusters in this mining model
        //Columns:
        //1. ID - The unique name for the cluster (e.g. 001, 002)
        //2. DistinguishingCharacteristics - The best way to describe
        //    what distinguishes this particular cluster vs. the whole population. 
        //    Only attributes which describe at least half the population of this 
        //    cluster are included in this string. The first characteristic is the 
        //    most distinguishing characteristic.
        //3. FullDescription - The Microsoft generated description representing 
        //    every attribute without regard to whether that attribute distinguishes
        //    this cluster vs. the whole population.
        [SafeToPrepare(true)]
        public static DataTable DistinguishingCharacteristicsForClusters(string ModelName, bool MentionAttributeName)
        {
            Microsoft.AnalysisServices.AdomdServer.MiningModel model = Context.MiningModels[ModelName];
            if (model == null) throw new Exception("Model not found");
            if (model.Content.Count == 0) throw new Exception("Model not processed");
            DataTable tableReturn = new DataTable();
            tableReturn.Columns.Add("ID");
            tableReturn.Columns.Add("DistinguishingCharacteristics");
            tableReturn.Columns.Add("FullDescription");

            if (Context.ExecuteForPrepare) return tableReturn;

            DataTable tableAll = GetClusterCharacteristics(ModelName, "", 0.0005); //get the characteristics of the entire population
            int iMiscNodesCount = 0;
            DataTable[] tables = new DataTable[model.Content[0].Children.Count];
            int[] extraAttributes = new int[model.Content[0].Children.Count];
            Dictionary<string, int> dictDistinguishers = new Dictionary<string, int>();
            for (int i = 0; i< model.Content[0].Children.Count; i++)
            {
                int iExtraAttributesAdded = 0;
                MiningContentNode node = model.Content[0].Children[i];
                object[] row = new object[3];
                row[0] = node.Name;
                DataTable tableNode = (tables[i] ?? GetClusterCharacteristics(ModelName, node.Name, 0.0005));
                tables[i] = tableNode;
                StringBuilder sDistinguishers = new StringBuilder();
                foreach (DataRow dr in tableNode.Rows)
                {
                    if (MIN_PROBABILITY >= Convert.ToDouble(dr["Frequency "]) && iExtraAttributesAdded == extraAttributes[i]) break;

                    //find matching row and continue if this cluster is distinguished by this attribute/value pair
                    foreach (DataRow dr2 in tableAll.Rows)
                    {
                        if (Convert.ToString(dr2["Attributes"]) == Convert.ToString(dr["Attributes"]) && Convert.ToString(dr2["Values"]) == Convert.ToString(dr["Values"])) {
                            if (Convert.ToDouble(dr2["Frequency "]) + MIN_PERCENT_DIFFERENT_THAN_WHOLE < Convert.ToDouble(dr["Frequency "])) //the column name actually ends in a space!
                            {
                                if (sDistinguishers.Length > 0) sDistinguishers.Append("; ");
                                if (MentionAttributeName) sDistinguishers.Append(dr["Attributes"]).Append(" = ");
                                sDistinguishers.Append(dr["Values"]);
                                if (MIN_PROBABILITY >= Convert.ToDouble(dr["Frequency "])) iExtraAttributesAdded++;
                            }
                            break;
                        }
                    }
                }
                Context.CheckCancelled(); //could be a bit long running, so allow user to cancel
                if (sDistinguishers.Length == 0) sDistinguishers.Append("Miscellaneous ").Append(++iMiscNodesCount);
                if (dictDistinguishers.ContainsKey(sDistinguishers.ToString()) && extraAttributes[i] < tableAll.Rows.Count)
                {
                    //a cluster with the exact same description already exists, so add another attribute to it and to the current cluster
                    extraAttributes[dictDistinguishers[sDistinguishers.ToString()]]++;
                    extraAttributes[i]++;
                    tableReturn.Rows.Clear();
                    dictDistinguishers.Clear();
                    i = -1; //reset loop
                }
                else
                {
                    row[1] = sDistinguishers.ToString();
                    row[2] = node.Description;
                    tableReturn.Rows.Add(row);
                }
            }
            return tableReturn;
        }

        [SafeToPrepare(true)]
        public static void AutoNameClusters(string ModelName)
        {
            AutoNameClusters(ModelName, true);
        }

        [SafeToPrepare(true)]
        public static void AutoNameClusters(string ModelName, bool MentionAttributeName)
        {
            if (Context.ExecuteForPrepare) return;
            DataTable dt = DistinguishingCharacteristicsForClusters(ModelName, MentionAttributeName);
            AdomdCommand cmd = new AdomdCommand();
            foreach (DataRow dr in dt.Rows)
            {
                cmd.CommandText = "UPDATE [" + ModelName.Replace("]", "]]") + "].CONTENT SET NODE_CAPTION='" + dr["DistinguishingCharacteristics"].ToString().Replace("'", "''") + "' WHERE NODE_UNIQUE_NAME='" + dr["ID"].ToString() + "'";
                cmd.ExecuteNonQuery();
            }
        }

        [SafeToPrepare(true)]
        private static DataTable GetClusterCharacteristics(string strModel, string strClusterUniqueID, double dThreshold)
        {
            //if we don't know the path to the system data mining sprocs assembly, get it
            if (_cachedSystemDataMiningSprocsPath.Length == 0)
            {
                Microsoft.AnalysisServices.Server svr = new Microsoft.AnalysisServices.Server();
                svr.Connect(Context.CurrentServerID);
                ClrAssembly ass = (ClrAssembly)svr.Assemblies.GetByName("System");
                if (ass == null) throw new Exception("System (data mining sprocs) assembly not found");
                foreach (ClrAssemblyFile file in ass.Files)
                {
                    if (file.Type == ClrAssemblyFileType.Main)
                    {
                        lock (_cachedSystemDataMiningSprocsPath) _cachedSystemDataMiningSprocsPath = file.Name;
                        break;
                    }
                }
                svr.Disconnect();
            }

            //get the DataMining sprocs assembly and call the GetClusterCharacteristics function
            System.Reflection.Assembly asAss = System.Reflection.Assembly.LoadFile(_cachedSystemDataMiningSprocsPath);
            Type t = asAss.GetType("Microsoft.AnalysisServices.System.DataMining.Clustering");
            object oClustering = t.GetConstructor(new Type[] { }).Invoke(new object[] { });
            return (DataTable)t.InvokeMember("GetClusterCharacteristics", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, oClustering, new object[] { strModel, strClusterUniqueID, dThreshold });
       }


    }
}