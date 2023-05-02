using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.AST
{
    public class FunctionCallNode : AbstractASTNode, IExpressionNode
    {
        public FunctionCallNode(IASTNode parent) : base(parent) { }
        public Function Function { get; internal set; }
        public IExpressionNode[] Arguments { get; internal set; }
        public DataType OutputType => Function.OutputType;
        public override string ToString() => $"call:{Function.FullName}({Arguments.JoinToString(", ")})";
    }
}
