using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Extensions
{
    internal static class IntExtensions
    {
        public static bool IsBetween(this int x, int a, int b) => x >= a && x <= b;
    }
}
