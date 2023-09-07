using EsotericDevZone.Celesta.Language.Definitions.Info;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Definitions.Entities
{
    public class Variable : Definition<VariableInfo>
    {
        public Variable(VariableInfo definitionInfo, DataType varType) : base(definitionInfo)
        {
            if (!definitionInfo.VariableType.Equals(varType.DefinitionInfo))
                throw new ArgumentException("Provided variable type does not match its info signature");
        }

        public DataType VariableType { get; }
    }
}
