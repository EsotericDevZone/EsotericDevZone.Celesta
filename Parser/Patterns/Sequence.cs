using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public class Sequence : AbstractPattern
    {
        public Sequence(params IPattern[] patterns)
        {
            Patterns = patterns.ToList();
        }

        public List<IPattern> Patterns;

        public override PatternMatchResult Parse(ParsePosition parsePosition, ParseErrorCollector errorCollector)
        {
            List<ParseResult> itemsResults = new List<ParseResult>();
            var newPosition = parsePosition.Clone();

            foreach (var pattern in Patterns)
            {
                var res = pattern.Parse(newPosition, errorCollector);
                if (!res.Success)
                    return errorCollector.Add(PatternMatchResult.Failed(parsePosition).SetInnerResult(res));
                itemsResults.Add(res.ParseResult);
                newPosition = res.ParsePosition;
            }
            return PatternMatchResult.Succeeded(newPosition, itemsResults.ToArray()).SetLabel(Label);
        }
    }
}
