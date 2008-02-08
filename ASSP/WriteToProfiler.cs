/*============================================================================
  File:    WriteToProfiler.cs

  Summary: Provides the ability to write custom comments to Profiler from MDX

  Date:    January 28th, 2008

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

namespace ASStoredProcs
{
    public class WriteToProfiler
    {
        public MDXValue WriteComment(int eventSubClass, int numberData, string textData)
        {
            Context.TraceEvent(eventSubClass, numberData, textData);
            return (MDXValue)null;
        }


        public MDXValue WriteComment(Expression expressionToEvaluate
        ,Tuple tupleToEvaluate
        , int eventSubClass
        , int numberData
        , string textData)
        {
            Context.TraceEvent(eventSubClass, numberData, textData);
            MDXValue m = expressionToEvaluate.Calculate(tupleToEvaluate);
            return m ;
        }


        public Set WriteComment(Set inputSet, int eventSubClass, int numberData, string textData)
        {
            Context.TraceEvent(eventSubClass, numberData, textData);
            return inputSet;
        }
    }
}
