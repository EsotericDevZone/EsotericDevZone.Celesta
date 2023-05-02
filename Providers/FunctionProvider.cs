using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsotericDevZone.Celesta.Providers
{
    internal class FunctionProvider : AbstractSymbolProvider<Function>, IFunctionProvider
    {
        public IEnumerable<Function> Find(string name, string scope, DataType[] argTypes)
        {
            return Find(name, scope).Where(f=>f.ArgumentTypes.SequenceEqual(argTypes));
        }

        public IEnumerable<Function> Find(string name, string scope, DataType[] argTypes, DataType outputType)
        {
            return Find(name, scope).Where(f => f.ArgumentTypes.SequenceEqual(argTypes)
             && f.OutputType == outputType);
        }

        public IEnumerable<Function> Find(string package, string name, string scope, DataType[] argTypes)
        {
            return Find(package, name, scope).Where(f => f.ArgumentTypes.SequenceEqual(argTypes));
        }

        public IEnumerable<Function> Find(string package, string name, string scope, DataType[] argTypes, DataType outputType)
        {
            return Find(package, name, scope).Where(f => f.ArgumentTypes.SequenceEqual(argTypes)
             && f.OutputType == outputType);
        }

        public Function Resolve(Identifier identifier, string scope, DataType[] argTypes, bool strict)
        {
            var candidates = Find(identifier.PackageName, identifier.Name, scope, argTypes).ToArray();
            if (candidates.Length == 0)
                return null;
            if (strict && candidates.Length > 1)
                throw new MultipleDefinitionException($"Multiple definitions of function {identifier}({argTypes.JoinToString(",")}):\n{candidates.JoinToString("\n").Indent("  ")}");
            return candidates[0];
        }
    }
}
