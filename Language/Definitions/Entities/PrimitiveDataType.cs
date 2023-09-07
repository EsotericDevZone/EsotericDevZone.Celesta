using EsotericDevZone.Celesta.Language.Definitions.Info;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Definitions.Entities
{
    public class PrimitiveDataType : DataType
    {
        public PrimitiveDataType(DataTypeInfo definitionInfo) : base(definitionInfo)
        {
            if (!definitionInfo.IsPrimitive)
                throw new ArgumentException("Expected data type info of a primitive type");
        }
    }
}
