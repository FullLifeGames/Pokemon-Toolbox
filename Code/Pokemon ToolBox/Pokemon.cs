using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon_ToolBox
{
    class Pokemon
    {
        public int dexnbr;
        public string name;      
        public string type1;
        public string type2;
        public double height;
        public double weight;
        public int[] stats;
        public List<double> sonderheight;
        public List<double> sonderweight;
        public List<string> sondertype1;
        public List<string> sondertype2;
        public List<int[]> sonderstats;

        public Pokemon(int dexnbr, string name, string type1, string type2, double height, double weight, int[] stats, List<double> sonderheight, List<double> sonderweight, List<string> sondertype1, List<string> sondertype2, List<int[]> sonderstats)
        {
            this.dexnbr = dexnbr;
            this.name = (string)name.Clone();
            this.height = height;
            this.weight = weight;
            this.type1 = (string)type1.Clone();
            this.type2 = (string)type2.Clone();
            this.stats = (int[])stats.Clone();
            this.sonderheight = new List<double>(sonderheight.Count);
            sonderheight.ForEach((item) =>
            {
                this.sonderheight.Add(item);
            });
            this.sonderweight = new List<double>(sonderweight.Count);
            sonderweight.ForEach((item) =>
            {
                this.sonderweight.Add(item);
            });
            this.sondertype1 = new List<string>(sondertype1.Count);
            sondertype1.ForEach((item) =>
            {
                this.sondertype1.Add((string)item.Clone());
            });
            this.sondertype2 = new List<string>(sondertype2.Count);
            sondertype2.ForEach((item) =>
            {
                this.sondertype2.Add((string)item.Clone());
            });
            this.sonderstats = new List<int[]>(sonderstats.Count);
            sonderstats.ForEach((item) =>
            {
                this.sonderstats.Add((int[])item.Clone());
            });
                
        }
    }
}
