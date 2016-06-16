using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Context
{
    public class TypeDef
    {
        public string Name;
        public virtual int Size { get; set; }

        public override bool Equals(object obj)
        {
            var other = (TypeDef)obj;

            return this.Name == other.Name && this.Size == other.Size;
        }

        public override int GetHashCode()
        {
            return (Name + Size).GetHashCode();
        }
    }

    public class FunctionTypeDef : TypeDef
    {
        public List<ExpressionType> ArgumentTypes;
        public ExpressionType ReturnType;

        public FunctionTypeDef()
        {
            Name = "Function";
        }

        public override bool Equals(object obj)
        {
            var other = (TypeDef)obj;

            if (!(other is FunctionTypeDef))
            {
                return false;
            }

            return true;

            //return this.Name == other.Name && 
        }

        public override int GetHashCode()
        {
            return (Name + Size).GetHashCode();
        }
    }

    public class StringLiteralTypeDef : TypeDef
    {
        public string Value;
    }
}
