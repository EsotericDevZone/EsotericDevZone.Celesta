using EsotericDevZone.Celesta.Definitions;
using System.Linq;

namespace EsotericDevZone.Celesta.AST
{
    public class OperatorNode : AbstractASTNode, IExpressionNode
    {
        internal OperatorNode(IASTNode parent) : base(parent)
        {            
        }

        public Operator Operator { get; internal set; }
        public IExpressionNode[] Arguments { get; internal set; }

        public DataType OutputType => Operator.OutputType;

        public override string ToString()
        {
            if(Operator.IsUnary)
            {
                return $"{Operator.Name}{Arguments[0]}";
            }
            else
            {
                return $"({Arguments[0]}{Operator.Name}{Arguments[1]})";
            }
        }
    }
}
