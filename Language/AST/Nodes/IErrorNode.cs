namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    public interface IErrorNode : IASTNode
    {
        string ErrorMessage { get; }
    }
}
