/*============================================================================
  File:    AMOHelpers.cs

  Summary: Implements various static helper functions

  Date:    February 27, 2010

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

namespace ASStoredProcs
{
    static class AMOHelpers
    {
        // Returns the name of the current cube, for perspectives it will
        // return the name of the underlying base cube.
        internal static string GetCurrentCubeName()
        {
            string sCubeName = "";
            if (Context.CurrentCube != null)
            {
                sCubeName = Context.CurrentCube.Name;
            }
            else
            {
                sCubeName = new Expression("[Measures].CurrentMember.Properties(\"CUBE_NAME\")").Calculate(null).ToString();
            }

            //this code will run if the current cube is a perspective. it will return the name of the base cube in order to work around a bug in walking through the objects in AdomdServer in a perspective
            Property propBaseCubeName = Context.Cubes[sCubeName].Properties.Find("BASE_CUBE_NAME");
            if (propBaseCubeName != null)
                return Convert.ToString(propBaseCubeName.Value);
            else
                return sCubeName;
        }

    }
}
