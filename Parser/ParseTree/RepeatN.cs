using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class RepeatNBlock : IParseTreeNode
    {
        public IParseTreeNode Count { get; }
        public IParseTreeNode Loop { get; }

        public RepeatNBlock(IParseTreeNode count, IParseTreeNode loop)
        {
            Count = count;
            Loop = loop;
        }

        public override string ToString() => $"repeat {Count} do\n{Loop.ToString().Indent("    ")}\nendrepeat";
    }
}
