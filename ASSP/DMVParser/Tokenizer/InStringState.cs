/*============================================================================
  File:    DefaultState.cs

  Summary: The class in this file is part of a state machine that tokenizes
           the DMV select syntax. It is responsible for handling strings.

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

namespace ASStoredProcs.DMVParser.Tokenizer
{
    internal class InStringState : ITokenizerState
    {
        const char singleQuote = '\'';
        const char doubleQuote = '"';

        public static bool IsStringDelimiter(char c)
        {
            if (c == singleQuote || c == doubleQuote)
            { return true; }
            return false;
        }

        #region ITokenizerState Members

        public ITokenizerState Exec(char currentChar, int position, string statement, ref StringBuilder token, List<Token> tokens)
        {
            if (InStringState.IsStringDelimiter(currentChar))
            {
                tokens.Add(new Token(token.ToString(),TokenType.String));
                token = new StringBuilder();
                return new DefaultState();
            }
            token.Append(currentChar);
            return this; // stay in string state
        }

        #endregion
    }

}
