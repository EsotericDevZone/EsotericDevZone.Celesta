using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Core;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class FunctionDeclaration : IParseTreeNode
    {
        public string Name { get; }
        public FunctionArgumentDeclaration[] Arguments { get; }
        public Identifier DataType { get; }
        public IParseTreeNode Body { get; }
        public FunctionDeclaration(string name, FunctionArgumentDeclaration[] arguments, Identifier dataType, IParseTreeNode body)
        {
            Name = name;
            Arguments = arguments;
            DataType = dataType;
            Body = body;
        }

        public override string ToString() => $"{DataType} {Name}({Arguments.JoinToString(",")}) begin\n{Body.ToString().Indent("    ")}\nend";        
    }
}
