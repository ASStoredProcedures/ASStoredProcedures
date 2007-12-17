/*============================================================================
  File:    DefaultState.cs

  Summary: The class in this file is part of a state machine that tokenizes
           the DMV select syntax. It stores a tokens identified by the tokenizer

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
    internal class Token
    {
        private string mText = "";
        private TokenType mType = TokenType.Unknown;

        public Token(string text)
        {
            mText = text;
        }

        public Token(string text, TokenType type)
        {
            mText = text;
            mType = type;
        }
        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }
        public TokenType Type
        {
            get { return mType; }
            set { mType = value; }
        }
    }

    public enum TokenType
    {
        Unknown = 0,
        Keyword,
        Object,
        String,
        Comma,
        Bracket
    }
}
