using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pokemon_ToolBox
{
    public partial class Form2 : Form
    {

        string mode2;

        public Form2(string[] abc)
        {
            mode = abc[0];
            mode2 = abc[1];
            InitializeComponent();
        }

        private void init()
        {
            initPokemon();
            initAttacks();
            initItems();
            initNatures();
            initAbilities();
        }

        Dictionary<string, Pokemon> matcherPokemon;

        private void initPokemon()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (d.Exists)
            {
                FileInfo f = new FileInfo(System.Environment.CurrentDirectory + "\\Pokemon.data");
                if (f.Exists)
                {
                    matcherPokemon = new Dictionary<string, Pokemon>();
                    donePokemon = new Dictionary<string, bool>();
                    StreamReader sr = new StreamReader(f.OpenRead());
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        int dexnbr = int.Parse(s.Substring(0, s.IndexOf(" ")));
                        s = s.Substring(s.IndexOf(" ") + 3);
                        string name = WebUtility.HtmlDecode(s.Substring(0, s.IndexOf(" ")));
                        s = s.Substring(s.IndexOf(" ") + 1);
                        string type1 = s.Substring(0, s.IndexOf(" "));
                        s = s.Substring(s.IndexOf(" ") + 1);
                        string type2 = s.Substring(0, s.IndexOf(" "));
                        s = s.Substring(s.IndexOf(" ") + 1);
                        double height = Convert.ToDouble((s.Substring(0, s.IndexOf(" ") - 1)), NumberFormatInfo.InvariantInfo);
                        s = s.Substring(s.IndexOf(" ") + 1);
                        double weight = Convert.ToDouble((s.Substring(0, s.IndexOf(" ") - 2)), NumberFormatInfo.InvariantInfo);
                        s = s.Substring(s.IndexOf(" ") + 1);
                        int[] stats = new int[6];
                        for (int i = 0; i < 6; i++)
                        {
                            stats[i] = int.Parse(s.Substring(0, s.IndexOf(" ")));
                            s = s.Substring(s.IndexOf(" ") + 1);
                        }
                        List<double> sonderheight = new List<double>();
                        List<double> sonderweight = new List<double>();
                        List<int[]> sonderstats = new List<int[]>();
                        List<string> sondertype1 = new List<string>();
                        List<string> sondertype2 = new List<string>();
                        while (s.Contains(" "))
                        {
                            if (s.Length > 0 && s.Substring(0, s.IndexOf(" ")).EndsWith("m"))
                            {
                                double h = Convert.ToDouble((s.Substring(0, s.IndexOf(" ") - 1)), NumberFormatInfo.InvariantInfo);

                                s = s.Substring(s.IndexOf(" ") + 1);
                                double w = Convert.ToDouble((s.Substring(0, s.IndexOf(" ") - 2)), NumberFormatInfo.InvariantInfo);

                                s = s.Substring(s.IndexOf(" ") + 1);
                                sonderheight.Add(h);
                                sonderweight.Add(w);
                            }
                            try
                            {
                                int test = int.Parse(s.Substring(0, s.IndexOf(" ")));
                            }
                            catch (Exception)
                            {
                                sondertype1.Add(s.Substring(0, s.IndexOf(" ")));
                                s = s.Substring(s.IndexOf(" ") + 1);
                                sondertype2.Add(s.Substring(0, s.IndexOf(" ")));
                                s = s.Substring(s.IndexOf(" ") + 1);
                            }
                            if (s.Length > 0 && s.Substring(0, s.IndexOf(" ")).EndsWith("m"))
                            {
                                double h = Convert.ToDouble((s.Substring(0, s.IndexOf(" ") - 1)), NumberFormatInfo.InvariantInfo);

                                s = s.Substring(s.IndexOf(" ") + 1);
                                double w = Convert.ToDouble((s.Substring(0, s.IndexOf(" ") - 2)), NumberFormatInfo.InvariantInfo);

                                s = s.Substring(s.IndexOf(" ") + 1);
                                sonderheight.Add(h);
                                sonderweight.Add(w);
                            }
                            try
                            {
                                if (s.Contains(" "))
                                {
                                    int[] statstemp = new int[6];
                                    for (int i = 0; i < 6; i++)
                                    {
                                        statstemp[i] = int.Parse(s.Substring(0, s.IndexOf(" ")));
                                        s = s.Substring(s.IndexOf(" ") + 1);
                                    }
                                    sonderstats.Add((int[])statstemp.Clone());
                                }
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        sondertype1.Reverse();
                        sondertype2.Reverse();
                        Pokemon poke = new Pokemon(dexnbr, name, type1, type2, height, weight, stats, sonderheight, sonderweight, sondertype1, sondertype2, sonderstats);
                        matcherPokemon.Add(name, poke);
                        donePokemon.Add(name, false);
                    }
                    matcherPokemon.Remove("Volcanion");
                    matcherPokemon.Remove("Hoopa");
                    donePokemon.Remove("Volcanion");
                    donePokemon.Remove("Hoopa");
                }
            }
        }

        Dictionary<string, Attacks> matcherAttacks;

        private void initAttacks()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (d.Exists)
            {
                FileInfo f = new FileInfo(System.Environment.CurrentDirectory + "\\Attacks.data");
                if (f.Exists)
                {
                    matcherAttacks = new Dictionary<string, Attacks>();
                    StreamReader sr = new StreamReader(f.OpenRead());
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        string[] sp = s.Split('~');
                        if (sp.Length > 3)
                        {
                            matcherAttacks.Add(sp[0], new Attacks(sp[0], int.Parse(sp[1]), int.Parse(sp[2]), sp[3]));
                        }
                    }
                    sr.Close();
                }
            }
        }

        Dictionary<string, Items> matcherItems;

        private void initItems()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (d.Exists)
            {
                FileInfo f = new FileInfo(System.Environment.CurrentDirectory + "\\Items.data");
                if (f.Exists)
                {
                    matcherItems = new Dictionary<string, Items>();
                    StreamReader sr = new StreamReader(f.OpenRead());
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        string[] sp = s.Split('~');
                        if (sp.Length > 1)
                        {
                            if (matcherItems.ContainsKey(sp[0]))
                            {
                                if (!matcherItems[sp[0]].description.Equals(sp[1]))
                                {
                                    matcherItems[sp[0]].description = (string)sp[1].Clone();
                                }

                            }
                            else
                            {
                                matcherItems.Add(sp[0], new Items(sp[0], sp[1]));
                            }
                        }
                    }
                    sr.Close();
                }
            }
            matcherItems.Add("Damage Berry", new Items("Damage Berry", "This is an invented berry, to make it easy to calc, how much an attack with the right berry would've done."));
            matcherItems.Add("Damage Plate", new Items("Damage Plate", "This is an invented plate, to make it easy to calc, how much an attack with the right plate would've done."));
            matcherItems.Add("Damage Gem", new Items("Damage Gem", "This is an invented gem, to make it easy to calc, how much an attack with the right gem would've done."));
        }

        Dictionary<string, Natures> matcherNatures;

        private void initNatures()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (d.Exists)
            {
                FileInfo f = new FileInfo(System.Environment.CurrentDirectory + "\\Natures.data");
                if (f.Exists)
                {
                    matcherNatures = new Dictionary<string, Natures>();
                    StreamReader sr = new StreamReader(f.OpenRead());
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        string[] sp = s.Split('~');
                        if (sp.Length > 2)
                        {
                            matcherNatures.Add(sp[0], new Natures(sp[0], sp[1], sp[2]));
                        }
                    }
                    sr.Close();
                }
            }
        }

        Dictionary<string, string> matcherAbilities;

        private void initAbilities()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (d.Exists)
            {
                FileInfo f = new FileInfo(System.Environment.CurrentDirectory + "\\Abilities.data");
                if (f.Exists)
                {
                    matcherAbilities = new Dictionary<string, string>();
                    StreamReader sr = new StreamReader(f.OpenRead());
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        s.Trim();
                        if (!s.Equals(""))
                        {
                            matcherAbilities.Add(s, s);
                        }
                    }
                    sr.Close();
                }
            }
        }

        int countLevel = 1;

        string loesung = "";

        Dictionary<string, bool> donePokemon;

        private void randomPokemon()
        {
            Random r = new Random();
            int a = r.Next(matcherPokemon.Count);
            int count = 0;
            foreach(string k in matcherPokemon.Keys)
            {
                if (a == count)
                {
                    string k2 = k;
                    while (donePokemon[k2] == true)
                    {
                        a = r.Next(matcherPokemon.Count);
                        count = 0;
                        foreach (string k3 in matcherPokemon.Keys)
                        {
                            count++;
                            if (a == count)
                            {
                                k2 = k3;
                                break;
                            }
                        }
                        
                    }
                    switch (mode2)
                    {
                        case "Pokemon":
                            versionGif(k2);
                            break;
                        default:
                            versionStats(k2);
                            break;
                    }
                    donePokemon[k2] = true;                    
                    loesung = k2;
                    break;
                }
                count++;
            }
        }

        private void versionGif(string k2)
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "\\gifs\\");
            if (!d.Exists)
            {
                MessageBox.Show("Entpacken Sie vorher die gifs.zip und fügen Sie den gifs Ordner in: " + System.Environment.CurrentDirectory + " ein");
                this.Close();
            }

            string s = k2;
            s = s.Replace("♀", "f");
            s = s.Replace("♂", "m");
            s = s.Replace("é", "e");
            using (var fs = new System.IO.FileStream(System.Environment.CurrentDirectory + "\\gifs\\" + s + ".gif", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                var ms = new System.IO.MemoryStream();
                fs.CopyTo(ms);
                ms.Position = 0;                               // <=== here
                if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
                pictureBox1.Image = Image.FromStream(ms);
            }
        }

        private void versionStats(string k2)
        {
            List<string> selector = new List<string>();
            for (int i = 0; i < matcherPokemon[k2].stats.Length; i++)
            {
                selector.Add(matcherPokemon[k2].stats[i] + "");
            }
            textBox2.Lines = selector.ToArray();
        }

        string mode;

        private void Form2_Load(object sender, EventArgs e)
        {
            init();    
            switch (mode)
            {
                case "text":
                    textBox1.Visible = true;
                    break;
                default:
                    button1.Visible = true;
                    button2.Visible = true;
                    button3.Visible = true;
                    button4.Visible = true;
                    break;
            }
            switch(mode2)
            {
                case "Pokemon":
                    pictureBox1.Visible = true;
                    break;
                default:
                    label5.Visible = true;
                    label6.Visible = true;
                    label7.Visible = true;
                    label8.Visible = true;
                    label9.Visible = true;
                    label10.Visible = true;
                    textBox2.Visible = true;
                    break;
            }
            randomPokemon();
            if (mode == null || !mode.Equals("text"))
            {
                buttonFill();
            }
        }

        private void buttonFill()
        {
            Random r = new Random();
            int a = r.Next(4);
            switch (a)
            {
                case 0:
                    button1.Text = loesung;
                    button2.Text = "";
                    button3.Text = "";
                    button4.Text = "";
                    break;
                case 1:
                    button1.Text = "";
                    button2.Text = loesung;
                    button3.Text = "";
                    button4.Text = "";
                    break;
                case 2:
                    button1.Text = "";
                    button2.Text = "";
                    button3.Text = loesung;
                    button4.Text = "";
                    break;
                case 3:
                    button1.Text = "";
                    button2.Text = "";
                    button3.Text = "";
                    button4.Text = loesung;
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
                a = r.Next(matcherPokemon.Count);
                int count = 0;
                foreach (string k in matcherPokemon.Keys)
                {
                    if (a == count)
                    {
                        string k2 = k;
                        while (k2.Equals(loesung) || k2.Equals(button1.Text) || k2.Equals(button2.Text) || k2.Equals(button3.Text) || k2.Equals(button4.Text))
                        {
                            a = r.Next(matcherPokemon.Count);
                            count = 0;
                            foreach (string k3 in matcherPokemon.Keys)
                            {
                                count++;
                                if (a == count)
                                {
                                    k2 = k3;
                                    break;
                                }
                            }

                        }
                        if (button1.Text.Equals(""))
                        {
                            button1.Text = k2;
                            break;
                        }
                        if (button2.Text.Equals(""))
                        {
                            button2.Text = k2;
                            break;
                        }
                        if (button3.Text.Equals(""))
                        {
                            button3.Text = k2;
                            break;
                        }
                        if (button4.Text.Equals(""))
                        {
                            button4.Text = k2;
                            break;
                        }
                    }
                    count++;
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (textBox1.Text.ToLower().Equals(loesung.ToLower()))
                {
                    MessageBox.Show("Correct! It was " + loesung);
                    label4.Text = (int.Parse(label4.Text) + 1)+"";
                }
                else
                {
                    MessageBox.Show("Wrong! It was " + loesung);
                    label3.Text = (int.Parse(label3.Text) + 1) + "";
                }
                if (countLevel == 724)
                {
                    MessageBox.Show("Congratulations! You beat the game and had "+label4.Text+" correct and "+label3.Text+" wrong!");
                    countLevel = 0;
                    label3.Text = "0";
                    label4.Text = "0";
                    List<string> temp = new List<string>();
                    foreach(string s in donePokemon.Keys)
                    {
                        temp.Add(s);
                    }
                    foreach (string s in temp)
                    {
                        donePokemon[s] = false;
                    }
                }
                randomPokemon();
                textBox1.Text = "";
                countLevel++;
                label2.Text = countLevel + "/724";
            }
        }

        private bool compareLoesung(string s)
        {
            Pokemon p1 = matcherPokemon[loesung];
            Pokemon p2 = matcherPokemon[s];
            bool b = true;
            for (int i = 0; i < p1.stats.Length; i++)
            {
                if (p1.stats[i] != p2.stats[i])
                {
                    b = false;
                }
            }
            return b;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool hard = false;
            switch (mode2)
            {
                case "Pokemon":
                    if (button1.Text.Equals(loesung))
                    {
                        hard = true;
                    }
                    break;
                default:
                    hard = compareLoesung(button1.Text);
                    break;

            }
            if (hard)
            {
                MessageBox.Show("Correct! It was " + loesung);
                label4.Text = (int.Parse(label4.Text) + 1) + "";
            }
            else
            {
                MessageBox.Show("Wrong! It was " + loesung);
                label3.Text = (int.Parse(label3.Text) + 1) + "";
            }
                
                if (countLevel == 724)
                {
                    MessageBox.Show("Congratulations! You beat the game and had " + label4.Text + " correct and " + label3.Text + " wrong!");
                    countLevel = 0;
                    label3.Text = "0";
                    label4.Text = "0";
                    List<string> temp = new List<string>();
                    foreach (string s in donePokemon.Keys)
                    {
                        temp.Add(s);
                    }
                    foreach (string s in temp)
                    {
                        donePokemon[s] = false;
                    }
                }
                randomPokemon();
                buttonFill();
                countLevel++;
                label2.Text = countLevel + "/724";
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool hard = false;
            switch (mode2)
            {
                case "Pokemon":
                    if (button3.Text.Equals(loesung))
                    {
                        hard = true;
                    }
                    break;
                default:
                    hard = compareLoesung(button3.Text);
                    break;

            }
            if (hard)
            {
                MessageBox.Show("Correct! It was " + loesung);
                label4.Text = (int.Parse(label4.Text) + 1) + "";
            }
            else
            {
                MessageBox.Show("Wrong! It was " + loesung);
                label3.Text = (int.Parse(label3.Text) + 1) + "";
            }
            if (countLevel == 724)
            {
                MessageBox.Show("Congratulations! You beat the game and had " + label4.Text + " correct and " + label3.Text + " wrong!");
                countLevel = 0;
                label3.Text = "0";
                label4.Text = "0";
                List<string> temp = new List<string>();
                foreach (string s in donePokemon.Keys)
                {
                    temp.Add(s);
                }
                foreach (string s in temp)
                {
                    donePokemon[s] = false;
                }
            }
            randomPokemon();
            buttonFill();
            countLevel++;
            label2.Text = countLevel + "/724";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool hard = false;
            switch (mode2)
            {
                case "Pokemon":
                    if (button2.Text.Equals(loesung))
                    {
                        hard = true;
                    }
                    break;
                default:
                    hard = compareLoesung(button2.Text);
                    break;

            }
            if (hard)
            {
                MessageBox.Show("Correct! It was " + loesung);
                label4.Text = (int.Parse(label4.Text) + 1) + "";
            }
            else
            {
                MessageBox.Show("Wrong! It was " + loesung);
                label3.Text = (int.Parse(label3.Text) + 1) + "";
            }
            if (countLevel == 724)
            {
                MessageBox.Show("Congratulations! You beat the game and had " + label4.Text + " correct and " + label3.Text + " wrong!");
                countLevel = 0;
                label3.Text = "0";
                label4.Text = "0";
                List<string> temp = new List<string>();
                foreach (string s in donePokemon.Keys)
                {
                    temp.Add(s);
                }
                foreach (string s in temp)
                {
                    donePokemon[s] = false;
                }
            }
            randomPokemon();
            buttonFill();
            countLevel++;
            label2.Text = countLevel + "/724";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool hard = false;
            switch (mode2)
            {
                case "Pokemon":
                    if (button4.Text.Equals(loesung))
                    {
                        hard = true;
                    }
                    break;
                default:
                    hard = compareLoesung(button4.Text);
                    break;

            }
            if (hard)
            {
                MessageBox.Show("Correct! It was " + loesung);
                label4.Text = (int.Parse(label4.Text) + 1) + "";
            }
            else
            {
                MessageBox.Show("Wrong! It was " + loesung);
                label3.Text = (int.Parse(label3.Text) + 1) + "";
            }
            if (countLevel == 724)
            {
                MessageBox.Show("Congratulations! You beat the game and had " + label4.Text + " correct and " + label3.Text + " wrong!");
                countLevel = 0;
                label3.Text = "0";
                label4.Text = "0";
                List<string> temp = new List<string>();
                foreach (string s in donePokemon.Keys)
                {
                    temp.Add(s);
                }
                foreach (string s in temp)
                {
                    donePokemon[s] = false;
                }
            }
            randomPokemon();
            buttonFill();
            countLevel++;
            label2.Text = countLevel + "/724";
        }

        
        
    }
}
