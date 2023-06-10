using EsotericDevZone.Celesta.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Providers
{
    public class OperatorProvider : AbstractSymbolProvider<Operator>, IOperatorProvider
    {
        public IEnumerable<Operator> FindBinary(string name, DataType argType1, DataType argType2)
            => Where(o => o.IsBinary && o.Name == name 
                && o.ArgumentTypes[0] == argType1 && o.ArgumentTypes[1] == argType2);        

        public IEnumerable<Operator> FindBinary(string name, DataType argType1, DataType argType2, DataType outputType)
            => Where(o => o.IsBinary && o.Name == name
                && o.ArgumentTypes[0] == argType1 && o.ArgumentTypes[1] == argType2
                && o.OutputType==outputType);

        public IEnumerable<Operator> FindUnary(string name, DataType argType1)
            => Where(o => o.IsUnary && o.Name == name
                && o.ArgumentTypes[0] == argType1);

        public IEnumerable<Operator> FindUnary(string name, DataType argType1, DataType outputType)
            => Where(o => o.IsUnary && o.Name == name
                && o.ArgumentTypes[0] == argType1
                && o.OutputType == outputType);
    }
}
