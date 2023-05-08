using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    public class Import : IParseTreeNode
    {
        public string Source { get; }
        public bool IsFilePath { get; }

        internal Import(string source, bool isFilePath)
        {
            IsFilePath = isFilePath;
            Source = IsFilePath ? source.Substring(1, source.Length - 2).FromLiteral() : source;                      
        }

        public override string ToString()
        {
            return $"import {Source}";
        }
    }
}
