using EsotericDevZone.Celesta.Parser.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Parser
{
    public class ParseRule
    {
        public string Key { get; }
        public IPattern Pattern { get; }
        public Func<PatternMatchResult, ParseResult> BuildResult { get; }

        public ParseRule(string key, IPattern pattern, Func<PatternMatchResult, ParseResult> buildResult)
        {
            Key = key;
            Pattern = pattern;
            BuildResult = buildResult;
        }        
    }
}
