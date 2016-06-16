using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Exceptions;

namespace CmC.Compiler.Context
{
    public class ExpressionType
    {
        public TypeDef BaseType;
        public int IndirectionLevel;
        public bool IsArray;
        public int ArrayLength;

        public int GetSize()
        {
            if (IndirectionLevel == 0)
            {
                return BaseType.Size;
            }
            else
            {
                //Pointer or address-of
                return 4;
            }
        }

        public int GetStorageSize()
        {
            if (IsArray)
            {
                return BaseType.Size * ArrayLength;
            }
            else
            {
                return GetSize();
            }
        }

        public int GetDereferencedSize()
        {
            if (IndirectionLevel > 1)
            {
                //Pointer to pointer
                return 4;
            }
            else if (IndirectionLevel == 1)
            {
                //Pointer
                return BaseType.Size;
            }
            else
            {
                //Address of
                return 4;
            }
        }

        public override string ToString()
        {
            if (IsArray)
            {
                return BaseType.Name + "[]";
            }
            else if (IndirectionLevel >= 0)
            {
                return BaseType.Name + String.Join("", Enumerable.Range(0, IndirectionLevel).Select(_ => "*"));
            }
            else
            {
                return "&" + BaseType.Name;
            }
        }

        public override bool Equals(object obj)
        {
            var other = (ExpressionType)obj;

            return this.BaseType.Equals(other.BaseType) && this.IndirectionLevel == other.IndirectionLevel;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
