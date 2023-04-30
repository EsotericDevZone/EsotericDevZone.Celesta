namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class Return : IParseTreeNode
    {
        public IParseTreeNode Expression { get; }

        public Return(IParseTreeNode expression)
        {
            Expression = expression;
        }

        public override string ToString() => $"return {Expression?.ToString() ?? ""}";
    }
}
