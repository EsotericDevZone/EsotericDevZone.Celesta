namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class BoolLiteral : IParseTreeNode
    {
        public string Value { get; }

        public BoolLiteral(string value)
        {
            Value = value;
        }

        public override string ToString() => $"Bool({Value})";
    }
}
