namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class FunctionArgumentDeclaration : IParseTreeNode
    {
        public Identifier DataType { get; }
        public string Name { get; }

        public FunctionArgumentDeclaration(Identifier dataType, string name)
        {
            DataType = dataType;
            Name = name;
        }

        public override string ToString() => $"{DataType} {Name}";
    }
}
