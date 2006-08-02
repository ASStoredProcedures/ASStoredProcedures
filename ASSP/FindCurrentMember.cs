/*============================================================================
  File:    FindCurrentMember.cs

  Summary: Implements functions to help determine the currentmember on dimensions and hierarchies

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

namespace ASStoredProcs
{
    public class FindCurrentMembers
    {
        //These functions can come in useful when trying to understand how 'strong hierarchies' work:
        //for example, when you're trying to find out how selecting a member on one attribute hierarchy
        //automatically affects the currentmember on other attribute hierarchies on the same dimension

        //FindCurrentMember returns a comma-delimited list of the uniquenames of the currentmember on every attribute hierarchy
        //on every dimension
        public static string FindCurrentMember()
        {
            string output = "";
            foreach (Dimension d in Context.CurrentCube.Dimensions)
            {
                foreach (Hierarchy h in d.AttributeHierarchies)
                {
                    output += h.CurrentMember.UniqueName + ", ";
                }
            }
            return output;
        }

        //FindCurrentMemberVerbose does the same thing as FindCurrentMember, but also displays the name
        //of the dimension and hierarchy and the caption of the currentmember
        public static string FindCurrentMemberVerbose()
        {
            string output = "";
            foreach (Dimension d in Context.CurrentCube.Dimensions)
            {
                foreach (Hierarchy h in d.AttributeHierarchies)
                {
                    output += "Dimension: " + d.Caption + " Hierarchy: " + h.Caption + " CurrentMember Caption: " + h.CurrentMember.Caption + " CurrentMember UniqueName: " + h.CurrentMember.UniqueName + " *** ";
                }
            }
            return output;
        }
    }
}
