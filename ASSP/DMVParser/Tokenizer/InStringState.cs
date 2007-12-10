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
