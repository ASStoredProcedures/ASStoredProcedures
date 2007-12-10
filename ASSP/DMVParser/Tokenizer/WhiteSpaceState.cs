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
