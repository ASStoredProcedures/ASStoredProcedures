/*============================================================================
  File:    SetOperations.cs

  Summary: 

  Date:    August 12, 2006

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/Wiki/View.aspx?ProjectName=ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/

using Microsoft.AnalysisServices.AdomdServer;

namespace ASStoredProcs
{
    public class SetOperations
    {
        // This function reverses the set
        public Set ReverseSet(Set InputSet)
        {
            SetBuilder sb = new SetBuilder();
            int c = InputSet.Tuples.Count;
            for (int i = c - 1; i >= 0; i--)
            {
                sb.Add(InputSet.Tuples[i]);
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
    }
}
