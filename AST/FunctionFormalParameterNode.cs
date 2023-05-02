using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Celesta.Definitions.Functions;

namespace EsotericDevZone.Celesta.AST
{
    public class FunctionFormalParameterNode : AbstractASTNode, IExpressionNode
    {
        internal FunctionFormalParameterNode(IASTNode parent, FunctionFormalParameter param) : base(parent)
        {
            Parameter = param;
        }

        public FunctionFormalParameter Parameter { get; }
        public string Name => Parameter.Name;
        public DataType OutputType => Parameter.Type;
        public int Index => Parameter.Index;

        public override string ToString() => $"fp:{Name}";
    }
}
