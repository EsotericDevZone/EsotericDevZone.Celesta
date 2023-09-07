using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Parser;
using EsotericDevZone.Celesta.Parser.Patterns;
using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser
{
    public class CelestaParser : EsotericDevZone.Celesta.Parser.Parser
    {
        public CelestaParser()
        {            

            InitRules();
            AddRules();

            MainRuleKey = nameof(RGlobalStatements);            
        }        

        Func<PatternMatchResult, ParseResult> ProcessParseResultAs<R, T>(Func<R, T> func)
            => r => new ParseResult<T>(func(r.ParseResult.GetValue<R>()));

        Func<PatternMatchResult, ParseResult> ProcessParseResultAs<R, T>(Func<PatternMatchResult, R, T> func)
            => r => new ParseResult<T>(func(r, r.ParseResult.GetValue<R>()));

        Func<PatternMatchResult, ParseResult> RawProcessParseResultAs<R>(Func<R, ParseResult> func)
            => r => func(r.ParseResult.GetValue<R>());

        Func<PatternMatchResult, ParseResult> ProcessParseResultToken<T>(Func<Token, T> func)
            => ProcessParseResultAs<Token, T>(func);
        Func<PatternMatchResult, ParseResult> ProcessParseResultSequence<T>(Func<ParseResult[], T> func)
            => ProcessParseResultAs<ParseResult[], T>(func);

        Func<PatternMatchResult, ParseResult> RawProcessParseResultSequence(Func<ParseResult[], ParseResult> func)
            => RawProcessParseResultAs<ParseResult[]>(func);

        Func<PatternMatchResult, ParseResult> ProcessParseResultSequenceItem<T>(int n, Func<ParseResult, T> func)
            => ProcessParseResultAs<ParseResult[], T>(results => func(results[n]));

        Func<PatternMatchResult, ParseResult> ProcessParseResultChosen<T>(Func<ParseResult, T> func)
            => r =>
            {
                var chosen = r.ParseResult.GetValue<ParseResult>();
                return new ParseResult<T>(func(chosen));
            };        

        ParseRule RSymbol;
        ParseRule RExpressionAtom;
        private ParseRule __RCommaSeparatedExpressions;
        private ParseRule __RSubscript;
        private ParseRule __RCall;
        private ParseRule __RArgumentedModifier;
        private ParseRule __RAccessorComponent;
        ParseRule RAccessor;
        ParseRule RExpressionTerm;
        ParseRule RStringLiteral;
        ParseRule RNumberLiteral;

        ParseRule ROpPostfix;

        ParseRule RExprLayerP0; // RExpressionTerm++, RExpressionTerm--        
        ParseRule RExprLayerP1; // ++LayerP0, --LayerP0, + ..., - ..., not ...
        private ParseRule RExprLayerP2;
        private ParseRule RExprLayerP3;
        private ParseRule RExprLayerP4;
        private ParseRule RExprLayerP5;
        private ParseRule RExprLayerP6;
        private ParseRule RExprLayerP7;
        private ParseRule RExprLayerP8;
        private ParseRule RExprLayerP9;
        private ParseRule RExprLayerP10;
        private ParseRule RExprLayerP11;
        private ParseRule RExpression;
        private ParseRule RStatement;
        private ParseRule __RStatementsList;
        private ParseRule RBlockStatement;
        private ParseRule RGlobalStatements;
        ParseRule ROpPrefix;

        IPattern RuleRefPattern(string ruleKey) => new RuleReference(this, ruleKey);

        Choose ChooseLiterals(params string[] literals) => new Choose(literals.Select(_ => new Literal(_)).ToArray());

        ParseRule GenerateBinaryOperatorRule(string crtLayer, string subLayer, string[] operators, bool leftAssociative = true)
        {
            return new ParseRule(crtLayer, new Sequence
                (
                    RuleRefPattern(subLayer),
                    new ZeroOrMore(new Sequence(ChooseLiterals(operators), RuleRefPattern(subLayer)))
                ),
                ProcessParseResultSequence(results =>
                {
                    var head = results[0].GetValue<IParseTreeNode>();
                    var tail = results[1].GetValue<ParseResult[]>()
                        .Select(_ => _.GetValue<ParseResult[]>()).ToArray();
                    //Debug.WriteLine(tail.JoinToString());
                    var ops = tail.Select(_ => _[0].GetValue<ParseResult>().GetValue<Token>().Text).ToArray();
                    var operands = new IParseTreeNode[] { head }
                        .Concat(tail.Select(_ => _[1].GetValue<IParseTreeNode>())).ToArray();

                    if (operands.Length != ops.Length + 1)
                        throw new InvalidOperationException();

                    if (operands.Length == 1)
                        return operands[0];
                    
                    if(leftAssociative)
                    {
                        var leftSide = operands[0];
                        for(int i=0;i<ops.Length;i++)
                        {
                            var rightSide = operands[i + 1];
                            leftSide = new BinaryOperatorParseTreeNode(ops[i], leftSide, rightSide);
                        }
                        return leftSide;
                    }
                    else
                    {
                        var rightSide = operands.Last();
                        for (int i = ops.Length-1; i >=0; i--)
                        {
                            var leftSide = operands[i];
                            rightSide = new BinaryOperatorParseTreeNode(ops[i], leftSide, rightSide);                            
                        }
                        return rightSide;
                    }


                })
            );
        }

        ParseRule GenerateBinaryLeftAssociativeOperatorRule(string crtLayer, string subLayer, params string[] operators)
            => GenerateBinaryOperatorRule(crtLayer, subLayer, operators, leftAssociative: true);

        ParseRule GenerateBinaryRightAssociativeOperatorRule(string crtLayer, string subLayer, params string[] operators)
            => GenerateBinaryOperatorRule(crtLayer, subLayer, operators, leftAssociative:false);

        ParseRule DelimitedNodesRule(string ruleKey, string itemKey, string delimiterLiteral, string headKey = null, bool strict = false, bool endsWithDelimiter = false,
                                     bool allowEmpty = true)
        {
            List<IPattern> seqPattern = new List<IPattern>();
            seqPattern.Add(RuleRefPattern(headKey ?? itemKey));

            if (strict)
                seqPattern.Add(new ZeroOrMore(new Sequence(new Literal(delimiterLiteral), RuleRefPattern(itemKey))));
            else
                seqPattern.Add(new ZeroOrMore(new Sequence(new ZeroOrMore(new Literal(delimiterLiteral)), RuleRefPattern(itemKey))));

            if (endsWithDelimiter)
            {
                if (strict)
                    seqPattern.Add(new OneOrMore(new Literal(delimiterLiteral)));
                else
                    seqPattern.Add(new ZeroOrMore(new Literal(delimiterLiteral)));
            }

            var choosePatterns = new List<IPattern>() { new Sequence(seqPattern.ToArray()).SetLabel(Labels.List) };
            if (allowEmpty)
                choosePatterns.Add(new Nothing().SetLabel("Nothing"));


            return new ParseRule(ruleKey,
                new Choose(choosePatterns.ToArray()),
                ProcessParseResultChosen(result =>
                {
                    if (result.Label != Labels.List) return new IParseTreeNode[0];
                    var seq = result.GetValue<ParseResult[]>();
                    var head = seq[0].GetValue<IParseTreeNode>();
                    var tail = seq[1].GetValue<ParseResult[]>()
                        .Select(_ => _.GetValue<ParseResult[]>()[1].GetValue<IParseTreeNode>())
                        .ToArray();
                    return new IParseTreeNode[] { head }.Concat(tail).ToArray();
                }));
        }

        string[] Keywords = new string[]
        {
            "package",
            "begin", "end",
            "global"
        };

        void InitRules()
        {
            RSymbol = new ParseRule(nameof(RSymbol),
                new SingleTokenRegex($"^(?!({Keywords.JoinToString("|")})$)[_A-Za-z][_A-Za-z0-9]{{0,32}}$").SetPatternName("symbol name"),
                ProcessParseResultToken(tk => new SymbolParseTreeNode(tk)));

            RExpressionAtom = new ParseRule(nameof(RExpressionAtom), new Choose
                (
                    RuleRefPattern(nameof(RStringLiteral)).SetLabel(Labels.String),
                    RuleRefPattern(nameof(RSymbol)).SetLabel(Labels.Symbol),
                    RuleRefPattern(nameof(RNumberLiteral)).SetLabel(Labels.Number),
                    new Sequence(new Literal("("), RuleRefPattern(nameof(RExpression)), new Literal(")")).SetLabel(Labels.Expression)
                ),
                ProcessParseResultChosen(result =>
                {
                    if (new string[] { Labels.Symbol, Labels.String, Labels.Number }.Contains(result.Label))
                        return result.GetValue<IParseTreeNode>();

                    if (result.Label == Labels.Expression)
                        return result.GetValue<ParseResult[]>()[1].GetValue<IParseTreeNode>();

                    throw new InvalidOperationException();
                }));

            __RCommaSeparatedExpressions = new ParseRule(nameof(__RCommaSeparatedExpressions),
                new Choose
                (
                    new Sequence
                    (
                        RuleRefPattern(nameof(RExpression)),
                        new ZeroOrMore(new Sequence(new Literal(","), RuleRefPattern(nameof(RExpression))))
                    ).SetLabel(Labels.List),
                    new Nothing().SetLabel("Nothing")
                ),
                ProcessParseResultChosen(result =>
                {
                    if (result.Label != Labels.List) return new IParseTreeNode[0];
                    var seq = result.GetValue<ParseResult[]>();
                    var head = seq[0].GetValue<IParseTreeNode>();
                    var tail = seq[1].GetValue<ParseResult[]>()
                        .Select(_ => _.GetValue<ParseResult[]>()[1].GetValue<IParseTreeNode>())
                        .ToArray();
                    return new IParseTreeNode[] { head }.Concat(tail).ToArray();

                }));

            __RSubscript = new ParseRule(nameof(__RSubscript),
                new Sequence(new Literal("["), RuleRefPattern(nameof(__RCommaSeparatedExpressions)), new Literal("]")),
                ProcessParseResultSequence(results => new SubscriptList { Arguments = results[1].GetValue<IParseTreeNode[]>() }));

            __RCall = new ParseRule(nameof(__RCall),
                new Sequence(new Literal("("), RuleRefPattern(nameof(__RCommaSeparatedExpressions)), new Literal(")")),
                ProcessParseResultSequence(results => new CallList { Arguments = results[1].GetValue<IParseTreeNode[]>() }));

            __RArgumentedModifier = new ParseRule(nameof(__RArgumentedModifier),
                new Choose(RuleRefPattern(nameof(__RSubscript)), RuleRefPattern(nameof(__RCall))),
                ProcessParseResultChosen(r => r.Value as ParseTreeNodesList));

            __RAccessorComponent = new ParseRule(nameof(__RAccessorComponent),
                new Sequence
                (
                    RuleRefPattern(nameof(RExpressionAtom)),
                    new ZeroOrMore(RuleRefPattern(nameof(__RArgumentedModifier)))
                ),
                ProcessParseResultSequence(results =>
                {
                    var target = results[0].GetValue<IParseTreeNode>();
                    var lists = results[1].GetValue<ParseResult[]>()
                        .Select(_ => _.GetValue<ParseTreeNodesList>()).ToArray();
                    return new AccessorComponent(target, lists);
                }));

            RAccessor = new ParseRule(nameof(RAccessor), new Sequence
                (
                    new Choose(new Literal("global"), RuleRefPattern(nameof(__RAccessorComponent))),
                    new ZeroOrMore(new Sequence(new Literal("."), RuleRefPattern(nameof(__RAccessorComponent))))
                ),
                RawProcessParseResultSequence(results =>
                {
                    var headValue = results[0].GetValue<ParseResult>().Value;
                    var head = headValue is AccessorComponent
                        ? headValue as AccessorComponent
                        : new AccessorComponent(new SymbolParseTreeNode(headValue as Token), new ParseTreeNodesList[0]);
                    var tail = results[1].GetValue<ParseResult[]>()
                        .Select(_ => _.GetValue<ParseResult[]>()[1])
                        .Select(_ => _.GetValue<AccessorComponent>())
                        .ToArray();
                    var items = new AccessorComponent[] { head }.Concat(tail)
                        .Select(_ => new object[] { _.Target }.Concat(_.ArgumentedLists))
                        .SelectMany(_ => _);

                    object aggregate = items.First();                    
                    foreach (var item in items.Skip(1))
                    {
                        if (!(aggregate is IParseTreeNode target))
                            throw new InvalidOperationException("Aggregated value must be IParseTreeNode");
                        else if (item is SymbolParseTreeNode node)
                            aggregate = new AccessorParseTreeNode(target, node);
                        else if (item is SubscriptList subscript)
                            aggregate = new SubscriptParseTreeNode(target, subscript.Arguments);
                        else if (item is CallList call)
                            aggregate = new CallParseTreeNode(target, call.Arguments);
                        else
                        {
                            return ParseResult.Error($"Failed to create accessor .{item}",
                                item is IParseTreeNode tn ? tn.StartPosition : target.EndPosition);
                        }
                    }
                    
                    return new ParseResult<IParseTreeNode>(aggregate as IParseTreeNode);                                        
                }));

            RExpressionTerm = new ParseRule(nameof(RExpressionTerm), new Choose
                (
                    RuleRefPattern(nameof(RAccessor)).SetLabel(Labels.Accessor),
                    new Sequence(new Literal("("), RuleRefPattern(nameof(RExpression)), new Literal(")")).SetLabel(Labels.Expression)
                ),
                ProcessParseResultChosen(result =>
                {
                    if (new string[] { Labels.Accessor, Labels.String, Labels.Number }.Contains(result.Label))
                        return result.GetValue<IParseTreeNode>();

                    if (result.Label == Labels.Expression)
                        return result.GetValue<ParseResult[]>()[1].GetValue<IParseTreeNode>();

                    throw new InvalidOperationException();
                }));

            RStringLiteral = new ParseRule(nameof(RStringLiteral),
                new SingleTokenRegex($"^\".*\"$").SetPatternName("string literal"),
                ProcessParseResultToken(tk => new StringLiteralParseTreeNode(tk)));

            RNumberLiteral = new ParseRule(nameof(RNumberLiteral),
                new Choose(
                    new SingleTokenRegex(@"^[0-9]+\.[0-9]*$").SetPatternName("decimal number literal").SetLabel(Labels.Decimal),
                    new SingleTokenRegex(@"^0x[0-9A-Fa-f]+$").SetPatternName("hexadecimal number literal").SetLabel(Labels.Integer16),
                    new SingleTokenRegex(@"^[0-9]+$").SetPatternName("integer number literal").SetLabel(Labels.Integer10)
                    ),
                ProcessParseResultChosen<IParseTreeNode>(result =>
                {
                    if (result.Label == Labels.Integer16)
                    {                        
                        return new IntegerLiteralParseTreeNode(result.GetValue<Token>());
                    }
                    if (result.Label == Labels.Integer10)
                    {                        
                        return new IntegerLiteralParseTreeNode(result.GetValue<Token>());
                    }
                    if (result.Label == Labels.Decimal)
                        return new DecimalLiteralParseTreeNode(result.GetValue<Token>());

                    throw new InvalidOperationException();
                }));

            ROpPostfix = new ParseRule(nameof(ROpPostfix),
                new Sequence(RuleRefPattern(nameof(RExpressionTerm)), new Choose(new Literal("++"), new Literal("--"))),
                ProcessParseResultSequence(results =>
                {
                    var term = results[0].GetValue<IParseTreeNode>();
                    var @operator = results[1].GetValue<ParseResult>().GetValue<Token>().Text;
                    return new PostFixOperatorParseTreeNode(term, @operator);
                }));

            RExprLayerP0 = new ParseRule(nameof(RExprLayerP0),
                new Choose(
                    RuleRefPattern(nameof(ROpPostfix)),
                    RuleRefPattern(nameof(RExpressionTerm))),
                ProcessParseResultChosen(result => result.Value));


            ROpPrefix = new ParseRule(nameof(ROpPrefix),
                new Sequence(ChooseLiterals("++", "--", "+", "-", "not"), RuleRefPattern(nameof(RExprLayerP0))),
                ProcessParseResultSequence(results =>
                {
                    var @operator = results[0].GetValue<ParseResult>().GetValue<Token>().Text;
                    var term = results[1].GetValue<IParseTreeNode>();
                    return new PrefixOperatorParseTreeNode(term, @operator);
                }));

            RExprLayerP1 = new ParseRule(nameof(RExprLayerP1),
                new Choose(
                    RuleRefPattern(nameof(ROpPrefix)),
                    RuleRefPattern(nameof(RExprLayerP0))),
                ProcessParseResultChosen(result => result.Value));

            RExprLayerP2 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP2), nameof(RExprLayerP1), "*", "/", "%");
            RExprLayerP3 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP3), nameof(RExprLayerP2), "+", "-");
            RExprLayerP4 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP4), nameof(RExprLayerP3), "lsh", "<<", "rsh", ">>");
            RExprLayerP5 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP5), nameof(RExprLayerP4), "<", "<=", ">", ">=");
            RExprLayerP6 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP6), nameof(RExprLayerP5), "==", "!=");
            RExprLayerP7 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP7), nameof(RExprLayerP6), "&");
            RExprLayerP8 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP8), nameof(RExprLayerP7), "^");
            RExprLayerP9 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP9), nameof(RExprLayerP8), "|");
            RExprLayerP10 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP10), nameof(RExprLayerP9), "&&", "and");
            RExprLayerP11 = GenerateBinaryLeftAssociativeOperatorRule(nameof(RExprLayerP11), nameof(RExprLayerP10), "||", "or");
            RExpression = GenerateBinaryRightAssociativeOperatorRule(nameof(RExpression), nameof(RExprLayerP11), "=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=");

            RStatement = new ParseRule(nameof(RStatement),
                new Choose
                (
                    RuleRefPattern(nameof(RPackageDeclarationStatement)),
                    RuleRefPattern(nameof(RBlockStatement)),
                    RuleRefPattern(nameof(RVariableDeclarationStatement)),
                    RuleRefPattern(nameof(RExpression))                
                ),
                ProcessParseResultChosen(result => result.Value));

            __RStatementsList = DelimitedNodesRule(nameof(__RStatementsList), nameof(RStatement), ";", strict: false, endsWithDelimiter: true);

            RBlockStatement = new ParseRule(nameof(RBlockStatement),
                new Sequence(new Literal("begin"), RuleRefPattern(nameof(__RStatementsList)), new Literal("end")),
                ProcessParseResultSequence(results 
                => new BlockParseTreeNode(results[1].GetValue<IParseTreeNode[]>(), results[0].GetValue<Token>(), results[2].GetValue<Token>())));

            __RGlobalKW = new ParseRule(nameof(__RGlobalKW), new Literal("global"), 
                ProcessParseResultAs<Token, SymbolParseTreeNode>(tk => new SymbolParseTreeNode(tk)));

            __RSymbolOrGlobalKW = new ParseRule(nameof(__RSymbolOrGlobalKW),
                new Choose(RuleRefPattern(nameof(__RGlobalKW)), RuleRefPattern(nameof(RSymbol))),
                ProcessParseResultChosen(r => r.Value));

            __RSimpleIdentifierChain = DelimitedNodesRule(nameof(__RSimpleIdentifierChain), nameof(RSymbol), ".",
                headKey: nameof(__RSymbolOrGlobalKW),
                strict: true, endsWithDelimiter: false, allowEmpty: false);

            RVariableDeclarationStatement = new ParseRule(nameof(RVariableDeclarationStatement),
                new Sequence
                (
                    RuleRefPattern(nameof(__RSimpleIdentifierChain)),
                    RuleRefPattern(nameof(RSymbol)),
                    new Optional(new Sequence(new Literal("="), RuleRefPattern(nameof(RExpression))))
                ),
                ProcessParseResultSequence(results =>
                {
                    var varType = results[0].GetValue<IParseTreeNode[]>().Select(_ => _ as SymbolParseTreeNode)
                        .Aggregate<IParseTreeNode>((a, s) => new AccessorParseTreeNode(a, s as SymbolParseTreeNode));
                    var varName = results[1].GetValue<SymbolParseTreeNode>();
                    var initialValueExpr = results[2].GetValue<ParseResult[]>(); 
                    if (initialValueExpr.Length == 0)
                        return new VariableDeclarationParseTreeNode(varType, varName);

                    initialValueExpr = initialValueExpr[0].GetValue<ParseResult[]>();
                    var initialValue = initialValueExpr[1].GetValue<IParseTreeNode>();
                    return new VariableDeclarationParseTreeNode(varType, varName, initialValue);
                }));

            RPackageDeclarationStatement = new ParseRule(nameof(RPackageDeclarationStatement),
                new Sequence
                (
                    new Literal("package"), RuleRefPattern(nameof(RSymbol)), RuleRefPattern(nameof(RBlockStatement))
                ),
                ProcessParseResultSequence(results =>
                    new PackageParseTreeNode(
                        results[1].GetValue<SymbolParseTreeNode>().SymbolName, results[2].GetValue<BlockParseTreeNode>(),
                        results[0].GetValue<Token>())));


            RGlobalStatements = new ParseRule(nameof(RGlobalStatements),
                RuleRefPattern(nameof(__RStatementsList)),
                ProcessParseResultAs<IParseTreeNode[], IParseTreeNode>((r, l) => 
                {
                    var tks = r.ParsePosition.Tokens;
                    return new RootParseTreeNode(l, new ParsePosition(tks, 0), new ParsePosition(tks, tks.Count - 1));
                }));
            
        }

        class ParseTreeNodesList
        {
            public IParseTreeNode[] Arguments { get; set; }
        }


        class SubscriptList : ParseTreeNodesList { }
        class CallList : ParseTreeNodesList { }        

        class AccessorComponent
        {
            public IParseTreeNode Target { get; }
            public ParseTreeNodesList[] ArgumentedLists { get; }

            public AccessorComponent(IParseTreeNode target, ParseTreeNodesList[] argumentedLists)
            {
                Target = target;
                ArgumentedLists = argumentedLists;
            }
        }

        static class Labels
        {
            public static string Accessor = "";
            public static string String = "";
            public static string Integer10 = "";
            public static string Integer16 = "";
            public static string Number = "";
            public static string PostFix = "";
            public static string Term = "";
            public static string Expression = "";
            public static string Symbol = "";
            public static string List = "";
            internal static string Decimal = "";

            static Labels()
            {
                (from f in typeof(Labels).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                 where f.FieldType == typeof(string)
                 select f)
                .ForEach(f => f.SetValue(null, f.Name));
            }
        }



        Tokenizer.Tokenizer Tokenizer = new Tokenizer.Tokenizer();        
        private ParseRule __RSimpleIdentifierChain;
        private ParseRule RVariableDeclarationStatement;
        private ParseRule RPackageDeclarationStatement;
        private ParseRule __RGlobalKW;
        private ParseRule __RSymbolOrGlobalKW;

        public ParseResult Parse(string input) => Parse(Tokenizer.SplitToTokens(input));


        void AddRules()
        {
            (from rule in
                (from f in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                 where f.FieldType == typeof(ParseRule)
                 select f.GetValue(this) as ParseRule)
             where rule != null
             select rule)             
             .Peek(_=>Debug.WriteLine(_.Key))
             .ForEach(AddRule);
        }

    }
}
