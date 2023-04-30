using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class Package : IParseTreeNode
    {
        public string Name { get; }
        public Block Content { get; }

        public Package(string name, Block content)
        {
            Name = name;
            Content = content;
        }

        public override string ToString() => $"package {Name}\n{Content.ToString().Indent("    ")}\nendpackage";
    }
}
