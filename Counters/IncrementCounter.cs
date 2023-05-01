using System.Threading;

namespace EsotericDevZone.Celesta.Counters
{
    internal class IncrementCounter : ICounter<int>
    {
        private static int _counter;
        public int GetNextValue() => Interlocked.Increment(ref _counter);
    }
}
