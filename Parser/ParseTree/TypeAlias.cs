﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class TypeAlias : IParseTreeNode
    {
        public string TypeName { get; }
        public Identifier ReferencedType { get; }

        public bool Isolated { get; }

        public TypeAlias(string typeName, Identifier referencedType, bool isolated)
        {
            TypeName = typeName;
            ReferencedType = referencedType;
            Isolated = isolated;
        }

        public override string ToString()
            => $"talias {TypeName} = {ReferencedType} {(Isolated ? "isolated" : "")}";
    }
}
