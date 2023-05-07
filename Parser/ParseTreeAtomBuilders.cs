using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.RuleBasedParser;
using EsotericDevZone.RuleBasedParser.Presets;

namespace EsotericDevZone.Celesta.Parser
{
    internal static class ParseTreeAtomBuilders
    {
        public static AtomResult BoolLiteral(string input)
        {
            if (input == "true" || input == "false")
                return AtomResult.Atom(new BoolLiteral(input));
            return AtomResult.Error($"Not a boolean: '{input}'");
        }

        public static AtomResult StringLiteral(string input)
        {
            var atom = AtomBuilders.DoubleQuotedString(input);
            if (atom.Failed)
                return AtomResult.Error($"Not a string: '{input}'");
            return AtomResult.Atom(new StringLiteral(input));
        }

        public static AtomResult SymbolLiteral(string input)
        {
            var atom = AtomBuilders.Symbol(input);
            if (atom.Failed)
                return atom;

            var symbol = atom.Value as string;
            if (Language.Keywords.Contains(symbol))
                return AtomResult.Error($"Symbol expected, keyword found : {input}");
            return AtomResult.Atom(symbol);
        }

        public static AtomResult NumberLiteral(string input)
        {
            var atom = AtomBuilders.Number(input);
            if (atom.Failed)
                return AtomResult.Error($"Not a number literal: '{input}'");

            return AtomResult.Atom(new NumericLiteral(input));            
        }
    }
}
