using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Definitions.Functions
{
    internal class SyscallFunction : Function
    {
        public SyscallFunction(int syscallId, string name, string package, string scope, DataType[] argumentTypes, DataType outputType)
            : base(name, package, scope, argumentTypes, outputType)
        {
            SyscallId = syscallId;
        }

        public int SyscallId { get; }
    }
}
