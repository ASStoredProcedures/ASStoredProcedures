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
        [SafeToPrepare(true)]
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

        [SafeToPrepare(true)]
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

        [SafeToPrepare(true)]
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

        [SafeToPrepare(true)]
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

        [SafeToPrepare(true)]
        public static Set Order(Set InputSet, Expression SortExpression)
        {
            return Order(InputSet, SortExpression, false);
        }

        [SafeToPrepare(true)]
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

        [SafeToPrepare(true)]
        public decimal RatioToParent(Set axis, Expression exp)
        {
            Hierarchy h = null;

            // Iterate over all hierarchies in the set
            int cHier = axis.Hierarchies.Count;
            int iHier;
            for (iHier = cHier - 1; iHier >= 0; iHier--)
            {
                h = axis.Hierarchies[iHier];
                // and find the hierarchy where the current member is not yet at the highest possible level
                if (h.CurrentMember.ParentLevel.LevelNumber > 0)
                    break;
            }

            // If there were no such hierarchy found - report ratio of 100%
            if (h == null || iHier < 0)
                return 1;

            // Since current member in this hierarchy is not yet at the highest level, we can safely call .Parent
            TupleBuilder tb = new TupleBuilder(h.CurrentMember.Parent);
            // and divide value at current cell by the value of its parent
            return (decimal)exp.Calculate(null) / (decimal)exp.Calculate(tb.ToTuple());
        }

        // The usage should be exactly same as the native MDX function TopCount.
        // The only difference is the function will return the tied up value if the Nth tuple
        // has the duplicated value
        [SafeToPrepare(true)]
        public static Set TopCountWithTies(Set InputSet, int NCount, Expression SortExpression)
        {
            return TopCountWithTiesInternal(InputSet, NCount, SortExpression, false);
        }

        // The usage should be exactly same as the native MDX function BottomCount.
        // The only difference is the function will return the tied up value if the Nth tuple
        // has the duplicated value
        [SafeToPrepare(true)]
        public static Set BottomCountWithTies(Set InputSet, int NCount, Expression SortExpression)
        {
            return TopCountWithTiesInternal(InputSet, NCount, SortExpression, true);
        }

        [SafeToPrepare(true)]
        private static Set TopCountWithTiesInternal(Set InputSet, int NCount, Expression SortExpression, bool Desc)
        {
            List<TupleValue> TupleValues = new List<TupleValue>();

            PriorityQueue queue = new PriorityQueue();

            foreach (Tuple t in InputSet.Tuples)
            {
                double dblValue = (double)SortExpression.Calculate(t);
                if (!Desc)
                    dblValue = -dblValue;
                TupleValue tv = new TupleValue(t, dblValue);

                bool bInsertTuple = false;
                if (queue.Count < NCount || queue.CountWithoutTies < NCount) //short circuit on simple count... only hit CountWithTies when needed
                {
                    bInsertTuple = true;
                }
                else
                {
                    TupleValue r = (TupleValue)queue.Peek();
                    int cmp = tv.CompareTo(r);
                    if (cmp > 0)
                    {
                        //a new lower value is being added which bumps off the highest value
                        //if there's a tie for the highest value, bump all the ties off
                        while (r.CompareTo((TupleValue)queue.Peek()) == 0)
                            queue.Pop();
                        bInsertTuple = true;
                    }
                    else if (cmp == 0)
                    {
                        bInsertTuple = true;
                    }
                }

                if (bInsertTuple)
                {
                    queue.Push(tv);
                }
            }


            System.Collections.Generic.Stack<TupleValue> stack = new System.Collections.Generic.Stack<TupleValue>(queue.Count);
            while (queue.Count > 0)
            {
                TupleValue tv = (TupleValue)queue.Pop();
                stack.Push(tv);
            }

            SetBuilder sb = new SetBuilder();
            while (stack.Count > 0)
            {
                TupleValue tv = stack.Pop();
                sb.Add(tv.Tuple);
            }

            return sb.ToSet();
        }

        #region Internal Sorting Classes
        private class TupleValue : System.IComparable<TupleValue>, System.IComparable
        {
            private Tuple _Tuple;
            internal Tuple Tuple
            {
                get { return _Tuple; }
                private set { _Tuple = value; }
            }
            private double _Value;
            internal double Value
            {
                get { return _Value; }
                private set { _Value = value; }
            }

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

            public int CompareTo(object t)
            {
                Context.CheckCancelled();
                return ((TupleValue)t)._Value.CompareTo(_Value);
            }
        }
        #endregion
    }
}
