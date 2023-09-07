using EsotericDevZone.Celesta.Language.Builder;
using EsotericDevZone.Celesta.Language.Definitions.Entities;
using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using EsotericDevZone.Celesta.Parser;
using System.Linq;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    public class DataTypeNode : AbstractDefinitionHolderNode<DataType, DataTypeInfo>
    {
        public string DataTypeName { get; }
        public DataType DataType => Definition;

        public DataTypeNode(IParseTreeNode parseTreeNode, Scope scope, DataType dataType):base(parseTreeNode,scope)
        {
            EditableDefHolder.Definition = dataType;
        }      

        public override string ToString()
        {
            if (IsSpeculative)
                return $"speculative datatype '{DataTypeName}'";
            else
                return $"datatype '{Definition}'";
        } 
    }
}
