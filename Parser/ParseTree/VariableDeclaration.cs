namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class VariableDeclaration : IParseTreeNode
    {
        public Identifier DataType { get; }
        public string VariableName { get; }
        public IParseTreeNode InitializationValue { get; }

        public VariableDeclaration(Identifier dataType, string variableName, IParseTreeNode initializationValue)
        {
            DataType = dataType;
            VariableName = variableName;
            InitializationValue = initializationValue;
        }

        public VariableDeclaration(Identifier dataType, string variableName)
            : this(dataType, variableName, null) { }

        public override string ToString()
        {
            if(InitializationValue==null)
                return $"{DataType} {VariableName}";
            return $"{DataType} {VariableName} = {InitializationValue}";
        }
    }
}
