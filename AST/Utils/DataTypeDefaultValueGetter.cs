using EsotericDevZone.Celesta.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.AST.Utils
{
    internal class DataTypeDefaultValueGetter : IDataTypeDefaultValueGetter
    {
        public Dictionary<DataType, Func<IASTNode>> DefaultValues { get; } = new Dictionary<DataType, Func<IASTNode>>();

        public IASTNode GetDefaultValue(DataType type)
        {
            if (DefaultValues.ContainsKey(type))
                return DefaultValues[type]();

            if (!type.IsAlias)
                throw new ArgumentException($"No default value set for type {type.FullName}");
            return GetDefaultValue(type.AliasedType);
        }

        public void SetDefaultValue(DataType type, Func<IASTNode> valueNode)
        {            
            DefaultValues[type] = valueNode;
        }
    }
}
