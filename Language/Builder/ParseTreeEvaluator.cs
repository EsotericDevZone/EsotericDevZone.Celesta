using EsotericDevZone.Celesta.Language.AST.Nodes;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Builder
{
    public class ParseTreeEvaluator
    {        
        public BuildContext BuildContext { get; }
        public ASTBuilder ASTBuilder { get; }

        public ParseTreeEvaluator(BuildContext buildContext, ASTBuilder astBuilder)
        {
            BuildContext = buildContext;
            ASTBuilder = astBuilder;
        }

        public IASTNode BuildAST(IParseTreeNode parseTreeNode)
        {
            return ASTBuilder.Build(parseTreeNode, BuildContext, BuildContext.GlobalScope);
        }


    }
}
