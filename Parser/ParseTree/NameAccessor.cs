namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class NameAccessor : IParseTreeNode
    {
        public IParseTreeNode TargetNode { get; }
        public string TargetName { get; }

        public NameAccessor(IParseTreeNode targetNode, string targetName)
        {
            TargetNode = targetNode;
            TargetName = targetName;
        }

        public override string ToString() => $"{TargetNode}.{TargetName}";
    }
}
