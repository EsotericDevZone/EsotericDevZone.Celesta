namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class StringLiteral : IParseTreeNode
    {
        public string Value { get; }

        public StringLiteral(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;        
    }
}
