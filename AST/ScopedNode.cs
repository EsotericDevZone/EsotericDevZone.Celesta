using EsotericDevZone.Celesta.Counters;
using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.AST
{
    public class ScopedNode : AbstractASTNode, IBlockNode
    {
        public string ScopeName { get; }

        internal List<IASTNode> Children { get; } = new List<IASTNode>();
        public IASTNode[] GetChildren() => Children.ToArray();
        internal ScopedNode(IASTNode parent, IStringCounter scopeCounter) : base(parent) 
        {
            ScopeName = parent == null ? "@main" : GetScope(parent) + scopeCounter.GetNextValue();
        }
        internal void AddChild(IASTNode node)
        {
            (node as AbstractASTNode).Parent = this;
            Children.Add(node);
        }

        public override string GetScopeName() => ScopeName;

        public override string ToString()
        {
            return $"//blk__{ScopeName}\n" + Children.Select(c => $"{c};").JoinToString("\n");
        }
    }
}
