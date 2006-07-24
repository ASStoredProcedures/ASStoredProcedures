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
        public static DateTime GetCubeLastProcessedDate() {
            Server oServer = new Server();
            oServer.Connect("Data Source=" + Context.CurrentServerID);
            DateTime dtTemp = oServer.Databases.GetByName(Context.CurrentDatabaseName).Cubes.GetByName(Context.CurrentCube.Name).LastProcessed;
            oServer.Disconnect();
            return dtTemp;
            //return Context.CurrentCube.LastProcessed; //this doesn't work because of a bug: https://connect.microsoft.com/SQLServer/feedback/ViewFeedback.aspx?FeedbackID=124606
        }
    }
}
