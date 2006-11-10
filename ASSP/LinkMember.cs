/*============================================================================
  File:    LinkMember.cs

  Summary: Implements modified versions of the standard MDX LinkMember
           function.

  Date:    November 10, 2006

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
    public class LinkMember
    {
        //works similar to the LinkMember function except that it only searches in the hierarchy specified by the second parameter and in the level which matches the level of m
        //this function could not be named LinkMember because of conflicts with the standard MDX LinkMember function
        //using a string as the second parameter instead of a Hierarchy object avoids a 50x decrease in performance
        public static Member HierarchyLinkMember(Member m, string sHierarchyUniqueName)
        {
            Expression ex = new Expression();
            ex.ExpressionText = sHierarchyUniqueName + "." + GetLastDotSegmentFromUniqueName(m.LevelName) + "." + GetLastDotSegmentFromUniqueName(m.UniqueName);
            return ex.CalculateMdxObject(null).ToMember();
        }

        //works similar to the LinkMember function except that it only searches in the level which is passed in as the second parameter
        //this function could not be named LinkMember because of conflicts with the standard MDX LinkMember function
        //using a string as the second parameter instead of a Level object avoids a 50x decrease in performance
        public static Member LevelLinkMember(Member m, string sLevelUniqueName)
        {
            Expression ex = new Expression();
            ex.ExpressionText = sLevelUniqueName + "." + GetLastDotSegmentFromUniqueName(m.UniqueName);
            return ex.CalculateMdxObject(null).ToMember();
        }

        #region Examples of passing in Level and Hierarchy objects instead of strings
        //works just like LinkMember except that it only searches in the hierarchy in the level which matches the level of m
        //this function could not be named LinkMember because of conflicts with the standard MDX LinkMember function
        //is at least 50x slower than LinkMember because of the use of the Hierarchy object
        //public static Member SlowLinkMember(Member m, Hierarchy hierarchy)
        //{
        //    Expression ex = new Expression();
        //    ex.ExpressionText = hierarchy.UniqueName + "." + GetLastDotSegmentFromUniqueName(m.LevelName) + "." + GetLastDotSegmentFromUniqueName(m.UniqueName);
        //    return ex.CalculateMdxObject(null).ToMember();
        //}

        //works just like LinkMember except that it only searches in the level which is passed in as the second parameter
        //this function could not be named LinkMember because of conflicts with the standard MDX LinkMember function
        //is very slow because of the use of the Level object
        //public static Member SlowLinkMember(Member m, Level level)
        //{
        //    Expression ex = new Expression();
        //    ex.ExpressionText = level.UniqueName + "." + GetLastDotSegmentFromUniqueName(m.UniqueName);
        //    return ex.CalculateMdxObject(null).ToMember();
        //}
        #endregion

        #region Internal utility functions
        //an internal utility function used to parse a unique name string
        private static string GetLastDotSegmentFromUniqueName(string sUniqueName)
        {
            bool bInBracket = false;
            for (int i = sUniqueName.Length - 1; i >= 0; i--)
            {
                if (sUniqueName.Substring(i, 1) == "]")
                {
                    bInBracket = true;
                }
                else if (sUniqueName.Substring(i, 1) == "[")
                {
                    if (i >= 1 && sUniqueName.Substring(i - 1, 2) == "[[")
                    {
                        i--;
                    }
                    else
                    {
                        bInBracket = false;
                    }
                }
                else if (!bInBracket && sUniqueName.Substring(i, 1) == ".")
                {
                    return sUniqueName.Substring(i + 1);
                }
            }
            throw new Exception("Cannot parse unique name in GetLastDotSegmentFromUniqueName");
        }
        #endregion
    }
}
