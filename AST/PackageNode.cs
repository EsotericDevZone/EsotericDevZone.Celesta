using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.AST
{
    internal class PackageNode : AbstractASTNode, IBlockNode
    {
        public string Name { get; }
        public string FullName { get; }

        public PackageNode(IASTNode parent, string name) : base(parent)
        {
            Name = name;
            var parentPackage = GetPackage(parent);
            FullName = parentPackage == "" ? name : parentPackage + "#" + name;
        }

        internal List<IASTNode> Children { get; } = new List<IASTNode>();
        public IASTNode[] GetChildren() => Children.ToArray();
        internal void AddChild(IASTNode node)
        {
            (node as AbstractASTNode).Parent = this;
            Children.Add(node);
        }

        public override string GetPackageName() => FullName;
        public override string ToString() => $"package {Name} /*{FullName}*/\n{Children.JoinToString("\n").Indent("    ")}\nend";
    }
}
