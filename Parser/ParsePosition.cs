using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Parser
{
    public class ParsePosition
    {
        public List<Token> Tokens { get; }
        public int Index { get; }

        public ParsePosition(List<Token> tokens, int index)
        {
            Tokens = tokens;
            Index = index;
        }

        public ParsePosition Advance(int value) => new ParsePosition(Tokens, Index + 1);

        public static ParsePosition BeginParse(List<Token> tokens) => new ParsePosition(tokens, 0);
        public ParsePosition Clone() => new ParsePosition(Tokens, Index);

        public ParsePosition ConsumeToken(out Token token)
        {
            if (Index >= 0 && Index < Tokens.Count)
            {
                token = Tokens[Index];
                return new ParsePosition(Tokens, Index + 1);
            }
            token = null;
            return Clone();
        }

        public bool IsOutOfRange => Index < 0 || Index >= Tokens.Count;

        public (int Line, int Column) GetLineColumn()
        {
            if (Index < 0) return (1, 1);
            if (Index >= Tokens.Count) return Tokens.Count == 0 ? (1, 1) : (Tokens.Last().Line + 1, Tokens.Last().Column);
            return (Tokens[Index].Line, Tokens[Index].Column);
        }
    }
}
