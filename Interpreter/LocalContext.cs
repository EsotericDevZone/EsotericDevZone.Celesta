using System.Collections.Generic;
using System.Linq;

namespace EsotericDevZone.Celesta.Interpreter
{
    public class LocalContext
    {
        public Dictionary<string, ValueObject> FormalParameters = new Dictionary<string, ValueObject>();
        
        public LocalContext(LocalContext parent=null)
        {
            if (parent != null)
                FormalParameters = parent.FormalParameters.ToDictionary(_ => _.Key, _ => _.Value);
        }
    }
}
