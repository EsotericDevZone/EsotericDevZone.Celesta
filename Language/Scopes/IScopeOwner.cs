using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Scopes
{
    public interface IScopeOwner
    {
        Scope InnerScope { get; }
    }
}
