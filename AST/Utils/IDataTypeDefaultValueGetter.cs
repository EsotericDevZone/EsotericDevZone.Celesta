using EsotericDevZone.Celesta.Definitions;
using System;

namespace EsotericDevZone.Celesta.AST.Utils
{
    public interface IDataTypeDefaultValueGetter
    {
        void SetDefaultValue(DataType type, Func<IASTNode> valueNode);
        IASTNode GetDefaultValue(DataType type);
    }
}
