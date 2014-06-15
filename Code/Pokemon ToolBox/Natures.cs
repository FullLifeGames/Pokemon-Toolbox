using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon_ToolBox
{
    class Natures
    {
        public string name;
        public string raises;
        public string lowers;

        public Natures(string name, string raises, string lowers)
        {
            this.name = (string)name;
            this.raises = (string)raises;
            this.lowers = (string)lowers;
        }
    }
}
