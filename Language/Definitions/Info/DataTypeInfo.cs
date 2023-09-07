using EsotericDevZone.Celesta.Language.Scopes;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Definitions.Info
{
    public class DataTypeInfo : DefinitionInfo 
    {
        private DataTypeInfo(Scope scope, string name) : base(scope, name)
        {

        }

        public int DataSize { get; private set; }
        public int SizeOnStack { get; private set; }
        public bool IsPrimitive { get; private set; }        
        public bool IsStruct { get; private set; }
        public bool IsClass { get; private set; }

        public override string ToString()
        {
            if(IsPrimitive)
            {
                return $"DataTypeInfo{{primitive {FullName}}}";
            }

            return base.ToString();
        }


        // class Actor begin syscall procedure set_frame(int x, int y) end
        // param int x;
        // param int y;

        DataTypeInfo MakePrimitive()
        {
            IsPrimitive = true;
            DataSize = SizeOnStack = 4;
            return this;
        }

        public static DataTypeInfo Primitive(Scope scope, string name) => new DataTypeInfo(scope, name).MakePrimitive();

        public override bool Equals(object obj)
        {
            return obj is DataTypeInfo info &&
                   base.Equals(obj) &&
                   EqualityComparer<Scope>.Default.Equals(Scope, info.Scope) &&
                   Name == info.Name &&                   
                   DataSize == info.DataSize &&
                   SizeOnStack == info.SizeOnStack &&
                   IsPrimitive == info.IsPrimitive &&
                   IsStruct == info.IsStruct &&
                   IsClass == info.IsClass;
        }

        public override int GetHashCode()
        {
            int hashCode = 1574925014;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Scope>.Default.GetHashCode(Scope);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);            
            hashCode = hashCode * -1521134295 + DataSize.GetHashCode();
            hashCode = hashCode * -1521134295 + SizeOnStack.GetHashCode();
            hashCode = hashCode * -1521134295 + IsPrimitive.GetHashCode();
            hashCode = hashCode * -1521134295 + IsStruct.GetHashCode();
            hashCode = hashCode * -1521134295 + IsClass.GetHashCode();
            return hashCode;
        }
    }    
}
