using EsotericDevZone.Core.Collections;
using EsotericDevZone.RuleBasedParser;

namespace EsotericDevZone.Celesta.Parser
{
    internal class CelestaTokensSplitOptions : TokensSplitOptions
    {
        public CelestaTokensSplitOptions() : base(
            Lists.Of("\"", "'"),
            Lists.Of(@"\+", @"\-", @"\*", @"\/", @"\%",
                @"\=\=", @"\<\=", @"\>\=", @"\<", @"\>", @"\!\=",
                @"\=\!", @"\:",
                @"\=", @"\d+\.\d+", @"\#",
                @"\,", @"\;", @"\.",
                @"\(", @"\)"))
        { }
    }
}
