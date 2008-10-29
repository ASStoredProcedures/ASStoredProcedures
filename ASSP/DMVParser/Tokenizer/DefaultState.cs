/*============================================================================
  File:    DefaultState.cs

  Summary: The class in this file is part of a state machine that tokenizes
           the DMV select syntax.

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
    internal class DefaultState : ITokenizerState
    {

        #region ITokenizerState Members

        public ITokenizerState Exec(char currentChar, int position, string statement, ref StringBuilder token, List<Token> tokens)
        {
            if (char.IsWhiteSpace(currentChar))
            {
                if (token.Length > 0)
                {
                    tokens.Add(new Token(token.ToString()));
                    token = new StringBuilder();
                }
                return new WhiteSpaceState();
            }
            if (InStringState.IsStringDelimiter(currentChar))
            {
                return new InStringState();
            }
            if (InCommentState.IsCommentStart(currentChar,position>0?statement[position-1]:'\x0000'))
            {
                return new InCommentState(statement.Substring(position-1,2));
            }
            switch (currentChar)
            {
                case '(':
                case ')':
                case '[':
                case ']':
                case '=': // an equals sign that is not inside string quotes should trigger a new token
                case ',': // add commas to their own token
       
                    if (token.Length > 0)
                    {
                        tokens.Add(new Token(token.ToString()));
                        token = new StringBuilder();
                    }
                    tokens.Add(new Token(currentChar.ToString(),TokenType.Comma));
                    break;
                default:
                    //else add the current char to the string builder
                    token.Append(currentChar);
                    break;
            }
            return this; // stay in default state
        }

        #endregion
    }

}
