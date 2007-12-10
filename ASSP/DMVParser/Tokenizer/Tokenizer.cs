using System;
using System.Collections.Generic;
using System.Text;

namespace ASStoredProcs.DMVParser.Tokenizer
{
    internal class Tokenizer
    {
        List<Token> mTokens = new List<Token>();
        public Tokenizer(string statement)
        {

            char currChar;
            ITokenizerState state = new DefaultState();
            StringBuilder token = new StringBuilder();

            char[] chars = statement.ToCharArray();

            for (int i = 0; i < statement.Length; i++)
            {
                currChar = chars[i];
                state = state.Exec(currChar, i, statement, ref token, mTokens);
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
