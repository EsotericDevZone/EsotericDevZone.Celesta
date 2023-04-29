using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.RuleBasedParser;
using EsotericDevZone.RuleBasedParser.Presets;

namespace EsotericDevZone.Celesta.Parser
{
    internal static class ParseTreeAtomBuilders
    {
        public static object StringLiteral(string input)
        {
            _ = AtomBuilders.DoubleQuotedString(input) as string;            
            return new StringLiteral(input);
        }

        public static object SymbolLiteral(string input)
        {
            var symbol = AtomBuilders.Symbol(input) as string;
            if (Language.Keywords.Contains(symbol))
                throw new ParseException("Symbol excpected, keyword found");
            return symbol;
        }

        public static object NumberLiteral(string input)
        {
            if (double.TryParse(input, out double _))
                return new NumericLiteral(input);
            throw new ParseException($"Not a number literal: '{input}'");
        }
    }
}
