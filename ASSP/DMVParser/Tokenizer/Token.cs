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
