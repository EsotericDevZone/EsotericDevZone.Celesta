using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public class ZeroOrMore : Many
    {
        public ZeroOrMore(IPattern pattern) : base(pattern, 0, int.MaxValue / 2) { }
    }

    public class OneOrMore : Many
    {
        public OneOrMore(IPattern pattern) : base(pattern, 1, int.MaxValue / 2) { }
    }

    public class Optional : Many
    {
        public Optional(IPattern pattern) : base(pattern, 0, 1) { }
    }    

    public class Many : AbstractPattern
    {
        public IPattern Pattern { get; }
        public int Min { get; }
        public int Max { get; }        

        public Many(IPattern pattern, int min, int max)
        {
            Pattern = pattern;
            Min = min;
            Max = max;            
        }

        public override PatternMatchResult Parse(ParsePosition parsePosition, ParseErrorCollector errorCollector)
        {
            var newPos = parsePosition;

            List<ParseResult> results = new List<ParseResult>();            

            while (true) 
            {                
                var res = Pattern.Parse(newPos, errorCollector);                

                if (!res.Success)
                {
                    if (results.Count < Min) 
                    {                        
                        return errorCollector.Add(PatternMatchResult.Failed(parsePosition)
                            .SetInnerResult(res)
                            .SetErrorMessage($"Insufficient matches (min {Min}, max {Max})"));
                    }
                    return PatternMatchResult.Succeeded(newPos, results.ToArray());
                }

                results.Add(res.ParseResult);
                newPos = res.ParsePosition;

                if(results.Count>Max)
                {
                    return errorCollector.Add(PatternMatchResult.Failed(parsePosition)
                            .SetInnerResult(res)
                            .SetErrorMessage($"Too many matches (min {Min}, max {Max})"));
                }
            }           
        }
    }
}
