using System;

namespace EsotericDevZone.Celesta.Counters
{
    internal class FunctionalStringCounter : IStringCounter
    {
        private readonly Func<int, string> Generator = i => $"{i}";
        private readonly IncrementCounter Counter = new IncrementCounter();

        public FunctionalStringCounter() { }
        public FunctionalStringCounter(Func<int, string> generator) => Generator = generator;
        public string GetNextValue() => Generator(Counter.GetNextValue());
    }
}
