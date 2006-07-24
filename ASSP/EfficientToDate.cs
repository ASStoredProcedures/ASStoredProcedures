/*============================================================================
  File:    EfficientToDate.cs

  Summary: Returns a set of members from the Time dimension which represent all
           of time through the currently selected time member. The returned set
           is as efficient as possible in that it takes advantage of
           aggregations.

  Date:    July 24, 2006

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
    public class EfficientToDate
    {
        //GetEfficientToDateSet:
        //Returns a set of members from the Time dimension which represent all of time through the currently selected time member
        //The returned set is as efficient as possible in that it takes advantage of aggregations
        //Example: If the currently selected time member is [Date].[Fiscal].[May 2006]
        // an inefficient way to represent all of time through May 2006 (assuming 2005 is the beginning of time) would be:
        //  {[Date].[Fiscal].[January 2005]:[Date].[Fiscal].[May 2006]}
        // A more efficient way would be:
        //  {[Date].[Fiscal].[2005],[Date].[Fiscal].[Q1 2006],[Date].[Fiscal].[April 2006],[Date].[Fiscal].[May 2006]}

        //Finds the first dimension of type Time then runs the GetEfficientToDateSet(string) function
        public static Set GetEfficientToDateSet()
        {
            return GetEfficientToDateSet(GetTimeDimensionName());
        }

        //For the specified dimension, gets the most efficient to date set as described above
        public static Set GetEfficientToDateSet(string sDimensionName)
        {
            Member oCurrentMember = GetMostGranularHierarchyCurrentMember(sDimensionName);
            return InternalGetEfficientToDateSet(oCurrentMember, 0);
        }

        //For the specified member, gets the most efficient to date set as described above
        public static Set GetEfficientToDateSet(Member oCurrentMember)
        {
            if (oCurrentMember.UniqueName == "")
            { //check if the member exists
                return (new SetBuilder()).ToSet(); //return empty set
            }
            return InternalGetEfficientToDateSet(oCurrentMember, 0);
        }

        //THIS IS THE ONLY FUNCTION WHICH PERFORMS COMPARABLY FAST TO DOING ALL THIS LOGIC IN STRAIGHT MDX
        //For the specified member, gets the most efficient periods to date set as described above
        //YourAssemblyName.GetEfficientPeriodsToDateSet("Calendar Year",[Date].[Calendar].[Month].&[2003]&[10]) returns the most efficient set that represents YTD October 2003
        //YourAssemblyName.GetEfficientPeriodsToDateSet("(All)",[Date].[Calendar].[Month].&[2003]&[10]) returns the most efficient set that represents all of time through October 2003
        public static Set GetEfficientPeriodsToDateSet(string sMinLevelName, Member oCurrentMember)
        {
            if (oCurrentMember.UniqueName == "")
            { //check if the member exists
                return (new SetBuilder()).ToSet(); //return empty set
            }
            return InternalGetEfficientToDateSet(oCurrentMember, sMinLevelName);
        }

        //For the specified member, gets the most efficient year to date set as described above
        public static Set GetEfficientYearToDateSet(Member oCurrentMember)
        {
            if (oCurrentMember.UniqueName == "")
            { //check if the member exists
                return (new SetBuilder()).ToSet(); //return empty set
            }
            return InternalGetEfficientToDateSet(oCurrentMember, GetLevelDepth(oCurrentMember.ParentLevel.ParentHierarchy, LevelTypeEnum.TimeYears));
        }

        //For the specified member, gets the most efficient quarter to date set as described above
        public static Set GetEfficientQuarterToDateSet(Member oCurrentMember)
        {
            if (oCurrentMember.UniqueName == "")
            {  //check if the member exists
                return (new SetBuilder()).ToSet(); //return empty set
            }
            return InternalGetEfficientToDateSet(oCurrentMember, GetLevelDepth(oCurrentMember.ParentLevel.ParentHierarchy, LevelTypeEnum.TimeQuarters));
        }

        //For the specified member, gets the most efficient month to date set as described above
        public static Set GetEfficientMonthToDateSet(Member oCurrentMember)
        {
            if (oCurrentMember.UniqueName == "")
            { //check if the member exists
                return (new SetBuilder()).ToSet(); //return empty set
            }
            return InternalGetEfficientToDateSet(oCurrentMember, GetLevelDepth(oCurrentMember.ParentLevel.ParentHierarchy, LevelTypeEnum.TimeMonths));
        }

        //internal use
        private static Set InternalGetEfficientToDateSet(Member oCurrentMember, int iMinLevelDepth)
        {
            string sMemberUniqueName = oCurrentMember.UniqueName;
            //oCurrentMember.ParentLevel is slow, so avoid using it... do the level lookups in the MDX expression
            string sNewLine = Environment.NewLine;
            string sExpression = "Filter(" + Environment.NewLine
             + " Generate(" + sNewLine
             + "  PeriodsToDate(" + sMemberUniqueName + ".Hierarchy.Levels(" + iMinLevelDepth + "), " + sMemberUniqueName + ") AS MYRANGE" + sNewLine
             + "  ,Ascendants(" + sMemberUniqueName + ".Hierarchy.CurrentMember)" + sNewLine
             + " )" + sNewLine
             + " ,Except(Descendants(" + sMemberUniqueName + ".Hierarchy.CurrentMember," + sMemberUniqueName + ".Level), MYRANGE).Count=0" + sNewLine
             + "  and Count(Except(Descendants(" + sMemberUniqueName + ".Hierarchy.CurrentMember.Parent," + sMemberUniqueName + ".Level), MYRANGE))<>0" + sNewLine
             + ")";
            Expression oExpression = new Expression(sExpression);
            return oExpression.CalculateMdxObject(new TupleBuilder(oCurrentMember).ToTuple()).ToSet();
        }

        //internal use - performs the same as the signature which takes in "iMinLevelDepth As Integer"... however, this version is utilized by GetEfficientPeriodsToDateSet which avoids referencing any Level objects so that performance is comparable to doing all this logic in straight MDX
        private static Set InternalGetEfficientToDateSet(Member oCurrentMember, string sMinLevelName)
        {
            string sMemberUniqueName = oCurrentMember.UniqueName;
            //oCurrentMember.ParentLevel is slow, so avoid using it... do the level lookups in the MDX expression
            string sNewLine = Environment.NewLine;
            string sExpression = "Filter(" + sNewLine
             + " Generate(" + sNewLine
             + "  PeriodsToDate(" + sMemberUniqueName + ".Hierarchy.Levels(\"" + sMinLevelName + "\"), " + sMemberUniqueName + ") AS MYRANGE" + sNewLine
             + "  ,Ascendants(" + sMemberUniqueName + ".Hierarchy.CurrentMember)" + sNewLine
             + " )" + sNewLine
             + " ,Except(Descendants(" + sMemberUniqueName + ".Hierarchy.CurrentMember," + sMemberUniqueName + ".Level), MYRANGE).Count=0" + sNewLine
             + "  and Count(Except(Descendants(" + sMemberUniqueName + ".Hierarchy.CurrentMember.Parent," + sMemberUniqueName + ".Level), MYRANGE))<>0" + sNewLine
             + ")";
            Expression oExpression = new Expression(sExpression);
            return oExpression.CalculateMdxObject(new TupleBuilder(oCurrentMember).ToTuple()).ToSet();
        }

        //this is a much slower version that does nearly the same thing... code left here for educational purposes
        //private static Set InternalGetEfficientToDateSet(Member oCurrentMember, int iMinLevelDepth) {
        //    if (oCurrentMember.UniqueName == "") {  //check if the member exists
        //        return (new SetBuilder()).ToSet(); //return empty set
        //    }
        //    SetBuilder oSetBuilder = new SetBuilder();
        //    if (iMinLevelDepth == 0 || oCurrentMember.LevelDepth >= iMinLevelDepth) {
        //        oSetBuilder.Add((new TupleBuilder(oCurrentMember)).ToTuple());
        //    }
        //    while (oCurrentMember.LevelDepth > iMinLevelDepth) { //loop until the current member is the All level
        //        //loop through siblings in reverse order until we find the current member
        //        //then add the rest of the siblings to the set
        //        //then change current member to the parent
        //        bool bPassedMember = false;
        //        MemberCollection oChildren = oCurrentMember.Parent.GetChildren();
        //        for (int i = oCurrentMember.Parent.ChildCount - 1; i>=0; i--) {
        //            if (bPassedMember) {
        //                oSetBuilder.Add((new TupleBuilder(oChildren[i])).ToTuple());
        //            } else if (oChildren[i].UniqueName == oCurrentMember.UniqueName) {
        //                bPassedMember = true;
        //            }
        //        }
        //        oCurrentMember = oCurrentMember.Parent;
        //    }
        //    return oSetBuilder.ToSet();
        //}

        //For the specified dimension, it returns the member from a user-defined or parent-child hierarchy which is most granular
        public static Member GetMostGranularHierarchyCurrentMember(String sDimensionName)
        {
            Member oMemberToReturn = null;
            int iMaxLevelDepth = 0;
            int iMaxLevelMembersCount = 0;
            Dimension oDimension = Context.CurrentCube.Dimensions[sDimensionName];
            foreach (Hierarchy oHierarchy in oDimension.Hierarchies)
            {
                if (oHierarchy.HierarchyOrigin != HierarchyOrigin.AttributeHierarchy)
                {
                    if (iMaxLevelMembersCount <= oHierarchy.CurrentMember.ParentLevel.GetMembers().Count)
                    {
                        iMaxLevelMembersCount = oHierarchy.CurrentMember.ParentLevel.GetMembers().Count;
                        if (iMaxLevelDepth < oHierarchy.CurrentMember.LevelDepth)
                        {
                            iMaxLevelDepth = oHierarchy.CurrentMember.LevelDepth;
                            oMemberToReturn = oHierarchy.CurrentMember;
                        }
                        else if (oMemberToReturn == null)
                        {
                            oMemberToReturn = oHierarchy.CurrentMember;
                        }
                    }
                }
            }
            if (oMemberToReturn == null)
            {
                throw new Exception("Could not find any user-defined or parent-child hierarchies in dimension " + sDimensionName);
            }
            return oMemberToReturn;
        }

        //Finds the first dimension of type Time then returns its name
        private static String GetTimeDimensionName()
        {
            foreach (Dimension oDimension in Context.CurrentCube.Dimensions)
            {
                if (oDimension.DimensionType == DimensionTypeEnum.Time)
                {
                    return oDimension.Name;
                }
            }
            throw new Exception("Could not find a dimension of type Time");
        }

        private static int GetLevelDepth(Hierarchy oHierarchy, LevelTypeEnum LevelType)
        {
            foreach (Level oLevel in oHierarchy.Levels)
            {
                if (oLevel.LevelType == LevelType)
                {
                    return oLevel.LevelNumber;
                }
            }
            throw new Exception("Could not find a level of type " + LevelType.ToString() + ". If you are looking for this level in a Fiscal hierarchy, the code will need to change to use AMO so it can see all the types of levels, not just the level types that AdomdServer exposes.");
        }
    }
}
