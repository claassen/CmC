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

        public int GetSize()
        {
            if (IndirectionLevel == 0)
            {
                return Type.Size;
            }
            else
            {
                return 1;
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
        }

        public static void CheckTypeIsNumeric(ExpressionType t)
        {
            if (!new[] { "int", "bool" }.Contains(t.Type.Name))
            {
                throw new TypeMismatchException(new ExpressionType() { Type = new TypeDef() { Name = "Numeric value" } }, t);
            }
        }

        public static void CheckTypeIsBoolean(ExpressionType t)
        {
            if (!new[] { "bool", "int" }.Contains(t.Type.Name))
            {
                throw new TypeMismatchException(new ExpressionType() { Type = new TypeDef() { Name = "Boolean value" } }, t);
            }
        }

        public override string ToString()
        {
            if (IndirectionLevel >= 0)
            {
                return Type.Name + String.Join("", Enumerable.Range(0, IndirectionLevel).Select(_ => "*"));
            }
            else
            {
                return "&" + Type.Name;
            }
        }
    }
}
