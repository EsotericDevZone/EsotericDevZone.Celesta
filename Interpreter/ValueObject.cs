using EsotericDevZone.Celesta.Definitions;


namespace EsotericDevZone.Celesta.Interpreter
{
    public class ValueObject
    {
        public DataType DataType;
        public object Value;
        public ValueObject(DataType dataType, object value)
        {
            DataType = dataType;
            Value = value;
        }

        internal bool ScopeMustReturn { get; set; } = false;
    }
}
