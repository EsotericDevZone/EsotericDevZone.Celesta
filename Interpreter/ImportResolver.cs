using EsotericDevZone.Celesta.AST.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Interpreter
{
    internal class ImportResolver : IImportResolver
    {
        public ISet<string> SourcesLookupPaths = new HashSet<string>();

        public void AddPath(string path) => SourcesLookupPaths.Add(Path.GetFullPath(path));

        public string GetSourcePath(string source, bool isFileName)
        {
            string path;
            if (isFileName)
            {
                path = Path.GetFullPath(source);
            }
            else
            {
                source += ".cels";
                if (File.Exists(source))
                    path = Path.GetFullPath(source);
                else
                {
                    foreach (var dir in SourcesLookupPaths)
                    {
                        var absPath = Path.Combine(dir, source);
                        if (File.Exists(absPath))
                        {
                            path = absPath;
                            break;
                        }
                    }
                    path = source;
                }
            }
            if (!File.Exists(path))
                throw new FileNotFoundException($"Import file not found : '{source}'");

            return path;            
        }
        
    }
}
