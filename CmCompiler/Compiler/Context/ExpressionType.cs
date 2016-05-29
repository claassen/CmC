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
        public TypeDef Type;
        public int IndirectionLevel;
        public bool IsArray;
        public int ArrayLength;

        //public bool IsFunction;
        //public ExpressionType ReturnType;
        //public List<ExpressionType> ArgumentTypes;

        public int GetSize()
        {
            if (IndirectionLevel == 0)
            {
                return Type.Size;
            }
            else
            {
                //Pointer or address-of
                return 4;
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
                return Type.Size;
            }
            else
            {
                //Address of
                return 4;
            }
        }

        public static void CheckTypesMatch(ExpressionType t1, ExpressionType t2)
        {
            if (t1.Type.Name != t2.Type.Name)
            {
                throw new TypeMismatchException(t1, t2);
            }

            if (t1.IndirectionLevel != t2.IndirectionLevel)
            {
                throw new TypeMismatchException(t1, t2);
            }

            if (t1.Type.IsFunction != t2.Type.IsFunction)
            {
                throw new TypeMismatchException(t1, t2);
            }

            if (t1.Type.IsFunction)
            {
                CheckTypesMatch(t1.Type.ReturnType, t2.Type.ReturnType);

                if (t1.Type.ArgumentTypes.Count != t2.Type.ArgumentTypes.Count)
                {
                    throw new TypeMismatchException(t1, t2);
                }

                for (int i = 0; i < t1.Type.ArgumentTypes.Count; i++)
                {
                    CheckTypesMatch(t1.Type.ArgumentTypes[i], t2.Type.ArgumentTypes[i]);
                }
            }
        }

        public static void CheckTypeIsNumeric(ExpressionType t)
        {
            if (t.GetSize() != 4)
            {
                throw new TypeMismatchException(new ExpressionType() { Type = new TypeDef() { Name = "Numeric value (4 byte value)" } }, t);
            }
        }

        public static void CheckTypeIsBoolean(ExpressionType t)
        {
            if (t.GetSize() != 4)
            {
                throw new TypeMismatchException(new ExpressionType() { Type = new TypeDef() { Name = "Boolean value (4 byte value)" } }, t);
            }
        }

        public override string ToString()
        {
            if (IsArray)
            {
                return Type.Name + "[]";
            }
            else if (IndirectionLevel >= 0)
            {
                return Type.Name + String.Join("", Enumerable.Range(0, IndirectionLevel).Select(_ => "*"));
            }
            else
            {
                return "&" + Type.Name;
            }
        }

        public override bool Equals(object obj)
        {
            var other = (ExpressionType)obj;

            return this.Type.Equals(other.Type) && this.IndirectionLevel == other.IndirectionLevel;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
