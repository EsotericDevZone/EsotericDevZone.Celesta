using EsotericDevZone.Celesta.Language.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Definitions.Info
{
    public class VariableInfo : DefinitionInfo
    {
        public VariableInfo(Scope scope, string name, DataTypeInfo variableType) : base(scope, name)
        {
            VariableType = variableType;
        }

        public DataTypeInfo VariableType { get; }


        public override string ToString() => $"VariableInfo{{{FullName} : {VariableType}}}";
    }
}
