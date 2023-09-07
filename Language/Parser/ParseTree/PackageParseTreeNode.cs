using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Parser;
using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class PackageParseTreeNode : AbstractParseTreeNode
    {
        public string PackageName { get; }
        public BlockParseTreeNode Content { get; }

        public PackageParseTreeNode(string packageName, BlockParseTreeNode content, Token packageToken)
            : base(packageToken.ParsePosition, content.EndPosition)
        {
            PackageName = packageName;
            Content = content;
        }

        public override string ToString()
        {
            var br = Environment.NewLine;
            return $"package {PackageName}{br}{Content}{br}";
        }
    }
}
