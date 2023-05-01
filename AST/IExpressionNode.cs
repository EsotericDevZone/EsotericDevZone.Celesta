using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    public interface IExpressionNode : IASTNode
    {
        DataType OutputType { get; }
    }
}
