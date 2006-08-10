/*============================================================================
  File:    Multiply.cs

  Summary: Implements a function which multiplies the values returned by an expression for all members in a set

  Date:    August 10, 2006

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
    public class Multiplication
    {
        //Multiply() multiplies the value Expression for each member of inputSet
        //This is useful for calculations such as compound growths, and (limited) testing shows that
        //using this function is much faster than implementing the same calculation in pure MDX using recursion
        public static double Multiply(Set inputSet, Expression inputExpression)
        {
            double result = 1;
            foreach (Tuple tuple in inputSet)
                result *= inputExpression.Calculate(tuple).ToDouble();
            return result;
        }
    }
}
