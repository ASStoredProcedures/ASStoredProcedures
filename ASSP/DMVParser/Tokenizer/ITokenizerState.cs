using System;
using System.Collections.Generic;
using System.Text;
using ASStoredProcs.DMVParser.Tokenizer;

namespace ASStoredProcs.DMVParser.Tokenizer
{
    internal interface ITokenizerState
    {
        ITokenizerState Exec(char currentChar, int position, string statement, ref StringBuilder token, List<Token> tokens);
    }
}
