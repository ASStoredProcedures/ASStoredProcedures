/*============================================================================
  File:    DefaultState.cs

  Summary: The class in this file is part of a state machine that tokenizes
           the DMV select syntax. This class handles whitespace.

  Date:    December 14, 2007

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
using ASStoredProcs.DMVParser.Tokenizer;

namespace ASStoredProcs.DMVParser.Tokenizer
{
    internal class WhiteSpaceState : ITokenizerState
    {

        #region ITokenizerState Members

        public ITokenizerState Exec(char currentChar, int position, string statement, ref StringBuilder token, List<Token> tokens)
        {
            if (char.IsWhiteSpace(currentChar) || char.IsControl(currentChar))
            {
                return this; // stay in Whitespace state
            }
            if (InStringState.IsStringDelimiter(currentChar))
            {
                return new InStringState();
            }
            ITokenizerState defState = new DefaultState();
            defState.Exec(currentChar, position, statement,ref token, tokens);
            return defState;
        }

        #endregion
    }

}
