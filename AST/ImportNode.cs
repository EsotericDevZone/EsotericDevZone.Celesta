namespace EsotericDevZone.Celesta.AST
{
    public class ImportNode : AbstractASTNode
    {
        public string Source { get; }        
        internal ImportNode(IASTNode parent, string source) : base(parent)
        {
            Source = source;            
        }
    }
}
