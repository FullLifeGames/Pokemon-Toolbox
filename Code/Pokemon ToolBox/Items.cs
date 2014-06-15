using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon_ToolBox
{
    class Items
    {
        public string name;
        public string description;

        public Items(string name, string description)
        {
            this.name = (string)name.Clone();
            this.description = (string)description.Clone();
        }
    }
}
