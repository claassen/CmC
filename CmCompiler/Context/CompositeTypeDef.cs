using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Context
{
    public class CompositeTypeDef : TypeDef
    {
        public Dictionary<string, Field> Fields;

        public override int Size
        {
            get
            {
                int size = 0;

                foreach (var field in Fields)
                {
                    size += field.Value.Type.GetSize();
                }

                return size;
            }
            set
            {
                throw new Exception("Can't set size in composite type");
            }
        }
    }
}
