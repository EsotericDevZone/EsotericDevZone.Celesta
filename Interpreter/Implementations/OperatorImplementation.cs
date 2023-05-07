using EsotericDevZone.Celesta.Definitions;
using System;

namespace EsotericDevZone.Celesta.Interpreter.Implementations
{
    internal class OperatorImplementation
    {
        public Operator OperatorDefinition { get; }
        public Func<ValueObject[], ValueObject> Operation { get; }
        public OperatorImplementation(Operator operatorDefinition, Func<ValueObject[], ValueObject> operation)
        {
            OperatorDefinition = operatorDefinition;
            Operation = operation;
        }
    }
}
