using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public class Choose : AbstractPattern
    {
        public Choose(params IPattern[] patterns)
        {
            Patterns = patterns.ToList();
        }

        public List<IPattern> Patterns;

        public override PatternMatchResult Parse(ParsePosition parsePosition, ParseErrorCollector errorCollector)
        {            
            PatternMatchResult bestErrorInnerResult = null;

            foreach (var pattern in Patterns)
            {
                var res = pattern.Parse(parsePosition, errorCollector);
                if (!res.Success)
                {
                    if (bestErrorInnerResult == null)
                        bestErrorInnerResult = res;
                    else if (bestErrorInnerResult.ParsePosition.Index < res.ParsePosition.Index)
                        bestErrorInnerResult = res;
                    continue;
                }
                return PatternMatchResult.Succeeded(res.ParsePosition, res.ParseResult).SetLabel(Label);
            }
            return errorCollector.Add(PatternMatchResult.Failed(parsePosition).SetInnerResult(bestErrorInnerResult)); 
        }
    }
}
