using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Exceptions;

namespace CmC
{
    public class Type
    {
        public string Name;
        public int IndirectionLevel;

        public static void CheckTypesMatch(Type t1, Type t2)
        {
            if (t1.Name != t2.Name)
            {
                throw new TypeMismatchException(t1, t2);
            }

            if (t1.IndirectionLevel != t2.IndirectionLevel)
            {
                throw new TypeMismatchException(t1, t2);
            }
        }

        public static void CheckTypeIsNumeric(Type t)
        {
            if (!new[] { "int", "bool" }.Contains(t.Name))
            {
                throw new TypeMismatchException(new Type() { Name = "Numeric value" }, t);
            }
        }

        public static void CheckTypeIsBoolean(Type t)
        {
            if (!new[] { "bool", "int" }.Contains(t.Name))
            {
                throw new TypeMismatchException(new Type() { Name = "Boolean value" }, t);
            }
        }

        public override string ToString()
        {
            if (IndirectionLevel >= 0)
            {
                return Name + String.Join("", Enumerable.Range(0, IndirectionLevel).Select(_ => "*"));
            }
            else
            {
                return "&" + Name;
            }
        }
    }
}
