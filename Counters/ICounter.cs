namespace EsotericDevZone.Celesta.Counters
{
    internal interface ICounter<T>
    {
        T GetNextValue();
    }
}
