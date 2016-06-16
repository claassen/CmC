using CmC.Compiler.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Context
{
    public static class TypeChecking
    {
        public static bool TypesMatch(TypeDef t1, TypeDef t2)
        {
            if (t1.Name != t2.Name)
            {
                return false;
                //throw new TypeMismatchException(t1, t2);
            }

            if ((t1 is FunctionTypeDef) != (t2 is FunctionTypeDef))
            {
                return false;
                //throw new TypeMismatchException(t1, t2);
            }

            if (t1 is FunctionTypeDef)
            {
                FunctionTypeDef t1FunctionType = (FunctionTypeDef)t1;
                FunctionTypeDef t2FunctionType = (FunctionTypeDef)t2;

                CheckExpressionTypesMatch(t1FunctionType.ReturnType, t2FunctionType.ReturnType);

                if (t1FunctionType.ArgumentTypes.Count != t2FunctionType.ArgumentTypes.Count)
                {
                    return false;
                    //throw new TypeMismatchException(t1, t2);
                }

                for (int i = 0; i < t1FunctionType.ArgumentTypes.Count; i++)
                {
                    CheckExpressionTypesMatch(t1FunctionType.ArgumentTypes[i], t2FunctionType.ArgumentTypes[i]);
                }
            }

            return true;
        }

        public static void CheckExpressionTypesMatch(ExpressionType t1, ExpressionType t2)
        {
            if (t1.IndirectionLevel != t2.IndirectionLevel)
            {
                throw new TypeMismatchException(t1, t2);
            }

            if (!TypesMatch(t1.BaseType, t2.BaseType))
            {
                throw new TypeMismatchException(t1, t2);
            }

            //if (t1.Type.Name != t2.Type.Name)
            //{
            //    throw new TypeMismatchException(t1, t2);
            //}

            //if (t1.IndirectionLevel != t2.IndirectionLevel)
            //{
            //    throw new TypeMismatchException(t1, t2);
            //}

            //if ((t1.Type is FunctionTypeDef) != (t2.Type is FunctionTypeDef))
            //{
            //    throw new TypeMismatchException(t1, t2);
            //}

            //if (t1.Type is FunctionTypeDef)
            //{
            //    FunctionTypeDef t1FunctionType = (FunctionTypeDef)t1.Type;
            //    FunctionTypeDef t2FunctionType = (FunctionTypeDef)t2.Type;

            //    CheckExpressionTypesMatch(t1FunctionType.ReturnType, t2FunctionType.ReturnType);

            //    if (t1FunctionType.ArgumentTypes.Count != t2FunctionType.ArgumentTypes.Count)
            //    {
            //        throw new TypeMismatchException(t1, t2);
            //    }

            //    for (int i = 0; i < t1FunctionType.ArgumentTypes.Count; i++)
            //    {
            //        CheckExpressionTypesMatch(t1FunctionType.ArgumentTypes[i], t2FunctionType.ArgumentTypes[i]);
            //    }
            //}
        }

        public static void CheckExpressionTypeIsNumeric(ExpressionType t)
        {
            if (t.GetSize() != 4)
            {
                throw new TypeMismatchException(new ExpressionType() { BaseType = new TypeDef() { Name = "Numeric value (4 byte value)" } }, t);
            }
        }

        public static void CheckExpressionTypeIsBoolean(ExpressionType t)
        {
            if (t.GetSize() != 4)
            {
                throw new TypeMismatchException(new ExpressionType() { BaseType = new TypeDef() { Name = "Boolean value (4 byte value)" } }, t);
            }
        }
    }
}
