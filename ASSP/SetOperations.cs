/*============================================================================
  File:    SetOperations.cs

  Summary: Various functions which transform sets

  Date:    August 12, 2006

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/

using Microsoft.AnalysisServices.AdomdServer;
using System.Collections.Generic;

namespace ASStoredProcs
{
    public class SetOperations
    {
        // This function reverses the set
        public Set ReverseSet(Set InputSet)
        {
            SetBuilder sb = new SetBuilder();
            List<Tuple> l = new List<Tuple>();
            int c = 0;
            foreach (Tuple t in InputSet.Tuples) //use enumerator, not indexes as specified at http://sqljunkies.com/WebLog/mosha/archive/2007/04/19/stored_procs_best_practices.aspx
            {
                l.Add(t);
                c++;
                Context.CheckCancelled();
            }
            for (int i = c - 1; i >= 0; i--)
            {
                sb.Add(l[i]);
            }

            return sb.ToSet();
        }

        public Set InverseHierarchility(Set InputSet)
        {
            int c = InputSet.Hierarchies.Count;
            if (c < 2)
                return InputSet;

            SetBuilder sb = new SetBuilder();
            foreach (Tuple t in InputSet.Tuples)
            {
                TupleBuilder tb = new TupleBuilder();
                for (int i = c-1; i >= 0; i--)
                {
                    tb.Add(t.Members[i]);
                }
                sb.Add(tb.ToTuple());
            }

            return sb.ToSet();
        }

        public Set RandomSample(Set InputSet, int k)
        {
            int n = InputSet.Tuples.Count;
            if (k >= n)
                return InputSet;

            System.Random r = new System.Random();

            SetBuilder sb = new SetBuilder();
            int i = n;
            foreach (Tuple t in InputSet.Tuples)
            {
                int rnd = r.Next(i);
                if (rnd < k)
                {
                    k--;
                    sb.Add(t);
                }
                i--;
            }
            return sb.ToSet();
        }

        public Set AsymmetricDrillDown(Set InputSet, int TupleIndex, int HierarchyIndex)
        {
            int cHier = InputSet.Hierarchies.Count;
            if (HierarchyIndex >= cHier)
                throw new System.ArgumentOutOfRangeException("Specified hierarchy index is out of range");

            SetBuilder sb = new SetBuilder();
            int i = 0;
            foreach (Tuple t in InputSet.Tuples)
            {
                sb.Add(t);
                if (i == TupleIndex)
                {
                    Member m = t.Members[HierarchyIndex];
                    foreach (Member child in m.GetChildren())
                    {
                        TupleBuilder tb = new TupleBuilder();
                        for (int iHier = 0; iHier < cHier; iHier++)
                        {
                            if (iHier != HierarchyIndex)
                                tb.Add(t.Members[iHier]);
                            else
                                tb.Add(child);
                        }
                        sb.Add(tb.ToTuple());
                    }
                }
                i++;
            }

            return sb.ToSet();
        }


        public static Set Order(Set InputSet, Expression SortExpression)
        {
            return Order(InputSet, SortExpression, false);
        }

        public static Set Order(Set InputSet, Expression SortExpression, bool SortDescending)
        {
            List<TupleValue> TupleValues = new List<TupleValue>();

            Context.TraceEvent(100, 0, "Order: Start getting data");

            int i = 0;
            foreach (Tuple t in InputSet.Tuples)
            {
                TupleValues.Add(new TupleValue(t, (double)SortExpression.Calculate(t)));
                i++;
                //no need to call Context.CheckCancelled() because Calculate already checks it
            }

            int cTuples = i;

            Context.TraceEvent(100, cTuples, "Order: Finish getting data for " + cTuples.ToString() + " tuples");

            Context.TraceEvent(200, cTuples, "Order: Start sorting");
            TupleValues.Sort();
            Context.TraceEvent(200, cTuples, "Order: Finish sorting");

            SetBuilder sb = new SetBuilder();

            if (SortDescending)
            {
                for (i = 0; i < cTuples; i++)
                {
                    sb.Add(TupleValues[i].Tuple);
                }
            }
            else
            {
                for (i = cTuples - 1; i >= 0; i--)
                {
                    sb.Add(TupleValues[i].Tuple);
                }
            }

            return sb.ToSet();
        }

        #region Internal Sorting Classes
        private class TupleValue : System.IComparable<TupleValue>
        {
            private Tuple _Tuple;
            internal Tuple Tuple
            {
                get { return _Tuple; }
                private set { _Tuple = value; }
            }
            private double _Value;

            public TupleValue(Tuple t, double v)
            {
                _Tuple = t;
                _Value = v;
            }

            public int CompareTo(TupleValue t)
            {
                Context.CheckCancelled();
                return t._Value.CompareTo(_Value);
            }
        }
        #endregion
    }
}
