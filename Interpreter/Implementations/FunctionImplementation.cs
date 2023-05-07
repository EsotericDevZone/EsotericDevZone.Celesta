using EsotericDevZone.Celesta.Definitions;
using System;

namespace EsotericDevZone.Celesta.Interpreter.Implementations
{
    internal class FunctionImplementation
    {
        public Function FunctionDefinition { get; }
        public Func<ValueObject[], ValueObject> Operation { get; }

        public FunctionImplementation(Function functionDefinition, Func<ValueObject[], ValueObject> operation)
        {
            FunctionDefinition = functionDefinition;
            Operation = operation;
        }
    }
}
