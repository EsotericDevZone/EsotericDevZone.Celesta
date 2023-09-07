using EsotericDevZone.Celesta.Language.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Definitions.Info
{
    public abstract class DefinitionInfo
    {
        public Scope Scope { get; }
        public string Name { get; }        
        public string FullName => $"{Scope.FullName}.{Name}";

        protected DefinitionInfo(Scope scope, string name)
        {
            Name = name;
            Scope = scope;
        }        

        public override bool Equals(object obj)
        {
            return obj is DefinitionInfo info &&
                   Name == info.Name &&
                   EqualityComparer<Scope>.Default.Equals(Scope, info.Scope);
        }

        public override int GetHashCode()
        {
            int hashCode = -2079862911;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<Scope>.Default.GetHashCode(Scope);
            return hashCode;
        }
    }
}
