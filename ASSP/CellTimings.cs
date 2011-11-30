/*============================================================================
  File:    CellTimings.cs

  Summary: Implements a function which shows the time taken (in ticks) to retrieve each cell in a cellset

  Date:    August 2, 2006

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
using System.Diagnostics;


namespace ASStoredProcs
{
    public class CellTimings
    {
        //TimeToCalculate returns the time taken in ticks to return the value displayed in a cell in a cellset
        //See the example MDX query for how it can be used in a calculated member; you could also use it in
        //an MDX Script assignment. This function could be useful when you have a poorly-performing query and you
        //don't know what is causing the problem: cells which display relatively high values, which take most time
        //to display their values, are likely to be the ones you need to investigate. Note that this function will
        //return slightly different values each time you run it, and you'll also see the effects of the caching that
        //Analysis Services does between queries as well as the caching it does inside each query when retrieving
        //cell values unaffected by any calculation. 
        [SafeToPrepare(true)]
        public static long TimeToCalculate(Tuple tupleToEvaluate)
        {
            Stopwatch stp = new Stopwatch();
            stp.Start();
            MDXValue m = new Expression("[Measures].Currentmember").Calculate(tupleToEvaluate);
            stp.Stop();
            return stp.ElapsedTicks;
        } 
    }
}
