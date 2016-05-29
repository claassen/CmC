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
        public bool IsFunction;
        public ExpressionType ReturnType;
        public List<ExpressionType> ArgumentTypes;
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
}
