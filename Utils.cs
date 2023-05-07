using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.Core;
using EsotericDevZone.Core.Collections;
using EsotericDevZone.RuleBasedParser;
using EsotericDevZone.RuleBasedParser.Presets;
using System;
using System.CodeDom;
using System.Linq;

namespace EsotericDevZone.Celesta
{
    public static class Utils
    {
        public class SymbolIdParser : RuleBasedParser.Parser
        {
            public SymbolIdParser()
            {
                TokensSplitOptions = new TokensSplitOptions(
                    Lists.Empty<string>(),
                    Lists.Of(@"\@", @"\#")
                );

                RegisterAtom("SYMBOL", AtomBuilders.Symbol);                
                
                ParseRules.RegisterRuleValueBuild("@RESULT", "@IDENTIFIER @SCOPE", (string[] idt, string scope) =>
                {
                    var package = idt.Take(idt.Length - 1).JoinToString("#");
                    var symbol = idt.Last();
                    return (Package: package, SymbolName: symbol, Scope: scope);
                });


                ParseRules.RegisterRule("@RESULT", "@IDENTIFIER", (ParseResult[] idt, Token[] tk) =>
                {                    
                    var symbols = (idt[0].Value as object[]).Select(_ => (string)_).ToArray();                    
                    var package = symbols.Take(symbols.Length - 1).JoinToString("#");
                    var symbol = symbols.Last();
                    return new ParseResult(null, (Package: package, SymbolName: symbol, Scope: ""));
                });                

                ParseRules.RegisterRule("@IDENTIFIER", "SYMBOL ?? # SYMBOL", (results, tokens) =>
                {                    
                    return new ParseResult(null, results.Select(_ => _.Value as string).ToArray());
                });
            
                    

                ParseRules.RegisterRule("@SCOPE", @"\@ SYMBOL ?? \@ SYMBOL", (results, tokens)
                    => new ParseResult(null, results.Select(_ => "@" + _.Value).JoinToString("")));

                RootRuleKey = "@RESULT";                
            }
        }

        public static (string Package, string SymbolName, string ScopeName) DispatchIdentifier(string identifier)
        {
            try
            {
                return new SymbolIdParser().Parse<(string, string, string)>(identifier);
            }
            catch
            {
                return (null, null, null);
            }
        }

        public static Identifier ToIdentifier(this string fullName)
        {
            var id = DispatchIdentifier(fullName);
            if (id.SymbolName == null) return null;
            return new Identifier(id.Package, id.SymbolName);
        }

    }
}