using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class FunctionArgumentDeclaration : IParseTreeNode
    {
        public IParseTreeNode DataType { get; }
        public string Name { get; }

        public FunctionArgumentDeclaration(IParseTreeNode dataType, string name)
        {
            DataType = dataType;
            Name = name;
        }

        public override string ToString() => $"{DataType} {Name}";
    }
}
