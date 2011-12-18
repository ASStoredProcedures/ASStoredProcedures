/*============================================================================
  File:    TraceEvent.cs

  Summary: Various functions which return trace events to profiler so a tester
           can detect when they are being called

  Date:    December 18, 2010

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/

using Microsoft.AnalysisServices.AdomdServer;
using System;

namespace ASSP
{
    public class TraceEvent
    {
        [SafeToPrepare(true)]
        public static int FireTraceEventAndReturnValue(int value)
        {
            Context.TraceEvent(100, value, "FireTraceEventAndReturnValue called");
            return value;
        }
    }
}
