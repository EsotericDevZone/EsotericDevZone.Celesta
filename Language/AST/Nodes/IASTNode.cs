using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using EsotericDevZone.Celesta.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    public interface IASTNode
    {
        IParseTreeNode ParseTreeNode { get; }
        ParsePosition BeginPosition { get; }
        ParsePosition EndPosition { get; }
        Scope ResidingScope { get; }        
    }
}
