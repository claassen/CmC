using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC
{
    public interface EmitToken
    {
    }

    public class Instruction : EmitToken
    {
        public int Address;
        public Op Op;
        public string Comment;

        public override string ToString()
        {
            return String.Format("{0,-30}", String.Format("{0,-2}", Address) + ": " + Op)
                + (Comment != null ? "; " + Comment : "");
        }
    }

    public class Label : EmitToken
    {
        public string Name;
        public int Address;

        public override string ToString()
        {
            return Name + ": ";
        }
    }

    public class Comment : EmitToken
    {
        public string Text;

        public override string ToString()
        {
            return ";" + Text;
        }
    }
}
