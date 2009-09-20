/*============================================================================
  File:    DefaultState.cs

  Summary: The class in this file is part of a state machine that tokenizes
           the DMV select syntax. This is the main class of the state machine.

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
using Microsoft.AnalysisServices.AdomdServer;

namespace ASStoredProcs.DMVParser.Tokenizer
{
    internal class Tokenizer
    {
        List<Token> mTokens = new List<Token>();
        internal Tokenizer(string statement)
        {

            char currChar;
            ITokenizerState state = new DefaultState();
            StringBuilder token = new StringBuilder();

            char[] chars = statement.ToCharArray();

            for (int i = 0; i < statement.Length; i++)
            {
                currChar = chars[i];
                state = state.Exec(currChar, i, statement, ref token, mTokens);
                Context.CheckCancelled();
            }
            if (token.Length > 0)
            {
                mTokens.Add(new Token(token.ToString()));
            }
        }

        public List<Token> Tokens
        {
            get
            {
                return mTokens;
            }
        }

    }
}
