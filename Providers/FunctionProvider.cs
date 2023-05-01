using EsotericDevZone.Celesta.Definitions;
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
    }
}
