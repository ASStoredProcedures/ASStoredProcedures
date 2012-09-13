/*============================================================================
  File:    Percentiles.cs

  Summary: Functions that calculate percentile related values

  Date:    September 13, 2012

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/

using Microsoft.AnalysisServices.AdomdServer;
using System.Data;
using Microsoft.AnalysisServices;
using System.Collections.Generic;


namespace ASStoredProcs
{
    public class PercentileFunctions
    {
        [SafeToPrepare(true)]
        public static double RangePoint(Set inputSet, double percentileValue, string calcMethod)
        {
            int tupleCount = 0;

            foreach (Tuple t in inputSet.Tuples)
            {
                tupleCount++;
            }

            switch (calcMethod.ToUpper())
            {
                case "EXC":
                    {
                        return ((percentileValue) * (tupleCount + 1)) - 1;
                    }
                case "INC":
                    {
                        return (((percentileValue) * (tupleCount - 1)) + 1) - 1;
                    }
                default:
                    {
                        return ((percentileValue) * (tupleCount + 1)) - 1;
                    }
            }
            
        }

        [SafeToPrepare(true)]
        public static double ValueAtPercentile(Set inputSet, Expression sortExpression, double percentileValue, bool sortAscending, string calcMethod)
        {
            //get position where percentile falls
            double Rank = RangePoint(inputSet, percentileValue, calcMethod);
            double RankFloorValue = 0;
            double RankCeilingValue = 0;
          

            //order the set ascending using Codeplex SSAS Stored Procedure Function
            if (sortAscending)
            {
                Set s = SetOperations.Order(inputSet, sortExpression);
                int i = 0;
                foreach(Tuple t in s)
                {
                    if(i == System.Math.Floor(Rank))
                    {
                        RankFloorValue = sortExpression.Calculate(t).ToDouble();
                    }
                    else if (i == System.Math.Ceiling(Rank))
                    {
                        RankCeilingValue = sortExpression.Calculate(t).ToDouble();
                        break;
                    }
                    i++;
                }

                //if the Rank is a whole number
                if ((Rank % 1) == 0)
                {
                    return RankFloorValue;
                }
                //if Rank is a decimal
                else
                {
                    return
                        (RankFloorValue
                            +
                            (Rank % 1 * 
                            (RankCeilingValue
                            - RankFloorValue
                            )));
                           /*(((sortExpression.Calculate(s.Tuples[Convert.ToInt32(Rank) + 1]).ToDouble()
                            - sortExpression.Calculate(s.Tuples[Convert.ToInt32(Rank)]).ToDouble())) * (Rank - Convert.ToInt32(Rank)));*/
                }
            }
            else {
                int i = 0;
                foreach (Tuple t in inputSet)
                {
                    if (i == System.Math.Floor(Rank))
                    {
                        RankFloorValue = sortExpression.Calculate(t).ToDouble();
                    }
                    else if (i == System.Math.Ceiling(Rank))
                    {
                        RankCeilingValue = sortExpression.Calculate(t).ToDouble();
                        break;
                    }
                    i++;
                }

                //if the Rank is a whole number
                if ((Rank % 1) == 0)
                {
                    return  RankFloorValue;;
                }
                //if Rank is a decimal
                else
                {
                    return
                     (RankFloorValue
                            +
                            (Rank % 1 *
                            (RankCeilingValue
                            - RankFloorValue
                            )));
                }
            }
        }

    }
}
