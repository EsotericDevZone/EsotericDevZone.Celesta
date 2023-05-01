using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.Core;
using EsotericDevZone.Core.Collections;
using EsotericDevZone.RuleBasedParser;
using EsotericDevZone.RuleBasedParser.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
namespace EsotericDevZone.Celesta.Parser
{
    internal static class ParseTreeNodeBuilders
    {
        public static ParseResult Identifier(ParseResult[] results)
        {
            return new ParseResult(results[0].GeneratorToken,
                    new Identifier(results.Select(_ => _.Value as string).ToArray()));
        }

        public static ParseResult NameAccessor(ParseResult[] results, Token[] tokens)
        {            
            return ParseResultBuilders.LeftAssociate((e, name) =>
                new ParseResult(name.GeneratorToken, new NameAccessor(e.Value as IParseTreeNode, name.Value as string)))
                (results, tokens);
        }

        public static ParseResult __List(ParseResult[] results)
        {            
            return new ParseResult(null, results.ToArray());
        }

        
        public static ParseResult FunctionCall(ParseResult[] results)
        {            
            //Console.WriteLine(string.Join("\n", results.Select(x => x.Value.ToString())));

            if (results.Length == 1) 
                return new ParseResult(results[0].GeneratorToken, 
                    new FunctionCall(results[0].Value as IParseTreeNode, new IParseTreeNode[0]));
            
            var argslist = (results[1].Value as ParseResult[]).Select(_ => _.Value as IParseTreeNode).ToArray();

            //argslist.ToList().ForEach(Console.WriteLine);
            return new ParseResult(results[0].GeneratorToken,
                new FunctionCall(results[0].Value as IParseTreeNode, argslist));
        }

        public static ParseResult Assignment(ParseResult[] results)
        {            
            return new ParseResult(results[0].GeneratorToken,
                new Assignment(results[0].Value as IParseTreeNode, results[1].Value as IParseTreeNode));
        }

        public static ParseResult If(ParseResult[] results)
        {
            if (results.Length == 2)
                return new ParseResult(results[0].GeneratorToken,
                    new IfBlock(results[0].Value as IParseTreeNode, results[1].Value as Block));
            else //if (results.Length == 3)
                return new ParseResult(results[0].GeneratorToken,
                    new IfBlock(results[0].Value as IParseTreeNode, results[1].Value as Block, results[2].Value as Block));
        }

        public static ParseResult While(ParseResult[] results)
        {            
            return new ParseResult(results[0].GeneratorToken,
                    new WhileBlock(results[0].Value as IParseTreeNode, results[1].Value as Block));
        }

        public static ParseResult RepeatN(ParseResult[] results)
        {
            return new ParseResult(results[0].GeneratorToken,
                    new RepeatNBlock(results[0].Value as IParseTreeNode, results[1].Value as Block));
        }

        public static ParseResult Block(ParseResult[] results) =>
            new ParseResult(null, new Block(results.Select(_ => _.Value as IParseTreeNode).ToArray()));

        public static ParseResult VariableDeclaration(ParseResult[] results)
        {
            if (results.Length == 2)
                return new ParseResult(results[0].GeneratorToken,
                    new VariableDeclaration(results[0].Value as Identifier, results[1].Value as string));
            if (results.Length == 3)
                return new ParseResult(results[0].GeneratorToken,
                    new VariableDeclaration(results[0].Value as Identifier, results[1].Value as string, results[2].Value as IParseTreeNode));
            throw new ParseException("Invalid variable declaration");            
        }

        public static ParseResult ArgumentDeclaration(ParseResult[] results)
        {            
            return new ParseResult(results[0].GeneratorToken,
                new FunctionArgumentDeclaration(results[0].Value as Identifier, results[1].Value as string));            
        }

        public static ParseResult TypeAlias(ParseResult[] results)
        {
            return new ParseResult(results[0].GeneratorToken, 
                new TypeAlias(results[0].Value as string, results[1].Value as Identifier, false));
        }

        public static ParseResult TypeAliasIsolated(ParseResult[] results)
        {
            return new ParseResult(results[0].GeneratorToken,
                new TypeAlias(results[0].Value as string, results[1].Value as Identifier, true));
        }

        public static ParseResult SyscallFunctionHeader(ParseResult[] results)
        {
            if (results.Length == 3)
                return new ParseResult(results[1].GeneratorToken,
                    new SyscallFunctionHeader((int)results[0].Value, (string)results[1].Value,
                        Arrays.Empty<FunctionArgumentDeclaration>(), results[2].Value as IParseTreeNode));
            else // if (results.Length == 4)                          
                return new ParseResult(results[1].GeneratorToken,
                    new SyscallFunctionHeader((int)results[0].Value, (string)results[1].Value,
                        (results[2].Value as ParseResult[]).Select(_ => _.Value as FunctionArgumentDeclaration).ToArray(),
                        results[3].Value as IParseTreeNode));           
        }

        public static ParseResult FunctionDeclaration(ParseResult[] results)
        {
            if (results.Length == 3)
                return new ParseResult(results[0].GeneratorToken, new FunctionDeclaration((string)results[0].Value,
                    Arrays.Empty<FunctionArgumentDeclaration>(),
                    results[1].Value as IParseTreeNode,
                    results[2].Value as IParseTreeNode)
                    );
            else // if (results.Length == 4)         
                return new ParseResult(results[0].GeneratorToken, new FunctionDeclaration((string)results[0].Value,
                    (results[1].Value as ParseResult[]).Select(_ => _.Value as FunctionArgumentDeclaration).ToArray(),
                    results[2].Value as IParseTreeNode,
                    results[3].Value as IParseTreeNode)
                    );
        }

        public static ParseResult Package(ParseResult[] results)
        {            
            return new ParseResult(results[0].GeneratorToken, new Package(results[0].Value as string, results[1].Value as Block));
        }

        public static ParseResult Return(ParseResult[] results, Token[] tokens)
        {
            return new ParseResult(tokens[0], new Return(results.FirstOrDefault()?.Value as IParseTreeNode));
        }

        public static ParseResult UnaryOperator(ParseResult[] results, Token[] tokens)
        {
            return new ParseResult(tokens[0], new UnaryOperator(tokens[0], results[0].Value as IParseTreeNode));
        }

    }
}
