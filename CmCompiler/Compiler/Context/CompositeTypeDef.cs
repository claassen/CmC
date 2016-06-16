using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Context
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
                    size += field.Value.Type.GetStorageSize();
                }

                if (size % 2 != 0)
                {
                    //Pad type to be multiple of 2 bytes in size
                    size++;
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
