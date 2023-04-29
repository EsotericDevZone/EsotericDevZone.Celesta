namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class NumericLiteral: IParseTreeNode
    {
        public string Value { get; set; }

        public NumericLiteral(string value)
        {
            Value = value;
        }

        public override string ToString() => $"Number({Value})";
    }
}
