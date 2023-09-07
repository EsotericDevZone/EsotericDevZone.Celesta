using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using EsotericDevZone.Celesta.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    public class ErrorNode : AbstractASTNode, IErrorNode
    {
        public ErrorNode(IParseTreeNode parseTreeNode, Scope scope, string errorMessage) : base(parseTreeNode, scope)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }

        public override string ToString() => ErrorMessage;        

    }
}
