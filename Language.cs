using EsotericDevZone.Core.Collections;
using System.Collections.Generic;

namespace EsotericDevZone.Celesta
{
    public static class Language
    {
        public static readonly List<string> Keywords = Lists.Of(
            "end",
            "if", "then", "else", "endif",
            "while", "do", "endwhile",
            "repeat", "endrepeat",
            "package", "endpackage",

            "type", "record", "endrecord",
            "function", "endfunction", "begin", "return",

            "syscall",
            "true", "false"
            );
    }
}
