using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon_ToolBox
{
    class Attacks
    {
        public string name;
        public int strength;
        public int split; //0 physical 1 special
        public string type;

        public Attacks(string name, int strength, int split, string type)
        {
            this.name = (string)name.Clone();
            this.strength = strength;
            this.split = split;
            this.type = (string)type.Clone();
        }
    }
}
