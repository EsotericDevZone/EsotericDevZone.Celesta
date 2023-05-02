using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.AST
{
    public class IfNode : AbstractASTNode
    {
        public IfNode(IASTNode parent) : base(parent)
        {            
        }

        public IExpressionNode Condition { get; internal set; }
        public IASTNode ThenBranch { get; internal set; }
        public IASTNode ElseBranch { get; internal set; }

        public override string ToString()
        {
            var result = $"if {Condition} then\n";
            result += ThenBranch.ToString().Indent("    ") + "\n";
            if (ElseBranch != null) 
            {
                result += "else\n";
                result += ElseBranch.ToString().Indent("    ") + "\n";
            }
            result += "endif";
            return result;
        }        
    }
}
