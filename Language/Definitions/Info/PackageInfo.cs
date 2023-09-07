using EsotericDevZone.Celesta.Language.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Definitions.Info
{
    public class PackageInfo : DefinitionInfo
    {
        public PackageInfo(Scope scope, string name) : base(scope, name)
        {
        }

        public override string ToString() => $"PackageInfo{{{FullName}}}";

        public override bool Equals(object obj)
        {
            return obj is PackageInfo info &&
                   base.Equals(obj) &&
                   Name == info.Name &&
                   EqualityComparer<Scope>.Default.Equals(Scope, info.Scope);
        }

        public override int GetHashCode()
        {
            int hashCode = -1648601209;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<Scope>.Default.GetHashCode(Scope);
            return hashCode;
        }
    }
}
