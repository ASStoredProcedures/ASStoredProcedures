using System;
using System.Collections.Generic;
using System.Text;

namespace ASStoredProcs.DMVParser.Tokenizer
{
    internal class InCommentState : ITokenizerState
    {
        private string mCommentStart = "";
        private char mPrevChar = '\x0000';

        public static bool IsCommentStart(char current, char prev)
        {
            switch (current)
            {
                case '-': 
                    if (prev == '-') return true;
                    break;
                case '/': 
                    if (prev == '/') return true;
                    break;
                case '*': 
                    if (prev =='/') return true;
                    break;
            }
            return false;
        }

        public InCommentState(string commentStart)
        {
            mCommentStart = commentStart;
        }

        #region ITokenizerState Members

        public ITokenizerState Exec(char currentChar, int position, string statement, ref StringBuilder token, List<Token> tokens)
        {
            if (token.Length > 0) token.Remove(0, token.Length);

            switch (mCommentStart)
            {
                case "--":
                case "//":
                    if (currentChar == '\n')
                    {
                        return new DefaultState();
                    }
                    break;
                case "/*":
                    if (mPrevChar == '*' && currentChar == '/')
                    {
                        return new DefaultState();
                    }
                    break;
            }
            mPrevChar = currentChar;
            return this;
        }

        #endregion
    }
}
