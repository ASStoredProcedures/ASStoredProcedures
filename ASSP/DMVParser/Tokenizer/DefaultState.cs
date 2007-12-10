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
            
                // add commas to their own token
                case ',':
       
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
