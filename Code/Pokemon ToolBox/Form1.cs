﻿using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Pokemon_ToolBox
{
    public partial class Form1 : Form
    {
        String Calcstart = "";

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(String s)
        {
            Calcstart = s;
            InitializeComponent();
        }

        Thread t1;

        private void button1_Click(object sender, EventArgs e)
        {
            t1 = new Thread(this.getDatafromSerebii);
            t1.Start();
        }

        private void getDatafromSerebii()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (!d.Exists)
            {
                d.Create();
            }
            FileInfo f;
            StreamWriter sw;
            WebClient wClient = new WebClient();
            progressBar1.BeginInvoke((MethodInvoker)delegate
            {
                progressBar1.Maximum = 999;
                progressBar1.Value = 0;
            });
            for (int i = 1; i < 1000; i++)
            {
                bool b = true;

                string s = "";
                try
                {
                    if (i < 10)
                    {
                        s = wClient.DownloadString("http://www.serebii.net/pokedex-xy/00" + i + ".shtml");
                    }
                    else if (i < 100)
                    {
                        s = wClient.DownloadString("http://www.serebii.net/pokedex-xy/0" + i + ".shtml");
                    }
                    else
                    {
                        s = wClient.DownloadString("http://www.serebii.net/pokedex-xy/" + i + ".shtml");
                    }
                }
                catch (WebException)
                {
                    b = false;
                }
                if (b)
                {
                    f = new FileInfo(System.Environment.CurrentDirectory + "\\" + i + ".log");
                    if (!f.Exists)
                    {
                        FileStream fs = f.Create();
                        fs.Close();
                    }
                    else
                    {
                        f.Delete();
                        FileStream fs = f.Create();
                        fs.Close();
                    }

                    sw = f.AppendText();
                    sw.WriteLine(s);
                    sw.Close();
                }
                progressBar1.BeginInvoke((MethodInvoker)delegate
                {
                    progressBar1.Value++;
                });
            }
            MessageBox.Show("Finished!");
            progressBar1.BeginInvoke((MethodInvoker)delegate
            {
                progressBar1.Value = 0;
                progressBar1.Maximum = 100;
            });
        }

        Thread t2;

        private void button2_Click(object sender, EventArgs e)
        {
            t2 = new Thread(this.buildData);
            t2.Start();
        }

        private void buildData()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            Dictionary<int, List<string>> dict = new Dictionary<int, List<string>>();
            if (!d.Exists)
            {
                MessageBox.Show("First use Retrieve Data Pokemon!");
            }
            else
            {
                progressBar1.BeginInvoke((MethodInvoker)delegate
                {
                    progressBar1.Maximum = 999;
                    progressBar1.Value = 0;
                });
                FileInfo f;
                for (int i = 1; i < 999; i++)
                {
                    f = new FileInfo(System.Environment.CurrentDirectory + "\\" + i + ".log");
                    if (f.Exists)
                    {
                        StreamReader sr = new StreamReader(f.OpenRead());
                        int count = 0;
                        int a = 0;
                        int b = 0;
                        Dictionary<int, bool> check = new Dictionary<int, bool>();
                        bool type = true;
                        while (!sr.EndOfStream)
                        {
                            count++;
                            string s = sr.ReadLine();
                            if (count == 6)
                            {
                                dict.Add(i, new List<string>());
                                dict[i].Add(s.Substring(7, s.IndexOf(" ") - 7));
                            }
                            if (s.Contains("shtml\"><img src=\"/pokedex-bw/type/"))
                            {
                                if (type)
                                {
                                    type = false;
                                    string temp = s.Substring(s.IndexOf("shtml\"><img src=\"/pokedex-bw/type/") + 34);
                                    dict[i].Add(temp.Substring(0, temp.IndexOf(".gif")));
                                    if (temp.Contains("shtml\"><img src=\"/pokedex-bw/type/"))
                                    {
                                        string temp2 = temp.Substring(temp.IndexOf("shtml\"><img src=\"/pokedex-bw/type/") + 34);
                                        dict[i].Add(temp2.Substring(0, temp2.IndexOf(".gif")));
                                    }
                                    else
                                    {
                                        dict[i].Add("-");
                                    }
                                }
                                else
                                {
                                    type = true;
                                }
                            }

                            if (s.Contains("<td class=\"foo\">Weight</td>"))
                            {
                                a = count + 7;
                                b = a + 2;
                            }

                            if (count == a || count == b)
                            {
                                while (s.IndexOf("\t") == 0)
                                {
                                    s = s.Substring(1);
                                }
                                dict[i].Add(s.Substring(0, s.IndexOf("</td>")));
                            }

                            if (s.Contains("<tr><td colspan=\"2\" width=\"14%\" class=\"fooinfo\">Base Stats - Total:"))
                            {
                                check.Add(count + 1, true);
                                check.Add(count + 2, true);
                                check.Add(count + 3, true);
                                check.Add(count + 4, true);
                                check.Add(count + 5, true);
                                check.Add(count + 6, true);
                            }
                            if (check.ContainsKey(count))
                            {
                                string temp = s.Substring(35);
                                dict[i].Add(temp.Substring(0, temp.IndexOf("</")));
                            }
                        }
                        sr.Close();
                    }
                    progressBar1.BeginInvoke((MethodInvoker)delegate
                    {
                        progressBar1.Value++;
                    });
                }
                f = new FileInfo(System.Environment.CurrentDirectory + "\\Pokemon.data");
                if (!f.Exists)
                {
                    FileStream fs = f.Create();
                    fs.Close();
                }
                else
                {
                    f.Delete();
                    FileStream fs = f.Create();
                    fs.Close();
                }
                StreamWriter sw = f.AppendText();
                foreach (KeyValuePair<int, List<String>> t in dict)
                {
                    string s = "";
                    foreach (string temp in t.Value)
                    {
                        s = s + " " + temp;
                    }
                    sw.WriteLine(t.Key + "  " + s + " ");
                }
                sw.Close();
                MessageBox.Show("Finished!");
                progressBar1.BeginInvoke((MethodInvoker)delegate
                {
                    progressBar1.Value = 0;
                    progressBar1.Maximum = 100;
                });
            }


        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            close();
        }

        private void close()
        {
            if (t1 != null)
            {
                if (t1.IsAlive)
                {
                    t1.Abort();
                }
            }
            if (t2 != null)
            {
                if (t2.IsAlive)
                {
                    t2.Abort();
                }
            }
            if (t3 != null)
            {
                if (t3.IsAlive)
                {
                    t3.Abort();
                }
            }
            if (t4 != null)
            {
                if (t4.IsAlive)
                {
                    t4.Abort();
                }
            }
            if (t5 != null)
            {
                if (t5.IsAlive)
                {
                    t5.Abort();
                }
            }
            if (t6 != null)
            {
                if (t6.IsAlive)
                {
                    t6.Abort();
                }
            }
            if (t7 != null)
            {
                if (t7.IsAlive)
                {
                    t7.Abort();
                }
            }
            if (t8 != null)
            {
                if (t8.IsAlive)
                {
                    t8.Abort();
                }
            }
            if (t9 != null)
            {
                if (t9.IsAlive)
                {
                    t9.Abort();
                }
            }
            if (t10 != null)
            {
                if (t10.IsAlive)
                {
                    t10.Abort();
                }
            }
            if (t11 != null)
            {
                if (t11.IsAlive)
                {
                    t11.Abort();
                }
            }
            if (t12 != null)
            {
                if (t12.IsAlive)
                {
                    t12.Abort();
                }
            }
            if (t13 != null)
            {
                if (t13.IsAlive)
                {
                    t13.Abort();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");

            if (d.Exists)
            {
                d = new DirectoryInfo(System.Environment.CurrentDirectory + "\\gifs");
                FileInfo f1 = new FileInfo(System.Environment.CurrentDirectory + "\\Pokemon.data");
                FileInfo f2 = new FileInfo(System.Environment.CurrentDirectory + "\\attacks.data");
                FileInfo f3 = new FileInfo(System.Environment.CurrentDirectory + "\\Items.data");
                FileInfo f4 = new FileInfo(System.Environment.CurrentDirectory + "\\Natures.data");
                FileInfo f5 = new FileInfo(System.Environment.CurrentDirectory + "\\Abilities.data");
                FileInfo f6 = new FileInfo(System.Environment.CurrentDirectory + "\\gifs.zip");
                if (f1.Exists)
                {
                    if (f2.Exists)
                    {
                        if (f3.Exists)
                        {
                            if (f4.Exists)
                            {
                                if (f5.Exists)
                                {
                                    if (d.Exists)
                                    {
                                        itemCheck = false;
                                        init();
                                        setup();
                                        stats();
                                        gifchange(0);
                                        gifchange(1);
                                        extraformset(0);
                                        extraformset(1);
                                        button11.Visible = false;
                                        itemCheck = true;
                                        if (!Calcstart.Equals(""))
                                        {
                                            importcalc(Calcstart);
                                        }
                                    }
                                    else if (f6.Exists)
                                    {
                                        try
                                        {
                                            using (ZipFile zip = ZipFile.Read(System.Environment.CurrentDirectory + "\\gifs.zip"))
                                            {
                                                zip.ExtractAll(System.Environment.CurrentDirectory + "\\");
                                            }
                                        }
                                        catch (Exception)
                                        { }
                                        itemCheck = false;
                                        init();
                                        setup();
                                        stats();
                                        gifchange(0);
                                        gifchange(1);
                                        extraformset(0);
                                        extraformset(1);
                                        button11.Visible = false;
                                        itemCheck = true;
                                        if (!Calcstart.Equals(""))
                                        {
                                            importcalc(Calcstart);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
                    }
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

        private void setup()
        {
            checkBox2.Checked = false;
            if (comboBox1.Items.Count == 0 && comboBox3.Items.Count == 0)
            {
                if (checkBox6.Checked)
                {
                    foreach (KeyValuePair<string, Pokemon> pokemon in matcherPokemon.OrderBy(key => key.Key))
                    {
                        comboBox1.Items.Add(pokemon.Key);
                        comboBox3.Items.Add(pokemon.Key);
                    }
                }
                else
                {
                    foreach (string s in matcherPokemon.Keys)
                    {
                        comboBox1.Items.Add(s);
                        comboBox3.Items.Add(s);
                    }
                }
            }
            if (comboBox13.SelectedItem == null)
            {
                comboBox13.SelectedItem = "normal";
            }
            comboBox13.Sorted = true;

            if (comboBox2.Items.Count == 0)
            {
                foreach (KeyValuePair<string, Attacks> attacks in matcherAttacks.OrderBy(key => key.Key))
                {
                    comboBox2.Items.Add(attacks.Key);
                }
            }

            if (comboBox4.Items.Count == 0 && comboBox5.Items.Count == 0)
            {
                foreach (KeyValuePair<string, Items> items in matcherItems.OrderBy(key => key.Key))
                {
                    comboBox4.Items.Add(items.Key);
                    comboBox5.Items.Add(items.Key);
                }
            }

            if (comboBox8.Items.Count == 0 && comboBox9.Items.Count == 0)
            {
                foreach (KeyValuePair<string, Natures> natures in matcherNatures.OrderBy(key => key.Key))
                {
                    comboBox8.Items.Add(natures.Key);
                    comboBox9.Items.Add(natures.Key);
                }
            }

            if (comboBox6.Items.Count == 0 && comboBox7.Items.Count == 0)
            {
                foreach (KeyValuePair<string, string> abilities in matcherAbilities.OrderBy(key => key.Key))
                {
                    if (!abilities.Key.Equals("Abilitydex"))
                    {
                        comboBox6.Items.Add(abilities.Key);
                        comboBox7.Items.Add(abilities.Key);
                    }
                }
            }

            setTypeChart();
        }

        Dictionary<string, Dictionary<string, double>> Typechart;

        private void setTypeChart()
        {
            Typechart = new Dictionary<string, Dictionary<string, double>>();
            Typechart.Add("normal", new Dictionary<string, double>());
            Typechart["normal"].Add("ghost", 0);
            Typechart["normal"].Add("fighting", 2);
            Typechart.Add("fighting", new Dictionary<string, double>());
            Typechart["fighting"].Add("flying", 2);
            Typechart["fighting"].Add("rock", 0.5);
            Typechart["fighting"].Add("bug", 0.5);
            Typechart["fighting"].Add("psychic", 2);
            Typechart["fighting"].Add("dark", 0.5);
            Typechart["fighting"].Add("fairy", 2);
            Typechart.Add("flying", new Dictionary<string, double>());
            Typechart["flying"].Add("fighting", 0.5);
            Typechart["flying"].Add("ground", 0);
            Typechart["flying"].Add("rock", 2);
            Typechart["flying"].Add("bug", 0.5);
            Typechart["flying"].Add("grass", 0.5);
            Typechart["flying"].Add("electric", 2);
            Typechart["flying"].Add("ice", 2);
            Typechart.Add("poison", new Dictionary<string, double>());
            Typechart["poison"].Add("fighting", 0.5);
            Typechart["poison"].Add("poison", 0.5);
            Typechart["poison"].Add("ground", 2);
            Typechart["poison"].Add("bug", 0.5);
            Typechart["poison"].Add("grass", 0.5);
            Typechart["poison"].Add("psychic", 2);
            Typechart["poison"].Add("fairy", 0.5);
            Typechart.Add("ground", new Dictionary<string, double>());
            Typechart["ground"].Add("poison", 0.5);
            Typechart["ground"].Add("rock", 0.5);
            Typechart["ground"].Add("water", 2);
            Typechart["ground"].Add("grass", 2);
            Typechart["ground"].Add("electric", 0);
            Typechart["ground"].Add("ice", 2);
            Typechart.Add("rock", new Dictionary<string, double>());
            Typechart["rock"].Add("normal", 0.5);
            Typechart["rock"].Add("fighting", 2);
            Typechart["rock"].Add("flying", 0.5);
            Typechart["rock"].Add("poison", 0.5);
            Typechart["rock"].Add("ground", 2);
            Typechart["rock"].Add("steel", 2);
            Typechart["rock"].Add("fire", 0.5);
            Typechart["rock"].Add("water", 2);
            Typechart["rock"].Add("grass", 2);
            Typechart.Add("bug", new Dictionary<string, double>());
            Typechart["bug"].Add("fighting", 0.5);
            Typechart["bug"].Add("flying", 2);
            Typechart["bug"].Add("ground", 0.5);
            Typechart["bug"].Add("rock", 2);
            Typechart["bug"].Add("fire", 2);
            Typechart["bug"].Add("grass", 0.5);
            Typechart.Add("ghost", new Dictionary<string, double>());
            Typechart["ghost"].Add("normal", 0);
            Typechart["ghost"].Add("fighting", 0);
            Typechart["ghost"].Add("posion", 0.5);
            Typechart["ghost"].Add("bug", 0.5);
            Typechart["ghost"].Add("ghost", 2);
            Typechart["ghost"].Add("dark", 2);
            Typechart.Add("steel", new Dictionary<string, double>());
            Typechart["steel"].Add("normal", 0.5);
            Typechart["steel"].Add("fighting", 2);
            Typechart["steel"].Add("flying", 0.5);
            Typechart["steel"].Add("poison", 0);
            Typechart["steel"].Add("ground", 2);
            Typechart["steel"].Add("rock", 0.5);
            Typechart["steel"].Add("bug", 0.5);
            Typechart["steel"].Add("steel", 0.5);
            Typechart["steel"].Add("fire", 2);
            Typechart["steel"].Add("grass", 0.5);
            Typechart["steel"].Add("psychic", 0.5);
            Typechart["steel"].Add("ice", 0.5);
            Typechart["steel"].Add("dragon", 0.5);
            Typechart["steel"].Add("fairy", 0.5);
            Typechart.Add("fire", new Dictionary<string, double>());
            Typechart["fire"].Add("ground", 2);
            Typechart["fire"].Add("rock", 2);
            Typechart["fire"].Add("bug", 0.5);
            Typechart["fire"].Add("steel", 0.5);
            Typechart["fire"].Add("fire", 0.5);
            Typechart["fire"].Add("water", 2);
            Typechart["fire"].Add("grass", 0.5);
            Typechart["fire"].Add("ice", 0.5);
            Typechart["fire"].Add("fairy", 0.5);
            Typechart.Add("water", new Dictionary<string, double>());
            Typechart["water"].Add("steel", 0.5);
            Typechart["water"].Add("fire", 0.5);
            Typechart["water"].Add("water", 0.5);
            Typechart["water"].Add("grass", 2);
            Typechart["water"].Add("electric", 2);
            Typechart["water"].Add("ice", 0.5);
            Typechart.Add("grass", new Dictionary<string, double>());
            Typechart["grass"].Add("flying", 2);
            Typechart["grass"].Add("poison", 2);
            Typechart["grass"].Add("ground", 0.5);
            Typechart["grass"].Add("bug", 2);
            Typechart["grass"].Add("fire", 2);
            Typechart["grass"].Add("water", 0.5);
            Typechart["grass"].Add("grass", 0.5);
            Typechart["grass"].Add("electric", 0.5);
            Typechart["grass"].Add("ice", 2);
            Typechart.Add("electric", new Dictionary<string, double>());
            Typechart["electric"].Add("flying", 0.5);
            Typechart["electric"].Add("ground", 2);
            Typechart["electric"].Add("steel", 0.5);
            Typechart["electric"].Add("electric", 0.5);
            Typechart.Add("psychic", new Dictionary<string, double>());
            Typechart["psychic"].Add("fighting", 0.5);
            Typechart["psychic"].Add("bug", 2);
            Typechart["psychic"].Add("ghost", 2);
            Typechart["psychic"].Add("psychic", 0.5);
            Typechart["psychic"].Add("dark", 2);
            Typechart.Add("ice", new Dictionary<string, double>());
            Typechart["ice"].Add("fighting", 2);
            Typechart["ice"].Add("rock", 2);
            Typechart["ice"].Add("steel", 2);
            Typechart["ice"].Add("fire", 2);
            Typechart["ice"].Add("ice", 0.5);
            Typechart.Add("dragon", new Dictionary<string, double>());
            Typechart["dragon"].Add("fire", 0.5);
            Typechart["dragon"].Add("water", 0.5);
            Typechart["dragon"].Add("grass", 0.5);
            Typechart["dragon"].Add("electric", 0.5);
            Typechart["dragon"].Add("ice", 2);
            Typechart["dragon"].Add("dragon", 2);
            Typechart["dragon"].Add("fairy", 2);
            Typechart.Add("dark", new Dictionary<string, double>());
            Typechart["dark"].Add("fighting", 2);
            Typechart["dark"].Add("bug", 2);
            Typechart["dark"].Add("ghost", 0.5);
            Typechart["dark"].Add("psychic", 0);
            Typechart["dark"].Add("dark", 0 - 5);
            Typechart["dark"].Add("fairy", 2);
            Typechart.Add("fairy", new Dictionary<string, double>());
            Typechart["fairy"].Add("fighting", 0.5);
            Typechart["fairy"].Add("poison", 2);
            Typechart["fairy"].Add("bug", 0.5);
            Typechart["fairy"].Add("steel", 2);
            Typechart["fairy"].Add("dragon", 0);
            Typechart["fairy"].Add("dark", 0.5);
        }
        //Pokemon A Labelnummern
        //KP 19 ATK 20 DEF 21 SAtk 22 SDef 23 Speed 24

        //Pokemon B Labelnummern
        //KP 30 ATK 29 DEF 28 SAtk 27 SDef 26 Speed 25
        double Wesena = 1;
        double Wesend = 1;
        double Wesensa = 1;
        double Wesensd = 1;
        double Wesenspe = 1;
        double BWesena = 1;
        double BWesend = 1;
        double BWesensa = 1;
        double BWesensd = 1;
        double BWesenspe = 1;
        private void stats()
        {
            try
            {
                try
                {
                    int Iv1 = 31; int Iv2 = 31; int Iv3 = 31; int Iv4 = 31; int Iv5 = 31; int Iv6 = 31; int BIv1 = 31; int BIv2 = 31; int BIv3 = 31; int BIv4 = 31; int BIv5 = 31; int BIv6 = 31; try { Iv1 = int.Parse(textBox26.Text); }
                    catch (FormatException) { } try { Iv2 = int.Parse(textBox25.Text); }
                    catch (FormatException) { } try { Iv3 = int.Parse(textBox24.Text); }
                    catch (FormatException) { } try { Iv4 = int.Parse(textBox23.Text); }
                    catch (FormatException) { } try { Iv5 = int.Parse(textBox22.Text); }
                    catch (FormatException) { } try { Iv6 = int.Parse(textBox21.Text); }
                    catch (FormatException) { } try { BIv1 = int.Parse(textBox20.Text); }
                    catch (FormatException) { } try { BIv2 = int.Parse(textBox19.Text); }
                    catch (FormatException) { } try { BIv3 = int.Parse(textBox18.Text); }
                    catch (FormatException) { } try { BIv4 = int.Parse(textBox17.Text); }
                    catch (FormatException) { } try { BIv5 = int.Parse(textBox16.Text); }
                    catch (FormatException) { } try { BIv6 = int.Parse(textBox15.Text); }
                    catch (FormatException) { }
                    Wesena *= multiplier(textBox34);
                    Wesend *= multiplier(textBox35);
                    Wesensa *= multiplier(textBox36);
                    Wesensd *= multiplier(textBox37);
                    Wesenspe *= multiplier(textBox38);
                    BWesena *= multiplier(textBox28);
                    BWesend *= multiplier(textBox29);
                    BWesensa *= multiplier(textBox30);
                    BWesensd *= multiplier(textBox31);
                    BWesenspe *= multiplier(textBox32);
                    if (comboBox10.SelectedItem == null || comboBox10.SelectedItem.ToString().Equals("Normal"))
                    {
                        Pokemon pa;
                        try
                        {
                            pa = matcherPokemon[comboBox1.SelectedItem.ToString()];
                        }
                        catch (NullReferenceException)
                        {
                            if (matcherPokemon.ContainsKey(comboBox1.Text))
                            {
                                pa = matcherPokemon[comboBox1.Text];
                            }
                            else
                            {
                                pa = matcherPokemon["Bulbasaur"];
                            }
                        }
                        label51.Text = pa.stats[0]+"";
                        label50.Text = pa.stats[1]+"";
                        label49.Text = pa.stats[2]+"";
                        label48.Text = pa.stats[3]+ "";
                        label47.Text = pa.stats[4]+ "";
                        label4.Text = pa.stats[5] + "";

                        int fp = (int.Parse(textBox1.Text));
                        double level = (double.Parse(textBox13.Text));
                        int wert;
                        if (!pa.name.Equals("Shedinja"))
                        {
                            wert = (int)(((2 * pa.stats[0] + Iv1 + fp / 4) * (double)(level / 100)) + 10 + level);
                        }
                        else
                        {
                            wert = 1;
                        }
                        label19.Text = wert + "";
                        fp = (int.Parse(textBox2.Text));
                        wert = (int)(((2 * pa.stats[1] + Iv2 + fp / 4) * (level / 100) + 5) * Wesena);
                        label20.Text = wert + "";
                        fp = (int.Parse(textBox3.Text));
                        wert = (int)(((2 * pa.stats[2] + Iv3 + fp / 4) * (level / 100) + 5) * Wesend);
                        label21.Text = wert + "";
                        fp = (int.Parse(textBox4.Text));
                        wert = (int)(((2 * pa.stats[3] + Iv4 + fp / 4) * (level / 100) + 5) * Wesensa);
                        label22.Text = wert + "";
                        fp = (int.Parse(textBox5.Text));
                        wert = (int)(((2 * pa.stats[4] + Iv5 + fp / 4) * (level / 100) + 5) * Wesensd);
                        label23.Text = wert + "";
                        fp = (int.Parse(textBox6.Text));
                        wert = (int)(((2 * pa.stats[5] + Iv6 + fp / 4) * (level / 100) + 5) * Wesenspe);
                        label24.Text = wert + "";
                        label34.Text = pa.type1;
                        label33.Text = pa.type2;
                    }
                    else
                    {
                        Pokemon pa;
                        try
                        {
                            pa = matcherPokemon[comboBox1.SelectedItem.ToString()];
                        }
                        catch (NullReferenceException)
                        {
                            pa = matcherPokemon[comboBox1.Text];
                        }
                        



                        string stttt = comboBox10.SelectedItem.ToString().Substring(5);
                        int[] dd = pa.sonderstats[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1];
                        label51.Text = dd[0] + "";
                        label50.Text = dd[1] + "";
                        label49.Text = dd[2] + "";
                        label48.Text = dd[3] + "";
                        label47.Text = dd[4] + "";
                        label4.Text = dd[5] + "";
                        int fp = (int.Parse(textBox1.Text));
                        double level = (int.Parse(textBox13.Text));
                        int wert;
                        if (!pa.name.Equals("Shedinja"))
                        {
                            wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1][0] + Iv1 + fp / 4) * (double)(level / 100)) + 10 + level);
                        }
                        else
                        {
                            wert = 1;
                        }
                        label19.Text = wert + "";
                        fp = (int.Parse(textBox2.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1][1] + Iv2 + fp / 4) * (level / 100) + 5) * Wesena);
                        label20.Text = wert + "";
                        fp = (int.Parse(textBox3.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1][2] + Iv3 + fp / 4) * (level / 100) + 5) * Wesend);
                        label21.Text = wert + "";
                        fp = (int.Parse(textBox4.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1][3] + Iv4 + fp / 4) * (level / 100) + 5) * Wesensa);
                        label22.Text = wert + "";
                        fp = (int.Parse(textBox5.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1][4] + Iv5 + fp / 4) * (level / 100) + 5) * Wesensd);
                        label23.Text = wert + "";
                        fp = (int.Parse(textBox6.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1][5] + Iv6 + fp / 4) * (level / 100) + 5) * Wesenspe);
                        label24.Text = wert + "";
                        if (pa.sondertype1.Count > (int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1))
                        {
                            label34.Text = pa.sondertype1[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1];
                            label33.Text = pa.sondertype2[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1];
                        }
                        else
                        {
                            label34.Text = pa.type1;
                            label33.Text = pa.type2;
                        }

                    }

                    if (comboBox11.SelectedItem == null || comboBox11.SelectedItem.ToString().Equals("Normal"))
                    {
                        Pokemon pa;
                        try
                        {
                            pa = matcherPokemon[comboBox3.SelectedItem.ToString()];
                        }
                        catch (NullReferenceException)
                        {
                            if (matcherPokemon.ContainsKey(comboBox3.Text))
                            {
                                pa = matcherPokemon[comboBox3.Text];
                            }
                            else
                            {
                                pa = matcherPokemon["Bulbasaur"];
                            }
                        }


                        label57.Text = pa.stats[0] + "";
                        label56.Text = pa.stats[1] + "";
                        label55.Text = pa.stats[2] + "";
                        label54.Text = pa.stats[3] + "";
                        label53.Text = pa.stats[4] + "";
                        label52.Text = pa.stats[5] + "";



                        int fp = (int.Parse(textBox12.Text));
                        double level = (double.Parse(textBox14.Text));

                        int wert;
                        if (!pa.name.Equals("Shedinja"))
                        {
                            wert = (int)(((2 * pa.stats[0] + BIv1 + fp / 4) * (double)(level / 100)) + 10 + level);
                        }
                        else
                        {
                            wert = 1;
                        }
                        label30.Text = wert + "";
                        fp = (int.Parse(textBox11.Text));
                        wert = (int)(((2 * pa.stats[1] + BIv2 + fp / 4) * (level / 100) + 5) * BWesena);
                        label29.Text = wert + "";
                        fp = (int.Parse(textBox10.Text));
                        wert = (int)(((2 * pa.stats[2] + BIv3 + fp / 4) * (level / 100) + 5) * BWesend);
                        label28.Text = wert + "";
                        fp = (int.Parse(textBox9.Text));
                        wert = (int)(((2 * pa.stats[3] + BIv4 + fp / 4) * (level / 100) + 5) * BWesensa);
                        label27.Text = wert + "";
                        fp = (int.Parse(textBox8.Text));
                        wert = (int)(((2 * pa.stats[4] + BIv5 + fp / 4) * (level / 100) + 5) * BWesensd);
                        label26.Text = wert + "";
                        fp = (int.Parse(textBox7.Text));
                        wert = (int)(((2 * pa.stats[5] + BIv6 + fp / 4) * (level / 100) + 5) * BWesenspe);
                        label25.Text = wert + "";
                        label35.Text = pa.type1;
                        label36.Text = pa.type2;
                    }
                    else
                    {
                        Pokemon pa;
                        try
                        {
                            pa = matcherPokemon[comboBox3.SelectedItem.ToString()];
                        }
                        catch (NullReferenceException)
                        {
                            pa = matcherPokemon[comboBox3.Text];
                        }

                        int[] dd = pa.sonderstats[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1];

                        label57.Text = dd[0] + "";
                        label56.Text = dd[1] + "";
                        label55.Text = dd[2] + "";
                        label54.Text = dd[3] + "";
                        label53.Text = dd[4] + "";
                        label52.Text = dd[5] + "";
                        int fp = (int.Parse(textBox12.Text));
                        double level = (double.Parse(textBox14.Text));
                        int wert;
                        if (!pa.name.Equals("Shedinja"))
                        {
                            wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1][0] + BIv1 + fp / 4) * (double)(level / 100)) + 10 + level);
                        }
                        else
                        {
                            wert = 1;
                        }
                        label30.Text = wert + "";
                        fp = (int.Parse(textBox11.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1][1] + BIv2 + fp / 4) * (level / 100) + 5) * BWesena);
                        label29.Text = wert + "";
                        fp = (int.Parse(textBox10.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1][2] + BIv3 + fp / 4) * (level / 100) + 5) * BWesend);
                        label28.Text = wert + "";
                        fp = (int.Parse(textBox9.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1][3] + BIv4 + fp / 4) * (level / 100) + 5) * BWesensa);
                        label27.Text = wert + "";
                        fp = (int.Parse(textBox8.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1][4] + BIv5 + fp / 4) * (level / 100) + 5) * BWesensd);
                        label26.Text = wert + "";
                        fp = (int.Parse(textBox7.Text));
                        wert = (int)(((2 * pa.sonderstats[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1][5] + BIv6 + fp / 4) * (level / 100) + 5) * BWesenspe);
                        label25.Text = wert + "";

                        if (pa.sondertype1.Count > (int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1))
                        {
                            label35.Text = pa.sondertype1[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1];
                            label36.Text = pa.sondertype2[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1];
                        }
                        else
                        {
                            label35.Text = pa.type1;
                            label36.Text = pa.type2;
                        }

                    }
                    damagecalc();
                }
                catch (FormatException)
                {

                }
            }
            catch (OverflowException)
            {

            }
        }

        private double multiplier(TextBox t)
        {
            t.Text.Trim();
            if (t == textBox34 && comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Hustle"))
            {
            }
            else if (t.Text.Equals("") || t.Text.Equals("0"))
            {
                return 1;
            }
            try
            {
                int a = int.Parse(t.Text);
                if (t == textBox34 && comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Hustle"))
                {
                    a++;
                }
                if (a > 0)
                {
                    return (1 + (double)a / 2);
                }
                else if (a < 0)
                {
                    return 2 / ((double)Math.Abs(a) + 2);
                }
                else
                {
                    return 1;
                }
            }
            catch (FormatException)
            {
                return 1;
            }

        }

        double atkmod = 1;
        double defmod = 1;
        double mod1 = 1;
        double mod2 = 1;
        double mod3 = 1;
        private void damagecalc()
        {
            if (comboBox7.SelectedItem == null || (comboBox7.SelectedItem != null && (comboBox7.SelectedItem.ToString().Equals("Wonder Guard") && checkopponenttype1() * checkopponenttype2() > 1) || (!comboBox7.SelectedItem.ToString().Equals("Wonder Guard"))))
            {
                if (comboBox2.SelectedItem != null)
                {
                    int atk;
                    int def;
                    Attacks a = matcherAttacks[comboBox2.SelectedItem.ToString()];
                    try
                    {
                        a = new Attacks(a.name, int.Parse(textBox27.Text), a.split, a.type);
                    }
                    catch (FormatException)
                    {

                    }
                    try
                    {
                        a = new Attacks(a.name, a.strength, a.split, comboBox13.SelectedItem.ToString());
                    }
                    catch (NullReferenceException)
                    {

                    }

                    a = paramAttack(a);
                    int atkevs = 0;
                    int defevs = 0;
                    int hpevs = 0;
                    int levela = 100;
                    int levelb = 100;
                    string atkab = "";
                    string defab = "";
                    try
                    {
                        levela = int.Parse(textBox13.Text);
                    }
                    catch (FormatException)
                    {
                    }
                    try
                    {
                        levelb = int.Parse(textBox14.Text);
                    }
                    catch (FormatException)
                    {
                    }


                    if (a.strength != 0)
                    {
                        comboBox13.SelectedItem = a.type;
                        label46.Text = a.strength + "";
                        
                        if (a.split == 0)
                        {
                            if (comboBox6.SelectedItem != null && (comboBox6.SelectedItem.ToString().Equals("Huge Power") || comboBox6.SelectedItem.ToString().Equals("Pure Power")))
                            {
                                if (a.name.Equals("Foul Play"))
                                {
                                    if (comboBox7.SelectedItem != null && (comboBox7.SelectedItem.ToString().Equals("Huge Power") || comboBox7.SelectedItem.ToString().Equals("Pure Power")))
                                    {
                                        atk = int.Parse(label29.Text) * 2;
                                        try
                                        {
                                            atkevs = int.Parse(textBox11.Text);
                                        }
                                        catch (FormatException)
                                        {
                                        }
                                    }
                                    else
                                    {
                                        atk = int.Parse(label29.Text);
                                        try
                                        {
                                            atkevs = int.Parse(textBox11.Text);
                                        }
                                        catch (FormatException)
                                        {
                                        }
                                    }
                                }
                                else
                                {
                                    atk = int.Parse(label20.Text) * 2;
                                    try
                                    {
                                        atkevs = int.Parse(textBox2.Text);
                                    }
                                    catch (FormatException)
                                    {
                                    }
                                }

                            }
                            else
                            {
                                if (a.name.Equals("Foul Play"))
                                {
                                    if (comboBox7.SelectedItem != null && (comboBox7.SelectedItem.ToString().Equals("Huge Power") || comboBox7.SelectedItem.ToString().Equals("Pure Power")))
                                    {
                                        atk = int.Parse(label29.Text) * 2;
                                        try
                                        {
                                            atkevs = int.Parse(textBox11.Text);
                                        }
                                        catch (FormatException)
                                        {
                                        }
                                    }
                                    else
                                    {
                                        atk = int.Parse(label29.Text);
                                        try
                                        {
                                            atkevs = int.Parse(textBox11.Text);
                                        }
                                        catch (FormatException)
                                        {
                                        }
                                    }
                                }
                                else
                                {
                                    atk = int.Parse(label20.Text);
                                    try
                                    {
                                        atkevs = int.Parse(textBox2.Text);
                                    }
                                    catch (FormatException)
                                    {
                                    }
                                }
                            }
                            
                            def = int.Parse(label28.Text);
                            try
                            {
                                defevs = int.Parse(textBox10.Text);
                            }
                            catch (FormatException)
                            {
                            }
                            if (comboBox8.SelectedItem != null)
                            {
                                if (matcherNatures[comboBox8.SelectedItem.ToString()].raises.Equals("Attack"))
                                {
                                    atkab = "+";
                                }
                                if (matcherNatures[comboBox8.SelectedItem.ToString()].lowers.Equals("Attack"))
                                {
                                    atkab = "-";
                                }
                            }
                            if (comboBox9.SelectedItem != null)
                            {
                                if (matcherNatures[comboBox9.SelectedItem.ToString()].raises.Equals("Defense"))
                                {
                                    defab = "+";
                                }
                                if (matcherNatures[comboBox9.SelectedItem.ToString()].lowers.Equals("Defense"))
                                {
                                    defab = "-";
                                }
                            }
                        }
                        else
                        {
                            atk = int.Parse(label22.Text);
                            try
                            {
                                atkevs = int.Parse(textBox4.Text);
                            }
                            catch (FormatException)
                            {
                            }

                            if (comboBox8.SelectedItem != null)
                            {
                                if (matcherNatures[comboBox8.SelectedItem.ToString()].raises.Equals("Special Attack"))
                                {
                                    atkab = "+";
                                }
                                if (matcherNatures[comboBox8.SelectedItem.ToString()].lowers.Equals("Special Attack"))
                                {
                                    atkab = "-";
                                }
                            }
                            if (a.name.Equals("Psyshock") || a.name.Equals("Psystrike"))
                            {
                                def = int.Parse(label28.Text);
                                try
                                {
                                    defevs = int.Parse(textBox10.Text);
                                }
                                catch (FormatException)
                                {
                                }
                                if (comboBox9.SelectedItem != null)
                                {
                                    if (matcherNatures[comboBox9.SelectedItem.ToString()].raises.Equals("Defense"))
                                    {
                                        defab = "+";
                                    }
                                    if (matcherNatures[comboBox9.SelectedItem.ToString()].lowers.Equals("Defense"))
                                    {
                                        defab = "-";
                                    }
                                }
                            }
                            else
                            {
                                def = int.Parse(label26.Text);
                                try
                                {
                                    defevs = int.Parse(textBox8.Text);
                                }
                                catch (FormatException)
                                {
                                }
                                if (comboBox9.SelectedItem != null)
                                {
                                    if (matcherNatures[comboBox9.SelectedItem.ToString()].raises.Equals("Special Defense"))
                                    {
                                        defab = "+";
                                    }
                                    if (matcherNatures[comboBox9.SelectedItem.ToString()].lowers.Equals("Special Defense"))
                                    {
                                        defab = "-";
                                    }
                                }
                            }
                        }
                        try
                        {
                            hpevs = int.Parse(textBox12.Text);
                        }
                        catch (FormatException)
                        {
                        }

                        try
                        {
                            int level = int.Parse(textBox13.Text);
                            int calc = (2 * level) / 5;
                            if (calc == 0)
                            {
                                calc = 1;
                            }
                            calc = calc + 2;
                            int atkbase = a.strength;
                            calc = calc * atkbase;
                            calc = (int)(calc * atk * atkmod);
                            calc = calc / 50;
                            if (calc == 0)
                            {
                                calc = 1;
                            }
                            calc = (int)(calc / (def * defmod));
                            if (calc == 0)
                            {
                                calc = 1;
                            }

                            calc = (int)(calc * mod1);
                            if (calc == 0)
                            {
                                calc = 1;
                            }
                            calc = calc + 2;
                            if (checkBox1.Checked)
                            {
                                try
                                {
                                    if (comboBox6.SelectedItem.ToString().Equals("Sniper"))
                                    {
                                        calc = (int)(calc * 2.25);
                                    }
                                    else
                                    {
                                        calc = (int)(calc * 1.5);
                                    }
                                }
                                catch (NullReferenceException)
                                {
                                    calc = (int)(calc * 1.5);
                                }
                            }

                            calc = (int)(calc * mod2);
                            int mincalc = calc * 85;
                            int maxcalc = calc * 100;
                            mincalc = mincalc / 100;
                            maxcalc = maxcalc / 100;
                            if (mincalc == 0)
                            {
                                mincalc = 1;
                            }
                            if (maxcalc == 0)
                            {
                                maxcalc = 1;
                            }
                            if (comboBox13.SelectedItem.ToString().Equals(label34.Text) || comboBox13.SelectedItem.ToString().Equals(label33.Text) || (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Protean")))  //Stab
                            {
                                try
                                {
                                    if (comboBox6.SelectedItem.ToString().Equals("Adaptibility"))
                                    {
                                        mincalc = (int)(mincalc * 2);
                                        maxcalc = (int)(maxcalc * 2);
                                    }
                                    else
                                    {
                                        mincalc = (int)(mincalc * 1.5);
                                        maxcalc = (int)(maxcalc * 1.5);
                                    }
                                }
                                catch (NullReferenceException)
                                {
                                    mincalc = (int)(mincalc * 1.5);
                                    maxcalc = (int)(maxcalc * 1.5);
                                }

                            }
                            double e = 1;
                            if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Scrappy"))
                            {
                                if (a.type.Equals("normal") || a.type.Equals("fighting"))
                                {
                                    if (a.name.Equals("Flying Press"))
                                    {
                                        if (!label35.Text.Equals("ghost"))
                                        {
                                            mincalc = (int)(mincalc * checkopponenttype1());
                                            maxcalc = (int)(maxcalc * checkopponenttype1());
                                        }
                                        if (!label36.Text.Equals("ghost"))
                                        {
                                            mincalc = (int)(mincalc * checkopponenttype2());
                                            maxcalc = (int)(maxcalc * checkopponenttype2());
                                        }
                                        double ty1 = checkopponenttype1("flying");
                                        double ty2 = checkopponenttype2("flying");
                                        mincalc = (int)(mincalc * ty1);
                                        maxcalc = (int)(maxcalc * ty1);
                                        mincalc = (int)(mincalc * ty2);
                                        maxcalc = (int)(maxcalc * ty2);
                                        if (checkopponenttype1() * checkopponenttype2() * ty1 * ty2 > 1)
                                        {
                                            try
                                            {
                                                if (comboBox4.SelectedItem.ToString().Equals("Expert Belt"))
                                                {
                                                    e = e * 1.2;
                                                }
                                            }
                                            catch (NullReferenceException)
                                            {

                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!label35.Text.Equals("ghost"))
                                        {
                                            mincalc = (int)(mincalc * checkopponenttype1());
                                            maxcalc = (int)(maxcalc * checkopponenttype1());
                                        }
                                        if (!label36.Text.Equals("ghost"))
                                        {
                                            mincalc = (int)(mincalc * checkopponenttype2());
                                            maxcalc = (int)(maxcalc * checkopponenttype2());
                                        }
                                        if (checkopponenttype1() * checkopponenttype2() > 1)
                                        {
                                            try
                                            {
                                                if (comboBox4.SelectedItem.ToString().Equals("Expert Belt"))
                                                {
                                                    e = e * 1.2;
                                                }
                                            }
                                            catch (NullReferenceException)
                                            {

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (a.name.Equals("Freeze-Dry"))
                                    {
                                        mincalc = (int)(mincalc * checkopponenttype1f());
                                        maxcalc = (int)(maxcalc * checkopponenttype1f());
                                        mincalc = (int)(mincalc * checkopponenttype2f());
                                        maxcalc = (int)(maxcalc * checkopponenttype2f());
                                        if (checkopponenttype1f() * checkopponenttype2f() > 1)
                                        {
                                            try
                                            {
                                                if (comboBox4.SelectedItem.ToString().Equals("Expert Belt"))
                                                {
                                                    e = e * 1.2;
                                                }
                                            }
                                            catch (NullReferenceException)
                                            {

                                            }
                                        }
                                    }
                                    else
                                    {
                                        mincalc = (int)(mincalc * checkopponenttype1());
                                        maxcalc = (int)(maxcalc * checkopponenttype1());
                                        mincalc = (int)(mincalc * checkopponenttype2());
                                        maxcalc = (int)(maxcalc * checkopponenttype2());
                                        if (checkopponenttype1() * checkopponenttype2() > 1)
                                        {
                                            try
                                            {
                                                if (comboBox4.SelectedItem.ToString().Equals("Expert Belt"))
                                                {
                                                    e = e * 1.2;
                                                }
                                            }
                                            catch (NullReferenceException)
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (a.name.Equals("Flying Press"))
                                {
                                    mincalc = (int)(mincalc * checkopponenttype1());
                                    maxcalc = (int)(maxcalc * checkopponenttype1());
                                    mincalc = (int)(mincalc * checkopponenttype2());
                                    maxcalc = (int)(maxcalc * checkopponenttype2());
                                    double ty1 = checkopponenttype1("flying");
                                    double ty2 = checkopponenttype2("flying");
                                    mincalc = (int)(mincalc * ty1);
                                    maxcalc = (int)(maxcalc * ty1);
                                    mincalc = (int)(mincalc * ty2);
                                    maxcalc = (int)(maxcalc * ty2);
                                    if (checkopponenttype1() * checkopponenttype2() * ty1 * ty2 > 1)
                                    {
                                        try
                                        {
                                            if (comboBox4.SelectedItem.ToString().Equals("Expert Belt"))
                                            {
                                                e = e * 1.2;
                                            }
                                        }
                                        catch (NullReferenceException)
                                        {

                                        }
                                    }
                                }
                                else if (a.name.Equals("Freeze-Dry"))
                                {
                                    mincalc = (int)(mincalc * checkopponenttype1f());
                                    maxcalc = (int)(maxcalc * checkopponenttype1f());
                                    mincalc = (int)(mincalc * checkopponenttype2f());
                                    maxcalc = (int)(maxcalc * checkopponenttype2f());
                                    if (checkopponenttype1f() * checkopponenttype2f() > 1)
                                    {
                                        try
                                        {
                                            if (comboBox4.SelectedItem.ToString().Equals("Expert Belt"))
                                            {
                                                e = e * 1.2;
                                            }
                                        }
                                        catch (NullReferenceException)
                                        {

                                        }
                                    }
                                }
                                else
                                {
                                    mincalc = (int)(mincalc * checkopponenttype1());
                                    maxcalc = (int)(maxcalc * checkopponenttype1());
                                    mincalc = (int)(mincalc * checkopponenttype2());
                                    maxcalc = (int)(maxcalc * checkopponenttype2());
                                    if (checkopponenttype1() * checkopponenttype2() > 1)
                                    {
                                        try
                                        {
                                            if (comboBox4.SelectedItem.ToString().Equals("Expert Belt"))
                                            {
                                                e = e * 1.2;
                                            }
                                        }
                                        catch (NullReferenceException)
                                        {

                                        }
                                    }
                                }
                            }
                            mincalc = (int)(mincalc * mod3 * e);
                            maxcalc = (int)(maxcalc * mod3 * e);
                            int minPercenta = (int)((((double)mincalc) / double.Parse(label30.Text)) * 10000);
                            int minPercentb = minPercenta % 100;
                            string minPercentc = (minPercentb / 10) + "" + (minPercentb % 10);
                            minPercenta = minPercenta / 100;
                            int maxPercenta = (int)((((double)maxcalc) / double.Parse(label30.Text)) * 10000);
                            int maxPercentb = maxPercenta % 100;
                            string maxPercentc = (maxPercentb / 10) + "" + (maxPercentb % 10);
                            maxPercenta = maxPercenta / 100;

                            if (comboBox3.SelectedItem != null && comboBox1.SelectedItem != null)
                            {
                                textBox33.Text = "Damagecalc: " + comboBox1.SelectedItem.ToString() + "'s (Level " + levela + ") " + a.name + " does with " + atkevs + atkab + " Evs MinDamage: " + mincalc + " MinPercent: " + minPercenta + "." + minPercentc + "% MaxDamage: " + maxcalc + " MaxPercent: " + maxPercenta + "." + maxPercentc + "% to " + hpevs + "/" + defevs + defab + " Ev " + comboBox3.SelectedItem.ToString() + " (Level " + levelb + ")";
                            }
                            else if (comboBox3.SelectedItem != null)
                            {
                                textBox33.Text = "Damagecalc: Bulbasaur's (Level " + levela + ") " + a.name + " does with " + atkevs + atkab + " Evs MinDamage: " + mincalc + " MinPercent: " + minPercenta + "." + minPercentc + "% MaxDamage: " + maxcalc + " MaxPercent: " + maxPercenta + "." + maxPercentc + "% to " + hpevs + "/" + defevs + defab + " Ev " + comboBox3.SelectedItem.ToString() + " (Level " + levelb + ")";
                            }
                            else if (comboBox1.SelectedItem != null)
                            {
                                textBox33.Text = "Damagecalc: " + comboBox1.SelectedItem.ToString() + "'s (Level " + levela + ") " + a.name + " does with " + atkevs + atkab + " Evs MinDamage: " + mincalc + " MinPercent: " + minPercenta + "." + minPercentc + "% MaxDamage: " + maxcalc + " MaxPercent: " + maxPercenta + "." + maxPercentc + "% to " + hpevs + "/" + defevs + defab + " Ev Bulbasaur (Level " + levelb + ")";
                            }
                            else
                            {
                                textBox33.Text = "Damagecalc: Bulbasaur's (Level "+ levela + ") " + a.name + " does with "+atkevs+atkab+" Evs MinDamage: " + mincalc + " MinPercent: " + minPercenta + "." + minPercentc + "% MaxDamage: " + maxcalc + " MaxPercent: " + maxPercenta + "." + maxPercentc + "% to "+hpevs+"/"+defevs+defab+" Ev Bulbasaur (Level "+ levelb + ")";
                            }
                        }
                        catch (FormatException)
                        {

                        }
                    }
                    else
                    {

                        textBox33.Text = "Damagecalc: " + comboBox1.SelectedItem.ToString() + "'s (Level " + levela + ") " + a.name + " does with " + atkevs + atkab + " Evs MinDamage: " + 0 + " MinPercent: " + 0 + "." + 0 + "% MaxDamage: " + 0 + " MaxPercent: " + 0 + "." + 0 + "%";

                    }

                }
            }
            else
            {
                textBox33.Text = "Damagecalc: " + " MinDamage: " + 0 + " MinPercent: " + 0 + "." + 0 + "% MaxDamage: " + 0 + " MaxPercent: " + 0 + "." + 0 + "%";
            }
        }

        private Attacks paramAttack(Attacks a)
        {
            if (a.name.Equals("Gyro Ball"))
            {
                a = new Attacks(a.name, (int)(25 * (double.Parse(label25.Text) / double.Parse(label24.Text))), a.split, a.type);
            }
            else if (a.name.Equals("Acrobatics"))
            {
                if (comboBox4.SelectedItem == null || comboBox4.SelectedItem.ToString().Equals("No Item"))
                {
                    a = new Attacks(a.name, a.strength * 2, a.split, a.type);
                }
                else if (comboBox4.SelectedItem.ToString().Equals("Damage Gem"))
                {
                    a = new Attacks(a.name, a.strength * 3, a.split, a.type);
                }
            }
            else if (a.name.Equals("Low Kick") || a.name.Equals("Grass Knot"))
            {
                int dmg = 0;
                double w = 0;
                if (comboBox11.SelectedItem == null || comboBox11.SelectedItem.ToString().Equals("Normal"))
                {
                    w = matcherPokemon[comboBox3.SelectedItem.ToString()].weight;
                }
                else
                {
                    w = matcherPokemon[comboBox3.SelectedItem.ToString()].sonderweight[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1];
                }
                if (w <= 10)
                {
                    dmg = 20;
                }
                else if (10.1 <= w && w <= 25)
                {
                    dmg = 40;
                }
                else if (25.1 <= w && w <= 50)
                {
                    dmg = 60;
                }
                else if (50.1 <= w && w <= 100)
                {
                    dmg = 80;
                }
                else if (100.1 <= w && w <= 200)
                {
                    dmg = 100;
                }
                else
                {
                    dmg = 120;
                }


                a = new Attacks(a.name, dmg, a.split, a.type);
            }
            else if (a.name.Equals("Beat Up"))
            {
                Pokemon p;
                if (comboBox1.SelectedItem == null)
                {
                    p = matcherPokemon["Bulbasaur"];
                }
                else
                {
                    p = matcherPokemon[comboBox1.SelectedItem.ToString()];
                }
                if (comboBox10.SelectedItem == null || comboBox10.SelectedItem.ToString().Equals("Normal"))
                {
                    a = new Attacks(a.name, (int)Math.Ceiling(((p.stats[1] / (double)10) + 5)), a.split, a.type);
                }
                else
                {
                    a = new Attacks(a.name, (int)Math.Ceiling(((p.sonderstats[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1][1]) / (double)10 + 5)), a.split, a.type);
                }
            }
            else if (a.name.Equals("Electro Ball"))
            {
                double d = double.Parse(label25.Text) / double.Parse(label24.Text);
                int dam = 0;
                if (d <= (1 / (double)4))
                {
                    dam = 150;
                }
                else if (1 / (double)4 < d && d <= 1 / (double)3)
                {
                    dam = 120;
                }
                else if (1 / (double)3 < d && d <= 1 / (double)2)
                {
                    dam = 80;
                }
                else
                {
                    dam = 60;
                }
                a = new Attacks(a.name, dam, a.split, a.type);
            }
            else if (a.name.Equals("Heat Crash") || a.name.Equals("Heavy Slam"))
            {
                int dmg = 0;
                double w = 0;
                if (comboBox11.SelectedItem == null || comboBox11.SelectedItem.ToString().Equals("Normal"))
                {
                    w = matcherPokemon[comboBox3.SelectedItem.ToString()].weight;
                }
                else
                {
                    w = matcherPokemon[comboBox3.SelectedItem.ToString()].sonderweight[int.Parse(comboBox11.SelectedItem.ToString().Substring(5)) - 1];
                }
                if (comboBox10.SelectedItem == null || comboBox10.SelectedItem.ToString().Equals("Normal"))
                {
                    w = w / matcherPokemon[comboBox1.SelectedItem.ToString()].weight;
                }
                else
                {
                    w = w / matcherPokemon[comboBox1.SelectedItem.ToString()].sonderweight[int.Parse(comboBox10.SelectedItem.ToString().Substring(5)) - 1];
                }
                if (w > 0.5)
                {
                    dmg = 40;
                }
                else if (0.5 <= w && w > 0.333)
                {
                    dmg = 60;
                }
                else if (0.333 <= w && w > 0.25)
                {
                    dmg = 80;
                }
                else if (0.25 <= w && w > 0.20)
                {
                    dmg = 100;
                }
                else
                {
                    dmg = 120;
                }

                a = new Attacks(a.name, dmg, a.split, a.type);
            }
            else if (a.name.Equals("Knock Off"))
            {
                if (comboBox5.SelectedItem == null || comboBox5.SelectedItem.ToString().Equals("No Item"))
                {

                }
                else
                {
                    a = new Attacks(a.name, (int)(a.strength * 1.5), a.split, a.type);
                }
            }
            else if (a.name.Equals("Punishment"))
            {
                int dmg = 60;
                try
                {
                    dmg = dmg + 20 * (int.Parse(textBox28.Text) + int.Parse(textBox29.Text) + int.Parse(textBox30.Text) + int.Parse(textBox31.Text) + int.Parse(textBox32.Text));
                }
                catch (FormatException)
                {

                }
                a = new Attacks(a.name, dmg, a.split, a.type);
            }
            else if (a.name.Equals("Weather Ball"))
            {
                if (comboBox12.SelectedItem != null)
                {
                    switch (comboBox12.SelectedItem.ToString())
                    {
                        case "Sun":
                            a = new Attacks(a.name, 100, a.split, "fire");
                            break;
                        case "Rain":
                            a = new Attacks(a.name, 100, a.split, "water");
                            break;
                        case "Sand":
                            a = new Attacks(a.name, 100, a.split, "rock");
                            break;
                        case "Hail":
                            a = new Attacks(a.name, 100, a.split, "ice");
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (a.name.Equals("Stored Power"))
            {
                int dmg = 20;
                int I = 0;
                try
                {
                    if (int.Parse(textBox34.Text) > 0)
                    {
                        I = int.Parse(textBox34.Text);
                    }
                }
                catch (FormatException)
                {

                }
                int II = 0;
                try
                {
                    if (int.Parse(textBox35.Text) > 0)
                    {
                        II = int.Parse(textBox35.Text);
                    }
                }
                catch (FormatException)
                {

                }
                int III = 0;
                try
                {
                    if (int.Parse(textBox36.Text) > 0)
                    {
                        III = int.Parse(textBox36.Text);
                    }
                }
                catch (FormatException)
                {

                }
                int IV = 0;

                try
                {
                    if (int.Parse(textBox37.Text) > 0)
                    {
                        IV = int.Parse(textBox37.Text);
                    }
                }
                catch (FormatException)
                {

                }
                int V = 0;
                try
                {
                    if (int.Parse(textBox38.Text) > 0)
                    {
                        V = int.Parse(textBox38.Text);
                    }
                }
                catch (FormatException)
                {

                }

                dmg = dmg + 20 * (I + II + III + IV + V);

                a = new Attacks(a.name, dmg, a.split, a.type);
            }
            if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Reckless"))
            {
                if (a.name.Equals("Brave Bird") || a.name.Equals("Double-Edge") || a.name.Equals("Flare Blitz") || a.name.Equals("Head Charge") || a.name.Equals("Head Smash") || a.name.Equals("High Jump Kick") || a.name.Equals("Jump Kick") || a.name.Equals("Submission") || a.name.Equals("Take Down") || a.name.Equals("Volt Tackle") || a.name.Equals("Wood Hammer") || a.name.Equals("Wild Charge"))
                {
                    a = new Attacks(a.name, (int)(a.strength * 1.2), a.split, a.type);
                }
            }
            else if (comboBox6.SelectedItem != null && (comboBox6.SelectedItem.ToString().Equals("Aerilate")) && a.type.Equals("normal"))
            {
                a = new Attacks(a.name, (int)(a.strength * 1.3), a.split, "flying");
            }
            else if (comboBox6.SelectedItem != null && (comboBox6.SelectedItem.ToString().Equals("Pixilate")) && a.type.Equals("normal"))
            {
                a = new Attacks(a.name, (int)(a.strength * 1.3), a.split, "fairy");
            }
            else if (comboBox6.SelectedItem != null && (comboBox6.SelectedItem.ToString().Equals("rigerate")) && a.type.Equals("normal"))
            {
                a = new Attacks(a.name, (int)(a.strength * 1.3), a.split, "ice");
            }
            else if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Analytic"))
            {
                if (int.Parse(label24.Text) < int.Parse(label25.Text))
                {
                    a = new Attacks(a.name, (int)(a.strength * 1.3), a.split, a.name);
                }
            }
            else if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Iron Fist"))
            {
                if (a.name.Equals("Bullet Punch") || a.name.Equals("Comet Punch") || a.name.Equals("Dizzy Punch") || a.name.Equals("Drain Punch") || a.name.Equals("Dynamic Punch") || a.name.Equals("Fire Punch") || a.name.Equals("Focus Punch") || a.name.Equals("Hammer Arm") || a.name.Equals("Ice Punch") || a.name.Equals("Mach Punch") || a.name.Equals("Mega Punch") || a.name.Equals("Meteor Mash") || a.name.Equals("Power-Up Punch") || a.name.Equals("Shadow Punch") || a.name.Equals("Sky Uppercut") || a.name.Equals("Thunder Punch"))
                {
                    a = new Attacks(a.name, (int)(a.strength * 1.2), a.split, a.type);
                }
            }
            else if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Mega Launcher"))
            {
                if (a.name.Equals("Aura Sphere") || a.name.Equals("Dark Pulse") || a.name.Equals("Dragon Pulse") || a.name.Equals("Water Pulse"))
                {
                    a = new Attacks(a.name, (int)(a.strength * 1.5), a.split, a.type);
                }
            }
            else if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Sand Force"))
            {
                if (comboBox12.SelectedItem != null && comboBox12.SelectedItem.ToString().Equals("Sand"))
                {
                    if (a.type.Equals("rock") || a.type.Equals("steel") || a.type.Equals("ground"))
                    {
                        a = new Attacks(a.name, (int)(a.strength * 1.3), a.split, a.type);
                    }
                }
            }
            else if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Sheer Force"))
            {
                if (comboBox2.SelectedItem != null && (comboBox2.SelectedItem.ToString().Equals("Acid") || comboBox2.SelectedItem.ToString().Equals("Acid Spray") || comboBox2.SelectedItem.ToString().Equals("Air Slash") || comboBox2.SelectedItem.ToString().Equals("Ancient Power") || comboBox2.SelectedItem.ToString().Equals("Astonish") || comboBox2.SelectedItem.ToString().Equals("Aurora Beam") || comboBox2.SelectedItem.ToString().Equals("Bite") || comboBox2.SelectedItem.ToString().Equals("Blaze Kick") || comboBox2.SelectedItem.ToString().Equals("Blizzard") || comboBox2.SelectedItem.ToString().Equals("Blue Flare") || comboBox2.SelectedItem.ToString().Equals("Body Slam") || comboBox2.SelectedItem.ToString().Equals("Bolt Strike") || comboBox2.SelectedItem.ToString().Equals("Bone Club") || comboBox2.SelectedItem.ToString().Equals("Bounce") || comboBox2.SelectedItem.ToString().Equals("Bubble") || comboBox2.SelectedItem.ToString().Equals("BubbleBeam") || comboBox2.SelectedItem.ToString().Equals("Bug Buzz") || comboBox2.SelectedItem.ToString().Equals("Bulldoze") || comboBox2.SelectedItem.ToString().Equals("Charge Beam") || comboBox2.SelectedItem.ToString().Equals("Chatter") || comboBox2.SelectedItem.ToString().Equals("Confusion") || comboBox2.SelectedItem.ToString().Equals("Constrict") || comboBox2.SelectedItem.ToString().Equals("Cross Poison") || comboBox2.SelectedItem.ToString().Equals("Crunch") || comboBox2.SelectedItem.ToString().Equals("Crush Claw") || comboBox2.SelectedItem.ToString().Equals("Dark Pulse") || comboBox2.SelectedItem.ToString().Equals("Discharge") || comboBox2.SelectedItem.ToString().Equals("Dizzy Punch") || comboBox2.SelectedItem.ToString().Equals("Dragon Rush") || comboBox2.SelectedItem.ToString().Equals("Dragon Breath") || comboBox2.SelectedItem.ToString().Equals("Dynamic Punch") || comboBox2.SelectedItem.ToString().Equals("Earth Power") || comboBox2.SelectedItem.ToString().Equals("Electroweb") || comboBox2.SelectedItem.ToString().Equals("Ember") || comboBox2.SelectedItem.ToString().Equals("Energy Ball") || comboBox2.SelectedItem.ToString().Equals("Extrasensory") || comboBox2.SelectedItem.ToString().Equals("Fake Out") || comboBox2.SelectedItem.ToString().Equals("Fiery Dance") || comboBox2.SelectedItem.ToString().Equals("Fire Blast") || comboBox2.SelectedItem.ToString().Equals("Fire Fang") || comboBox2.SelectedItem.ToString().Equals("Fire Punch") || comboBox2.SelectedItem.ToString().Equals("Flame Charge") || comboBox2.SelectedItem.ToString().Equals("Flame Wheel") || comboBox2.SelectedItem.ToString().Equals("Flamethrower") || comboBox2.SelectedItem.ToString().Equals("Flare Blitz") || comboBox2.SelectedItem.ToString().Equals("Flash Cannon") || comboBox2.SelectedItem.ToString().Equals("Focus Blast") || comboBox2.SelectedItem.ToString().Equals("Force Palm") || comboBox2.SelectedItem.ToString().Equals("Freeze Shock") || comboBox2.SelectedItem.ToString().Equals("Glaciate") || comboBox2.SelectedItem.ToString().Equals("Gunk Shot") || comboBox2.SelectedItem.ToString().Equals("Headbutt") || comboBox2.SelectedItem.ToString().Equals("Heart Stamp") || comboBox2.SelectedItem.ToString().Equals("Heat Wave") || comboBox2.SelectedItem.ToString().Equals("Hurricane") || comboBox2.SelectedItem.ToString().Equals("Hyper Fang") || comboBox2.SelectedItem.ToString().Equals("Ice Beam") || comboBox2.SelectedItem.ToString().Equals("Ice Burn") || comboBox2.SelectedItem.ToString().Equals("Ice Fang") || comboBox2.SelectedItem.ToString().Equals("Ice Punch") || comboBox2.SelectedItem.ToString().Equals("Icicle Crash") || comboBox2.SelectedItem.ToString().Equals("Icy Wind") || comboBox2.SelectedItem.ToString().Equals("Inferno") || comboBox2.SelectedItem.ToString().Equals("Iron Head") || comboBox2.SelectedItem.ToString().Equals("Iron Tail") || comboBox2.SelectedItem.ToString().Equals("Lava Plume") || comboBox2.SelectedItem.ToString().Equals("Leaf Tornado") || comboBox2.SelectedItem.ToString().Equals("Lick") || comboBox2.SelectedItem.ToString().Equals("Low Sweep") || comboBox2.SelectedItem.ToString().Equals("Luster Purge") || comboBox2.SelectedItem.ToString().Equals("Metal Claw") || comboBox2.SelectedItem.ToString().Equals("Meteor Mash") || comboBox2.SelectedItem.ToString().Equals("Mirror Shot") || comboBox2.SelectedItem.ToString().Equals("Mist Ball") || comboBox2.SelectedItem.ToString().Equals("Moonblast") || comboBox2.SelectedItem.ToString().Equals("Mud Bomb") || comboBox2.SelectedItem.ToString().Equals("Mud Shot") || comboBox2.SelectedItem.ToString().Equals("Mud-Slap") || comboBox2.SelectedItem.ToString().Equals("Muddy Water") || comboBox2.SelectedItem.ToString().Equals("Needle Arm") || comboBox2.SelectedItem.ToString().Equals("Night Daze") || comboBox2.SelectedItem.ToString().Equals("Octazooka") || comboBox2.SelectedItem.ToString().Equals("Ominous Wind") || comboBox2.SelectedItem.ToString().Equals("Play Rough") || comboBox2.SelectedItem.ToString().Equals("Poison Fang") || comboBox2.SelectedItem.ToString().Equals("Poison Jab") || comboBox2.SelectedItem.ToString().Equals("Poison Sting") || comboBox2.SelectedItem.ToString().Equals("Poison Tail") || comboBox2.SelectedItem.ToString().Equals("Powder Snow") || comboBox2.SelectedItem.ToString().Equals("Psybeam") || comboBox2.SelectedItem.ToString().Equals("Psychic") || comboBox2.SelectedItem.ToString().Equals("Razor Shell") || comboBox2.SelectedItem.ToString().Equals("Relic Song") || comboBox2.SelectedItem.ToString().Equals("Rock Climb") || comboBox2.SelectedItem.ToString().Equals("Rock Slide") || comboBox2.SelectedItem.ToString().Equals("Rock Smash") || comboBox2.SelectedItem.ToString().Equals("Rock Tomb") || comboBox2.SelectedItem.ToString().Equals("Rolling Kick") || comboBox2.SelectedItem.ToString().Equals("Sacred Fire") || comboBox2.SelectedItem.ToString().Equals("Scald") || comboBox2.SelectedItem.ToString().Equals("Searing Shot") || comboBox2.SelectedItem.ToString().Equals("Secret Power") || comboBox2.SelectedItem.ToString().Equals("Seed Flare") || comboBox2.SelectedItem.ToString().Equals("Shadow Ball") || comboBox2.SelectedItem.ToString().Equals("Signal Beam") || comboBox2.SelectedItem.ToString().Equals("Silver Wind") || comboBox2.SelectedItem.ToString().Equals("Sky Attack") || comboBox2.SelectedItem.ToString().Equals("Sludge") || comboBox2.SelectedItem.ToString().Equals("Sludge Bomb") || comboBox2.SelectedItem.ToString().Equals("Sludge Wave") || comboBox2.SelectedItem.ToString().Equals("Smog") || comboBox2.SelectedItem.ToString().Equals("Snarl") || comboBox2.SelectedItem.ToString().Equals("Snore") || comboBox2.SelectedItem.ToString().Equals("Spark") || comboBox2.SelectedItem.ToString().Equals("Steamroller") || comboBox2.SelectedItem.ToString().Equals("Steel Wing") || comboBox2.SelectedItem.ToString().Equals("Stomp") || comboBox2.SelectedItem.ToString().Equals("Struggle Bug") || comboBox2.SelectedItem.ToString().Equals("Thunder") || comboBox2.SelectedItem.ToString().Equals("Thunder Fang") || comboBox2.SelectedItem.ToString().Equals("Thunderbolt") || comboBox2.SelectedItem.ToString().Equals("Thunder Punch") || comboBox2.SelectedItem.ToString().Equals("Thunder Shock") || comboBox2.SelectedItem.ToString().Equals("Tri Attack") || comboBox2.SelectedItem.ToString().Equals("Twineedle") || comboBox2.SelectedItem.ToString().Equals("Twister") || comboBox2.SelectedItem.ToString().Equals("Volt Tackle") || comboBox2.SelectedItem.ToString().Equals("Water Pulse") || comboBox2.SelectedItem.ToString().Equals("Waterfall") || comboBox2.SelectedItem.ToString().Equals("Zap Cannon") || comboBox2.SelectedItem.ToString().Equals("Zen Headbutt")))
                {
                    a = new Attacks(a.name, (int)(a.strength * 1.3), a.split, a.type);
                }
            }
            else if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Technician"))
            {
                if (a.strength <= 60)
                {
                    a = new Attacks(a.name, (int)(a.strength * 1.5), a.split, a.type);
                }
            }
            else if (comboBox6.SelectedItem != null && comboBox13.SelectedItem != null && ((comboBox6.SelectedItem.ToString().Equals("Swarm") && comboBox13.SelectedItem.ToString().Equals("bug")) || (comboBox6.SelectedItem.ToString().Equals("Blaze") && comboBox13.SelectedItem.ToString().Equals("fire")) || (comboBox6.SelectedItem.ToString().Equals("Torrent") && comboBox13.SelectedItem.ToString().Equals("water")) || (comboBox6.SelectedItem.ToString().Equals("Overgrow") && comboBox13.SelectedItem.ToString().Equals("grass"))))
            {
                a = new Attacks(a.name, (int)(a.strength * 1.5), a.split, a.type);
            }
            else if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Tough Claws") && (!comboBox2.SelectedItem.ToString().Equals("Barrage") && !comboBox2.SelectedItem.ToString().Equals("Beat Up") && !comboBox2.SelectedItem.ToString().Equals("Bonemerang") && !comboBox2.SelectedItem.ToString().Equals("Bone Club") && !comboBox2.SelectedItem.ToString().Equals("Bone Rush") && !comboBox2.SelectedItem.ToString().Equals("Bulldoze") && !comboBox2.SelectedItem.ToString().Equals("Bullet Seed") && !comboBox2.SelectedItem.ToString().Equals("Earthquake") && !comboBox2.SelectedItem.ToString().Equals("Egg Bomb") && !comboBox2.SelectedItem.ToString().Equals("Explosion") && !comboBox2.SelectedItem.ToString().Equals("Feint") && !comboBox2.SelectedItem.ToString().Equals("Fissure") && !comboBox2.SelectedItem.ToString().Equals("Fling") && !comboBox2.SelectedItem.ToString().Equals("Freeze Shock") && !comboBox2.SelectedItem.ToString().Equals("Fusion Bolt") && !comboBox2.SelectedItem.ToString().Equals("Gear Grind") && !comboBox2.SelectedItem.ToString().Equals("Gunk Shot") && !comboBox2.SelectedItem.ToString().Equals("Ice Shard") && !comboBox2.SelectedItem.ToString().Equals("Icicle Crash") && !comboBox2.SelectedItem.ToString().Equals("Icicle Spear") && !comboBox2.SelectedItem.ToString().Equals("Magnet Bomb") && !comboBox2.SelectedItem.ToString().Equals("Magnitude") && !comboBox2.SelectedItem.ToString().Equals("Metal Burst") && !comboBox2.SelectedItem.ToString().Equals("Natural Gift") && !comboBox2.SelectedItem.ToString().Equals("Pay Day") && !comboBox2.SelectedItem.ToString().Equals("Poison Sting") && !comboBox2.SelectedItem.ToString().Equals("Pin Missile") && !comboBox2.SelectedItem.ToString().Equals("Present") && !comboBox2.SelectedItem.ToString().Equals("Psycho Cut") && !comboBox2.SelectedItem.ToString().Equals("Razor Leaf") && !comboBox2.SelectedItem.ToString().Equals("Rock Blast") && !comboBox2.SelectedItem.ToString().Equals("Rock Slide") && !comboBox2.SelectedItem.ToString().Equals("Rock Throw") && !comboBox2.SelectedItem.ToString().Equals("Rock Tomb") && !comboBox2.SelectedItem.ToString().Equals("Rock Wrecker") && !comboBox2.SelectedItem.ToString().Equals("Sacred Fire") && !comboBox2.SelectedItem.ToString().Equals("Sand Tomb") && !comboBox2.SelectedItem.ToString().Equals("Secret Power") && !comboBox2.SelectedItem.ToString().Equals("Seed Bomb") && !comboBox2.SelectedItem.ToString().Equals("Self-Destruct") && !comboBox2.SelectedItem.ToString().Equals("Sky Attack") && !comboBox2.SelectedItem.ToString().Equals("Spike Cannon") && !comboBox2.SelectedItem.ToString().Equals("Smack Down") && !comboBox2.SelectedItem.ToString().Equals("Stone Edge") && !comboBox2.SelectedItem.ToString().Equals("Twineedle")))
            {
                if (a.split == 0)
                {
                    a = new Attacks(a.name, (int)(a.strength * 1.3), a.split, a.type);
                }
                else
                {
                    if (comboBox2.SelectedItem.ToString().Equals("Draining Kiss") || comboBox2.SelectedItem.ToString().Equals("Final Gambit") || comboBox2.SelectedItem.ToString().Equals("Grass Knot") || comboBox2.SelectedItem.ToString().Equals("Infestation") || comboBox2.SelectedItem.ToString().Equals("Petal Dance") || comboBox2.SelectedItem.ToString().Equals("Trump Card") || comboBox2.SelectedItem.ToString().Equals("Wring Out"))
                    {
                        a = new Attacks(a.name, (int)(a.strength * 1.3), a.split, a.type);
                    }
                }
            }
            else if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Solar Power"))
            {
                if (comboBox12.SelectedItem != null && comboBox12.SelectedItem.ToString().Equals("Sun"))
                {
                    a = new Attacks(a.name, (int)(a.strength * 1.5), a.split, a.type);
                }
            }
            if (comboBox7.SelectedItem != null && comboBox7.SelectedItem.ToString().Equals("Thick Fat"))
            {
                if (comboBox13.SelectedItem != null && (comboBox13.SelectedItem.ToString().Equals("ice") || comboBox13.SelectedItem.ToString().Equals("fire")))
                {
                    a = new Attacks(a.name, (int)(a.strength * 0.5), a.split, a.type);
                }
            }
            else if (comboBox7.SelectedItem != null && comboBox7.SelectedItem.ToString().Equals("Bulletproof"))
            {
                if (comboBox13.SelectedItem != null && (comboBox2.SelectedItem.ToString().Equals("Acid Spray") || comboBox2.SelectedItem.ToString().Equals("Aura Sphere") || comboBox2.SelectedItem.ToString().Equals("Barrage") || comboBox2.SelectedItem.ToString().Equals("Bullet Seed") || comboBox2.SelectedItem.ToString().Equals("Egg Bomb") || comboBox2.SelectedItem.ToString().Equals("Electro Ball") || comboBox2.SelectedItem.ToString().Equals("Energy Ball") || comboBox2.SelectedItem.ToString().Equals("Focus Blast") || comboBox2.SelectedItem.ToString().Equals("Gyro Ball") || comboBox2.SelectedItem.ToString().Equals("Ice Ball") || comboBox2.SelectedItem.ToString().Equals("Magnet Bomb") || comboBox2.SelectedItem.ToString().Equals("Mist Ball") || comboBox2.SelectedItem.ToString().Equals("Mud Bomb") || comboBox2.SelectedItem.ToString().Equals("Octazooka") || comboBox2.SelectedItem.ToString().Equals("Rock Wrecker") || comboBox2.SelectedItem.ToString().Equals("Searing Shot") || comboBox2.SelectedItem.ToString().Equals("Seed Bomb") || comboBox2.SelectedItem.ToString().Equals("Shadow Ball") || comboBox2.SelectedItem.ToString().Equals("Sludge Bomb") || comboBox2.SelectedItem.ToString().Equals("Weather Ball") || comboBox2.SelectedItem.ToString().Equals("Zap Cannon")))
                {
                    a = new Attacks(a.name, 0, a.split, a.type);
                    label46.Text = "0";
                }
            }
            return a;
        }


        private double checkopponenttype1()
        {
            if (!Typechart[label35.Text].ContainsKey(comboBox13.SelectedItem.ToString()))
            {
                return 1;
            }
            else
            {
                return Typechart[label35.Text][comboBox13.SelectedItem.ToString()];
            }
        }

        private double checkopponenttype1f()
        {
            if (!Typechart[label35.Text].ContainsKey(comboBox13.SelectedItem.ToString()))
            {
                return 1;
            }
            else
            {
                if (label35.Text.Equals("water"))
                {
                    return 2;
                }
                return Typechart[label35.Text][comboBox13.SelectedItem.ToString()];
            }
        }

        private double checkopponenttype1(string s)
        {
            if (!Typechart[label35.Text].ContainsKey(s))
            {
                return 1;
            }
            else
            {
                return Typechart[label35.Text][s];
            }
        }

        private double checkopponenttype2()
        {
            if (label36.Text.Equals("--") || label36.Text.Equals("-"))
            {
                return 1;
            }
            if (!Typechart[label36.Text].ContainsKey(comboBox13.SelectedItem.ToString()))
            {
                return 1;
            }
            else
            {
                return Typechart[label36.Text][comboBox13.SelectedItem.ToString()];
            }
        }

        private double checkopponenttype2f()
        {
            if (label36.Text.Equals("--") || label36.Text.Equals("-"))
            {
                return 1;
            }
            if (!Typechart[label36.Text].ContainsKey(comboBox13.SelectedItem.ToString()))
            {
                return 1;
            }
            else
            {
                if (label36.Text.Equals("water"))
                {
                    return 2;
                }
                return Typechart[label36.Text][comboBox13.SelectedItem.ToString()];
            }
        }

        private double checkopponenttype2(string s)
        {
            if (label36.Text.Equals("--") || label36.Text.Equals("-"))
            {
                return 1;
            }
            if (!Typechart[label36.Text].ContainsKey(s))
            {
                return 1;
            }
            else
            {
                return Typechart[label36.Text][s];
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            init();
            setup();
            stats();
            gifchange(0);
            gifchange(1);
            extraformset(0);
            extraformset(1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemCheck)
            {
                if (comboBox10.SelectedItem != null && !comboBox10.SelectedItem.Equals("Normal"))
                {
                    comboBox10.SelectedItem = "Normal";
                }
                itemChoice();
                itemDefend();
                modCheck();
                gifchange(0);
                Wesen1();
                Wesen2();
                stats();
                extraformset(0);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemCheck)
            {
                if (comboBox11.SelectedItem != null && !comboBox11.SelectedItem.Equals("Normal"))
                {
                    comboBox11.SelectedItem = "Normal";
                }
                itemChoice();
                itemDefend();
                modCheck();
                gifchange(1);
                Wesen1();
                Wesen2();
                stats();
                extraformset(1);
            }
        }

        private void extraformset(int a)
        {
            switch (a)
            {
                case 0:
                    changeItems(comboBox10, comboBox1);
                    break;
                case 1:
                    changeItems(comboBox11, comboBox3);
                    break;
            }
        }

        private void changeItems(ComboBox c, ComboBox cs)
        {
            string s;
            if (cs.SelectedItem == null)
            {
                s = "Bulbasaur";
            }
            else
            {
                s = cs.SelectedItem.ToString();
            }
            c.Items.Clear();
            c.Items.Add("Normal");
            if (matcherPokemon[s].sonderstats.Count != 0)
            {
                for (int i = 1; i <= matcherPokemon[s].sonderstats.Count; i++)
                {
                    c.Items.Add("Form: " + i);
                }
            }
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemtemp)
            {
                gifchange(0);
                stats();
            }
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemtemp)
            {
                gifchange(1);
                stats();
            }
        }
        Thread t3;
        private void button4_Click(object sender, EventArgs e)
        {
            t3 = new Thread(dlAttacks);
            t3.Start();
        }

        private void dlAttacks()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (!d.Exists)
            {
                d.Create();
            }
            WebClient wClientp = new WebClient();
            string physical = wClientp.DownloadString("http://www.serebii.net/attackdex-xy/physical.shtml");
            WebClient wClients = new WebClient();
            string special = wClients.DownloadString("http://www.serebii.net/attackdex-xy/special.shtml");
            FileInfo f;
            StreamWriter sw;

            f = new FileInfo(System.Environment.CurrentDirectory + "\\physical.log");
            if (!f.Exists)
            {
                FileStream fs = f.Create();
                fs.Close();
            }
            else
            {
                f.Delete();
                FileStream fs = f.Create();
                fs.Close();
            }


            sw = f.AppendText();
            sw.WriteLine(physical);
            sw.Close();

            f = new FileInfo(System.Environment.CurrentDirectory + "\\special.log");
            if (!f.Exists)
            {
                FileStream fs = f.Create();
                fs.Close();
            }
            else
            {
                f.Delete();
                FileStream fs = f.Create();
                fs.Close();
            }

            sw = f.AppendText();
            sw.WriteLine(special);
            sw.Close();
            MessageBox.Show("Finished!");
        }

        Thread t4;
        private void button5_Click(object sender, EventArgs e)
        {
            t4 = new Thread(crawlAttacks);
            t4.Start();
        }

        private void crawlAttacks()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");

            if (!d.Exists)
            {
                MessageBox.Show("First use Retrieve Data Attacks!");
            }
            else
            {
                FileInfo fp = new FileInfo(System.Environment.CurrentDirectory + "\\physical.log");
                FileInfo fs = new FileInfo(System.Environment.CurrentDirectory + "\\special.log");

                if (!fp.Exists || !fs.Exists)
                {
                    MessageBox.Show("First use Retrieve Data Attacks!");
                }
                else
                {
                    FileInfo f = new FileInfo(System.Environment.CurrentDirectory + "\\attacks.data");
                    if (!f.Exists)
                    {
                        FileStream ft = f.Create();
                        ft.Close();
                    }
                    else
                    {
                        f.Delete();
                        FileStream ft = f.Create();
                        ft.Close();
                    }
                    StreamWriter sw = f.AppendText();
                    StreamReader sr = new StreamReader(fp.OpenRead());
                    bool b = false;
                    Dictionary<int, bool> check = new Dictionary<int, bool>();
                    int count = 0;
                    string name = "";
                    int strength = 0;
                    string type = "";
                    while (!sr.EndOfStream)
                    {
                        count++;
                        string s = sr.ReadLine();
                        if (s.Contains("fooinfo"))
                        {
                            b = !b;
                            if (b)
                            {
                                if (!name.Equals(""))
                                {
                                    sw.WriteLine(name + "~" + strength + "~" + 0 + "~" + type);
                                }
                                check.Add(count + 1, true);
                                check.Add(count + 4, true);
                                check.Add(count + 13, true);
                            }
                        }
                        if (check.ContainsKey(count))
                        {
                            if (s.Contains("<a h=\"/attackdex-xy/"))
                            {
                                name = s.Substring(s.IndexOf(">") + 1, s.IndexOf("</") - (s.IndexOf(">") + 1));
                            }
                            else if (s.Contains("<img src="))
                            {
                                type = s.Substring(s.IndexOf("/type/") + 6, s.IndexOf(".gif") - (s.IndexOf("/type/") + 6));
                            }
                            else
                            {
                                s.Trim();
                                while (s.Contains("\t"))
                                {
                                    s = s.Substring(2);
                                }
                                if (s.Equals("--"))
                                {
                                    strength = 0;
                                }
                                else
                                {
                                    strength = int.Parse(s);
                                }
                            }
                        }
                    }
                    sr.Close();

                    sr = new StreamReader(fs.OpenRead());
                    b = false;
                    check = new Dictionary<int, bool>();
                    count = 0;
                    name = "";
                    strength = 0;
                    type = "";
                    while (!sr.EndOfStream)
                    {
                        count++;
                        string s = sr.ReadLine();
                        if (s.Contains("fooinfo"))
                        {
                            b = !b;
                            if (b)
                            {
                                if (!name.Equals(""))
                                {
                                    sw.WriteLine(name + "~" + strength + "~" + 1 + "~" + type);
                                }
                                check.Add(count + 1, true);
                                check.Add(count + 4, true);
                                check.Add(count + 13, true);
                            }
                        }
                        if (check.ContainsKey(count))
                        {
                            if (s.Contains("<a h=\"/attackdex-xy/"))
                            {
                                name = s.Substring(s.IndexOf(">") + 1, s.IndexOf("</") - (s.IndexOf(">") + 1));
                            }
                            else if (s.Contains("<img src="))
                            {
                                type = s.Substring(s.IndexOf("/type/") + 6, s.IndexOf(".gif") - (s.IndexOf("/type/") + 6));
                            }
                            else
                            {
                                s.Trim();
                                while (s.Contains("\t"))
                                {
                                    s = s.Substring(2);
                                }
                                if (s.Equals("--"))
                                {
                                    strength = 0;
                                }
                                else
                                {
                                    strength = int.Parse(s);
                                }
                            }
                        }
                    }
                    sr.Close();
                    sw.Close();
                }
            }
            MessageBox.Show("Finished!");
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }
        bool itemCheck = true;
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            itemCheck = false;
            itemChoice();
            itemDefend();
            modCheck();
            visualAttack();
            if(stopE){ Wesen1(); Wesen2(); stats(); }
            itemCheck = true;
        }

        private void visualAttack()
        {
            if (comboBox2.SelectedItem != null && matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
            {
                if (matcherAttacks[comboBox2.SelectedItem.ToString()].strength == 0)
                {
                    textBox27.Text = "--";
                }
                else
                {
                    textBox27.Text = matcherAttacks[comboBox2.SelectedItem.ToString()].strength + "";
                }
                comboBox13.SelectedItem = matcherAttacks[comboBox2.SelectedItem.ToString()].type;
            }
            else
            {
                textBox27.Text = "0";
                comboBox13.SelectedItem = "0";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }
        Thread t5;
        private void button6_Click(object sender, EventArgs e)
        {
            t5 = new Thread(getItemsSerebii);
            t5.Start();
        }

        private void getItemsSerebii()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (!d.Exists)
            {
                d.Create();
            }
            WebClient wClientp = new WebClient();
            string items = wClientp.DownloadString("http://www.serebii.net/xy/items.shtml");
            FileInfo f;
            StreamWriter sw;

            f = new FileInfo(System.Environment.CurrentDirectory + "\\items.log");
            if (!f.Exists)
            {
                FileStream fs = f.Create();
                fs.Close();
            }
            else
            {
                f.Delete();
                FileStream fs = f.Create();
                fs.Close();
            }


            sw = f.AppendText();
            sw.WriteLine(items);
            sw.Close();

            MessageBox.Show("Finished!");
        }
        Thread t6;
        private void button7_Click(object sender, EventArgs e)
        {
            t6 = new Thread(buildDataItems);
            t6.Start();
        }

        private void buildDataItems()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");

            if (!d.Exists)
            {
                MessageBox.Show("First use Retrieve Data Items!");
            }
            else
            {
                FileInfo fi = new FileInfo(System.Environment.CurrentDirectory + "\\items.log");

                if (!fi.Exists)
                {
                    MessageBox.Show("First use Retrieve Data Items!");
                }
                else
                {
                    FileInfo f = new FileInfo(System.Environment.CurrentDirectory + "\\Items.data");
                    matcherItems = new Dictionary<string, Items>();
                    if (!f.Exists)
                    {
                        FileStream ft = f.Create();
                        ft.Close();
                    }
                    else
                    {
                        f.Delete();
                        FileStream ft = f.Create();
                        ft.Close();
                    }
                    StreamWriter sw = f.AppendText();
                    StreamReader sr = new StreamReader(fi.OpenRead());
                    string name = "";
                    string description = "";
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        if (s.Contains("<td class=\"fooinfo\"><a h=\"/itemdex/"))
                        {
                            if (!name.Equals(""))
                            {
                                sw.WriteLine(name + "~" + description);
                            }
                            name = s.Substring(s.IndexOf(".shtml") + 8, s.IndexOf("</") - (s.IndexOf(".shtml") + 8));
                        }
                        else if (s.Contains("<td class=\"fooinfo\">"))
                        {
                            description = s.Substring(s.IndexOf("<td class=\"fooinfo\">") + 20, s.IndexOf("</") - (s.IndexOf("<td class=\"fooinfo\">") + 20));
                        }
                    }
                    MessageBox.Show("Finished!");
                    sw.Close();
                    sr.Close();
                }
            }
        }
        bool itemtempa = true;
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemtempa)
            {
                itemCheck = false;
                itemtempa = false;
                itemChoice();
                itemDefend();
                modCheck();
                Wesen1();
                Wesen2();
                stats();
                itemtempa = true;
                itemCheck = true;
            }
        }

        private void itemChoice()
        {
            atkmod = 1;
            mod1 = 1;
            mod2 = 1;

            mod3 = 1;
            try
            {
                if (matcherItems.ContainsKey(comboBox4.SelectedItem.ToString()))
                {
                    label38.Text = matcherItems[comboBox4.SelectedItem.ToString()].description;
                    if (comboBox4.SelectedItem.ToString().Contains("Plate"))
                    {
                        comboBox4.SelectedItem = "Damage Plate";
                        atkmod = atkmod * 1.2;
                    }
                    else if (comboBox4.SelectedItem.ToString().Contains("Gem"))
                    {
                        comboBox4.SelectedItem = "Damage Gem";
                        atkmod = atkmod * 1.3;
                    }
                    else
                    {
                        switch (comboBox4.SelectedItem.ToString())
                        {
                            case "Life Orb":
                                mod3 = mod3 * 1.3;
                                break;
                            case "Choice Band":
                                if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                                {
                                    if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 0)
                                    {
                                        atkmod = atkmod * 1.5;
                                    }
                                }
                                break;
                            case "Choice Specs":
                                if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                                {
                                    if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 1)
                                    {
                                        atkmod = atkmod * 1.5;
                                    }
                                }
                                break;
                            case "Light Ball":
                                if (comboBox1.SelectedItem.ToString().Equals("Pikachu"))
                                {
                                    atkmod = atkmod * 2;
                                }
                                break;
                            case "Thick Club":
                                if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                                {
                                    if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 0)
                                    {
                                        if (comboBox1.SelectedItem.ToString().Equals("Marowak"))
                                        {
                                            atkmod = atkmod * 2;
                                        }
                                    }
                                }
                                break;
                            case "Soul Dew":
                                if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                                {
                                    if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 1)
                                    {
                                        if (comboBox1.SelectedItem.ToString().Equals("Latios") || comboBox1.SelectedItem.ToString().Equals("Latias"))
                                        {
                                            atkmod = atkmod * 1.5;
                                        }
                                    }
                                }
                                break;
                            case "Deepseatooth":
                                if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                                {
                                    if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 1)
                                    {
                                        if (comboBox1.SelectedItem.ToString().Equals("Clamperl"))
                                        {
                                            atkmod = atkmod * 2;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        private void itemDefend()
        {
            defmod = 1;
            try
            {
                if (matcherItems.ContainsKey(comboBox5.SelectedItem.ToString()))
                {
                    label37.Text = matcherItems[comboBox5.SelectedItem.ToString()].description;
                    if (comboBox5.SelectedItem.ToString().Contains("Berry"))
                    {
                        comboBox5.SelectedItem = "Damage Berry";
                        mod3 = mod3 * 0.5;
                    }
                    else
                    {

                        switch (comboBox5.SelectedItem.ToString())
                        {
                            case "Deepseascale":
                                if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                                {
                                    if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 1)
                                    {
                                        if (comboBox3.SelectedItem.ToString().Equals("Clamperl"))
                                        {
                                            defmod = defmod * 2;
                                        }
                                    }
                                }
                                break;
                            case "Eviolite":
                                defmod = defmod * 1.5;
                                break;
                            case "Soul Dew":
                                if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                                {
                                    if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 1)
                                    {
                                        if (comboBox1.SelectedItem.ToString().Equals("Latios") || comboBox1.SelectedItem.ToString().Equals("Latias"))
                                        {
                                            defmod = defmod * 1.5;
                                        }
                                    }
                                }
                                break;
                            case "Assault Vest":
                                if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                                {
                                    if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 1)
                                    {
                                        defmod = defmod * 1.5;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        bool itemtemp = true;
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemtemp)
            {
                itemCheck = false;
                itemtemp = false;
                itemChoice();
                itemDefend();
                modCheck();
                Wesen1();
                Wesen2();
                stats();
                itemtemp = true;
                itemCheck = true;
            }
        }

        Thread t7;

        private void button8_Click(object sender, EventArgs e)
        {
            t7 = new Thread(getNaturesSerebii);
            t7.Start();
        }

        private void getNaturesSerebii()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (!d.Exists)
            {
                d.Create();
            }
            WebClient wClientp = new WebClient();
            string items = wClientp.DownloadString("http://www.serebii.net/pokemon_advance/personality.shtml");
            FileInfo f;
            StreamWriter sw;

            f = new FileInfo(System.Environment.CurrentDirectory + "\\natures.log");
            if (!f.Exists)
            {
                FileStream fs = f.Create();
                fs.Close();
            }
            else
            {
                f.Delete();
                FileStream fs = f.Create();
                fs.Close();
            }


            sw = f.AppendText();
            sw.WriteLine(items);
            sw.Close();

            MessageBox.Show("Finished!");
        }

        Thread t8;

        private void button9_Click(object sender, EventArgs e)
        {
            t8 = new Thread(buildDataNatures);
            t8.Start();
        }

        private void buildDataNatures()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");

            if (!d.Exists)
            {
                MessageBox.Show("First use Retrieve Data Natures!");
            }
            else
            {
                FileInfo fi = new FileInfo(System.Environment.CurrentDirectory + "\\natures.log");

                if (!fi.Exists)
                {
                    MessageBox.Show("First use Retrieve Data Natures!");
                }
                else
                {
                    FileInfo f = new FileInfo(System.Environment.CurrentDirectory + "\\Natures.data");
                    if (!f.Exists)
                    {
                        FileStream ft = f.Create();
                        ft.Close();
                    }
                    else
                    {
                        f.Delete();
                        FileStream ft = f.Create();
                        ft.Close();
                    }
                    StreamWriter sw = f.AppendText();
                    StreamReader sr = new StreamReader(fi.OpenRead());
                    string name = "";
                    string raises = "";
                    string lowers = "";
                    int count = 0;
                    int countdata = 0;
                    Dictionary<int, bool> pos = new Dictionary<int, bool>();
                    bool b = false;
                    while (!sr.EndOfStream)
                    {
                        count++;
                        string s = sr.ReadLine();
                        if (s.Contains("Natures Name"))
                        {
                            b = true;
                        }
                        if (b)
                        {
                            if (s.Contains("<tr>"))
                            {
                                pos.Add(count + 2, true);
                                pos.Add(count + 5, true);
                                pos.Add(count + 8, true);
                            }
                        }
                        if (countdata == 0 && pos.ContainsKey(count))
                        {
                            if (!name.Equals(""))
                            {
                                sw.WriteLine(name.Trim() + "~" + raises.Trim() + "~" + lowers.Trim());
                            }
                            while (s.Contains("\t") && s.IndexOf("\t") == 0)
                            {
                                s = s.Substring(2);
                            }
                            if (s.Contains("\t"))
                            {
                                name = s.Substring(0, s.IndexOf("\t"));
                            }
                            else
                            {
                                name = s;
                            }
                            countdata = 1;
                        }
                        else if (countdata == 1 && pos.ContainsKey(count))
                        {
                            while (s.Contains("\t") && s.IndexOf("\t") == 0)
                            {
                                s = s.Substring(2);
                            }
                            if (s.Contains("\t"))
                            {
                                raises = s.Substring(0, s.IndexOf("\t"));
                            }
                            else
                            {
                                raises = s;
                            }
                            countdata = 2;
                        }
                        else if (countdata == 2 && pos.ContainsKey(count))
                        {
                            while (s.Contains("\t") && s.IndexOf("\t") == 0)
                            {
                                s = s.Substring(2);
                            }
                            if (s.Contains("\t"))
                            {
                                lowers = s.Substring(0, s.IndexOf("\t"));
                            }
                            else
                            {
                                lowers = s;
                            }
                            countdata = 0;
                        }
                    }
                    MessageBox.Show("Finished!");
                    sw.Close();
                    sr.Close();
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            clean();
        }

        private void clean()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (d.Exists)
            {
                foreach (FileInfo f in d.GetFiles())
                {
                    if (f.ToString().Contains(".log"))
                    {
                        f.Delete();
                    }
                }
            }
            MessageBox.Show("Finished!");
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stopE)
            {
                itemChoice();
                itemDefend();
                modCheck();
                Wesen1();
                Wesen2();
                stats();
            }
        }

        private void Wesen1()
        {
            try
            {
                string raises = matcherNatures[comboBox8.SelectedItem.ToString()].raises;
                string lowers = matcherNatures[comboBox8.SelectedItem.ToString()].lowers;
                Wesena = 1;
                Wesend = 1;
                Wesensa = 1;
                Wesensd = 1;
                Wesenspe = 1;
                switch (raises.Trim())
                {
                    case "Attack":
                        Wesena = 1.1;
                        break;
                    case "Defense":
                        Wesend = 1.1;
                        break;
                    case "Special Attack":
                        Wesensa = 1.1;
                        break;
                    case "Special Defense":
                        Wesensd = 1.1;
                        break;
                    case "Speed":
                        Wesenspe = 1.1;
                        break;
                    case "None":
                        break;
                }
                switch (lowers.Trim())
                {
                    case "Attack":
                        Wesena = 0.9;
                        break;
                    case "Defense":
                        Wesend = 0.9;
                        break;
                    case "Special Attack":
                        Wesensa = 0.9;
                        break;
                    case "Special Defense":
                        Wesensd = 0.9;
                        break;
                    case "Speed":
                        Wesenspe = 0.9;
                        break;
                    case "None":
                        break;
                }
            }
            catch (NullReferenceException)
            {
                Wesena = 1;
                Wesend = 1;
                Wesensa = 1;
                Wesensd = 1;
                Wesenspe = 1;
            }
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stopE)
            {
                itemChoice();
                itemDefend();
                modCheck();
                Wesen1();
                Wesen2();
                stats();
            }
        }

        private void Wesen2()
        {
            try
            {
                string raises = matcherNatures[comboBox9.SelectedItem.ToString().Trim()].raises;
                string lowers = matcherNatures[comboBox9.SelectedItem.ToString().Trim()].lowers;
                BWesena = 1;
                BWesend = 1;
                BWesensa = 1;
                BWesensd = 1;
                BWesenspe = 1;
                switch (raises.Trim())
                {
                    case "Attack":
                        BWesena = 1.1;
                        break;
                    case "Defense":
                        BWesend = 1.1;
                        break;
                    case "Special Attack":
                        BWesensa = 1.1;
                        break;
                    case "Special Defense":
                        BWesensd = 1.1;
                        break;
                    case "Speed":
                        BWesenspe = 1.1;
                        break;
                    case "None":
                        break;
                }
                switch (lowers.Trim())
                {
                    case "Attack":
                        BWesena = 0.9;
                        break;
                    case "Defense":
                        BWesend = 0.9;
                        break;
                    case "Special Attack":
                        BWesensa = 0.9;
                        break;
                    case "Special Defense":
                        BWesensd = 0.9;
                        break;
                    case "Speed":
                        BWesenspe = 0.9;
                        break;
                    case "None":
                        break;
                }
            }
            catch (NullReferenceException)
            {
                BWesena = 1;
                BWesend = 1;
                BWesensa = 1;
                BWesensd = 1;
                BWesenspe = 1;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                FileInfo ftest;
                FileInfo f;
                DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
                bool b = false;
                if (!d.Exists)
                {
                    d.Create();
                    b = true;
                }
                ftest = new FileInfo(System.Environment.CurrentDirectory + "\\Completed.data");
                if (ftest.Exists)
                {
                    ftest.Delete();
                }
                ftest = new FileInfo(System.Environment.CurrentDirectory + "\\Pokemon.data");
                OpenFileDialog dlgFileOpen = new OpenFileDialog();
                if (!ftest.Exists)
                {
                    b = true;
                    dlgFileOpen.Title = "Open now the Pokemon.data";
                    dlgFileOpen.Filter = "Pokemon Data|Pokemon.data";
                    dlgFileOpen.ShowDialog();
                    f = new FileInfo(dlgFileOpen.FileName);
                    f.CopyTo(System.Environment.CurrentDirectory + "\\Pokemon.data");
                }
                ftest = new FileInfo(System.Environment.CurrentDirectory + "\\Items.data");
                if (!ftest.Exists)
                {
                    b = true;
                    dlgFileOpen = new OpenFileDialog();
                    dlgFileOpen.Title = "Open now the Items.data";
                    dlgFileOpen.Filter = "Items Data|Items.data";
                    dlgFileOpen.ShowDialog();
                    f = new FileInfo(dlgFileOpen.FileName);
                    f.CopyTo(System.Environment.CurrentDirectory + "\\Items.data");
                }
                ftest = new FileInfo(System.Environment.CurrentDirectory + "\\attacks.data");
                if (!ftest.Exists)
                {
                    b = true;
                    dlgFileOpen = new OpenFileDialog();
                    dlgFileOpen.Title = "Open now the attacks.data";
                    dlgFileOpen.Filter = "Attacks Data|attacks.data";
                    dlgFileOpen.ShowDialog();
                    f = new FileInfo(dlgFileOpen.FileName);
                    f.CopyTo(System.Environment.CurrentDirectory + "\\attacks.data");
                }
                ftest = new FileInfo(System.Environment.CurrentDirectory + "\\Natures.data");
                if (!ftest.Exists)
                {
                    b = true;
                    dlgFileOpen = new OpenFileDialog();
                    dlgFileOpen.Title = "Open now the Natures.data";
                    dlgFileOpen.Filter = "Natures Data|Natures.data";
                    dlgFileOpen.ShowDialog();
                    f = new FileInfo(dlgFileOpen.FileName);
                    f.CopyTo(System.Environment.CurrentDirectory + "\\Natures.data");
                }
                ftest = new FileInfo(System.Environment.CurrentDirectory + "\\Abilities.data");
                if (!ftest.Exists)
                {
                    b = true;
                    dlgFileOpen = new OpenFileDialog();
                    dlgFileOpen.Title = "Open now the Abilities.data";
                    dlgFileOpen.Filter = "Abilities Data|Abilities.data";
                    dlgFileOpen.ShowDialog();
                    f = new FileInfo(dlgFileOpen.FileName);
                    f.CopyTo(System.Environment.CurrentDirectory + "\\Abilities.data");
                }
                try
                {
                    d = new DirectoryInfo(System.Environment.CurrentDirectory + "\\gifs");
                    if (!d.Exists)
                    {
                        d.Create();
                        b = true;
                        FolderBrowserDialog dlgFolderOpen = new FolderBrowserDialog();
                        dlgFolderOpen.Description = "Open now the gifs Foler";
                        dlgFolderOpen.ShowDialog();
                        d = new DirectoryInfo(dlgFolderOpen.SelectedPath);
                        foreach (FileInfo ft in d.GetFiles())
                        {
                            ft.CopyTo(System.Environment.CurrentDirectory + "\\gifs\\" + ft.Name);
                        }
                    }
                }
                catch (ArgumentException)
                {
                    try
                    {
                        using (ZipFile zip = ZipFile.Read(System.Environment.CurrentDirectory + "\\gifs.zip"))
                        {
                            zip.ExtractAll(System.Environment.CurrentDirectory + "\\");
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
                if (b)
                {
                    init();
                    setup();
                    stats();
                    gifchange(0);
                    gifchange(1);
                    extraformset(0);
                    extraformset(1);
                }
                if (!checkBox2.Checked)
                {
                    button11.Visible = false;
                }
            }
            catch (ArgumentException)
            {

            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox2.Checked)
                {
                    button6.Visible = true;
                    button4.Visible = true;
                    button1.Visible = true;
                    button7.Visible = true;
                    button5.Visible = true;
                    button2.Visible = true;
                    button8.Visible = true;
                    button3.Visible = true;
                    button9.Visible = true;
                    button10.Visible = true;
                    label41.Visible = true;
                    button12.Visible = true;
                    button13.Visible = true;
                    button11.Visible = true;
                    button14.Visible = true;
                    button15.Visible = true;
                    button16.Visible = true;
                    button17.Visible = true;
                    button29.Visible = true;
                    button28.Visible = true;
                    button27.Visible = true;
                    button18.Visible = false;
                    button19.Visible = false;
                    button20.Visible = false;
                    button21.Visible = false;
                    button22.Visible = false;
                    checkBox6.Visible = false;
                    textBox33.Visible = false;
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = false;
                    button23.Visible = false;
                    button24.Visible = false;
                }
                else
                {
                    button6.Visible = false;
                    button4.Visible = false;
                    button1.Visible = false;
                    button7.Visible = false;
                    button5.Visible = false;
                    button2.Visible = false;
                    button8.Visible = false;
                    button3.Visible = false;
                    button9.Visible = false;
                    button10.Visible = false;
                    label41.Visible = false;
                    button12.Visible = false;
                    button13.Visible = false;
                    button11.Visible = false;
                    button14.Visible = false;
                    button15.Visible = false;
                    button16.Visible = false;
                    button17.Visible = false;
                    button29.Visible = false;
                    button28.Visible = false;
                    button27.Visible = false;
                    button18.Visible = true;
                    button19.Visible = true;
                    button20.Visible = true;
                    button21.Visible = true;
                    button22.Visible = true;
                    checkBox6.Visible = true;
                    textBox33.Visible = true;
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = true;
                    button23.Visible = true;
                    button24.Visible = true;
                }
            }
            catch (ArgumentException)
            {
            }

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            itemChoice();
            itemDefend();
            modCheck();
            Wesen1();
            Wesen2();
            stats();
        }

        private void modCheck()
        {
            try
            {
                if (checkBox3.Checked)
                {

                    if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                    {
                        if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 0)
                        {
                            if (comboBox6.SelectedItem != null && (comboBox6.SelectedItem.ToString().Equals("Guts") || comboBox6.SelectedItem.ToString().Equals("Toxic Boost")))
                            {
                                mod1 = mod1 * 1.5;
                            }
                            else if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString().Equals("Anti Burn"))
                            {
                            }
                            else
                            {
                                mod1 = mod1 * 0.5;
                            }
                            if (comboBox7.SelectedItem != null && comboBox7.SelectedItem.ToString().Equals("Marvel Scale"))
                            {
                                defmod = defmod * 1.5;
                            }
                        }
                    }
                }
                if (checkBox4.Checked)
                {
                    if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                    {
                        if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 0)
                        {
                            mod1 = mod1 * 0.5;
                        }
                    }
                }
                if (checkBox5.Checked)
                {
                    if (matcherAttacks.ContainsKey(comboBox2.SelectedItem.ToString()))
                    {
                        if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 1)
                        {
                            mod1 = mod1 * 0.5;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            try
            {
                switch (comboBox12.SelectedItem.ToString())
                {
                    case "Sun":
                        if (comboBox13.SelectedItem.Equals("water"))
                        {
                            atkmod = atkmod * 0.5;
                        }
                        if (comboBox13.SelectedItem.Equals("fire"))
                        {
                            atkmod = atkmod * 1.5;
                        }
                        break;
                    case "Rain":
                        if (comboBox13.SelectedItem.Equals("water"))
                        {
                            atkmod = atkmod * 1.5;
                        }
                        if (comboBox13.SelectedItem.Equals("fire"))
                        {
                            atkmod = atkmod * 0.5;
                        }
                        break;
                    case "Sand":
                        if (label35.Text.Equals("rock") || label36.Text.Equals("rock"))
                        {
                            defmod = defmod * 1.5;
                        }
                        break;
                    case "Hail":
                        break;
                    default:
                        break;
                }
            }
            catch (NullReferenceException)
            {
            }

            try
            {
                switch (comboBox6.SelectedItem.ToString())
                {
                    case "Compound Eyes":
                        if (checkopponenttype1() * checkopponenttype2() < 1)
                        {
                            mod3 = mod3 * 2;
                        }
                        break;
                    case "Parental Bond":
                        atkmod = atkmod * 1.5;
                        break;
                    default:
                        break;
                }
            }
            catch (NullReferenceException)
            {
            }

            try
            {
                switch (comboBox7.SelectedItem.ToString())
                {
                    case "Heat Proof":
                        if (comboBox13.SelectedItem != null && comboBox13.SelectedItem.ToString().Equals("fire"))
                        {
                            mod3 = mod3 * 0.5;
                        }
                        break;
                    case "Levitate":
                        if (comboBox13.SelectedItem != null && comboBox13.SelectedItem.ToString().Equals("ground"))
                        {
                            mod3 = mod3 * 0;
                        }
                        break;
                    case "Filter":
                        if (checkopponenttype1() * checkopponenttype2() > 1)
                        {
                            mod3 = mod3 * 0.75;
                        }
                        break;
                    case "Solid Rock":
                        if (checkopponenttype1() * checkopponenttype2() > 1)
                        {
                            mod3 = mod3 * 0.75;
                        }
                        break;
                    case "Fur Coat":
                        if (matcherAttacks[comboBox2.SelectedItem.ToString()].split == 0)
                        {
                            mod3 = mod3 * 0.5;
                        }
                        break;
                    case "Multiscale":
                        mod3 = mod3 * 0.5;
                        break;
                    default:
                        break;
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        private void textBox26_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox25_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox23_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            stats();
        }

        Thread t9;

        private void button12_Click(object sender, EventArgs e)
        {
            t9 = new Thread(getAbilitiesSerebii);
            t9.Start();
        }

        private void getAbilitiesSerebii()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (!d.Exists)
            {
                d.Create();
            }
            WebClient wClientp = new WebClient();
            string items = wClientp.DownloadString("http://www.serebii.net/games/ability.shtml");
            FileInfo f;
            StreamWriter sw;

            f = new FileInfo(System.Environment.CurrentDirectory + "\\abilities.log");
            if (!f.Exists)
            {
                FileStream fs = f.Create();
                fs.Close();
            }
            else
            {
                f.Delete();
                FileStream fs = f.Create();
                fs.Close();
            }


            sw = f.AppendText();
            sw.WriteLine(items);
            sw.Close();

            items = wClientp.DownloadString("http://www.serebii.net/blackwhite/ability.shtml");


            f = new FileInfo(System.Environment.CurrentDirectory + "\\abilities1.log");
            if (!f.Exists)
            {
                FileStream fs = f.Create();
                fs.Close();
            }
            else
            {
                f.Delete();
                FileStream fs = f.Create();
                fs.Close();
            }


            sw = f.AppendText();
            sw.WriteLine(items);
            sw.Close();

            items = wClientp.DownloadString("http://www.serebii.net/xy/abilities.shtml");


            f = new FileInfo(System.Environment.CurrentDirectory + "\\abilities2.log");
            if (!f.Exists)
            {
                FileStream fs = f.Create();
                fs.Close();
            }
            else
            {
                f.Delete();
                FileStream fs = f.Create();
                fs.Close();
            }


            sw = f.AppendText();
            sw.WriteLine(items);
            sw.Close();

            MessageBox.Show("Finished!");
        }

        Thread t10;

        private void button13_Click(object sender, EventArgs e)
        {
            t10 = new Thread(buildDataAbilities);
            t10.Start();
        }

        private void buildDataAbilities()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");

            if (!d.Exists)
            {
                MessageBox.Show("First use Retrieve Data Abilities!");
            }
            else
            {
                FileInfo fi = new FileInfo(System.Environment.CurrentDirectory + "\\abilities.log");

                if (!fi.Exists)
                {
                    MessageBox.Show("First use Retrieve Data Abilities!");
                }
                else
                {
                    FileInfo f = new FileInfo(System.Environment.CurrentDirectory + "\\Abilities.data");
                    if (!f.Exists)
                    {
                        FileStream ft = f.Create();
                        ft.Close();
                    }
                    else
                    {
                        f.Delete();
                        FileStream ft = f.Create();
                        ft.Close();
                    }
                    StreamWriter sw = f.AppendText();
                    StreamReader sr = new StreamReader(fi.OpenRead());
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        if (s.Contains("<td colspan=\"2\"><a name=\""))
                        {
                            if (s.Contains("</b>"))
                            {
                                sw.WriteLine(s.Substring(s.IndexOf("<b>") + 3, s.IndexOf("</b>") - (s.IndexOf("<b>") + 3)));
                            }
                            else
                            {
                                sw.WriteLine(s.Substring(s.IndexOf("<b>") + 3, s.IndexOf("</a>") - (s.IndexOf("<b>") + 3)));
                            }
                        }
                    }
                    sr.Close();
                    fi = new FileInfo(System.Environment.CurrentDirectory + "\\abilities1.log");
                    sr = new StreamReader(fi.OpenRead());
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        if (s.Contains("<td colspan=\"5\" class=\"fooleft\"><font size=\"3\"><a h=\"/abilitydex/"))
                        {
                            sw.WriteLine(s.Substring(s.IndexOf("<b>") + 3, s.IndexOf("</b>") - (s.IndexOf("<b>") + 3)));
                        }
                    }
                    sr.Close();
                    fi = new FileInfo(System.Environment.CurrentDirectory + "\\abilities2.log");
                    sr = new StreamReader(fi.OpenRead());
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        if (s.Contains("<a h=\"/abilitydex/"))
                        {
                            sw.WriteLine(s.Substring(s.IndexOf(">") + 1, s.IndexOf("</a>") - (s.IndexOf(">") + 1)));
                        }
                    }
                    MessageBox.Show("Finished!");
                    sw.Close();
                    sr.Close();
                }
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            itemChoice();
            itemDefend();
            modCheck();
            Wesen1();
            Wesen2();
            stats();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            itemChoice();
            itemDefend();
            modCheck();
            Wesen1();
            Wesen2();
            stats();
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            itemChoice();
            itemDefend();
            modCheck();
            stats();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            itemChoice();
            itemDefend();
            modCheck();
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            itemChoice();
            itemDefend();
            modCheck();
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox3.Items.Clear();
            setup();
        }

        private void textBox33_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox35_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox36_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox37_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox38_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox29_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox30_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox31_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        private void textBox32_TextChanged(object sender, EventArgs e)
        {
            if(stopE){ Wesen1(); Wesen2(); stats(); }
        }

        Thread t11;

        private void button14_Click(object sender, EventArgs e)
        {
            t11 = new Thread(getParentSitegif);
            t11.Start();
        }

        private void getParentSitegif()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (!d.Exists)
            {
                d.Create();
            }
            WebClient wClientp = new WebClient();
            string items = wClientp.DownloadString("http://play.pokemonshowdown.com/sprites/xyani/");
            FileInfo f;
            StreamWriter sw;

            f = new FileInfo(System.Environment.CurrentDirectory + "\\gifparent.log");
            if (!f.Exists)
            {
                FileStream fs = f.Create();
                fs.Close();
            }
            else
            {
                f.Delete();
                FileStream fs = f.Create();
                fs.Close();
            }


            sw = f.AppendText();
            sw.WriteLine(items);
            sw.Close();

            MessageBox.Show("Finished!");
        }

        Thread t12;

        private void button15_Click(object sender, EventArgs e)
        {
            t12 = new Thread(downloadgif);
            t12.Start();
        }

        private void downloadgif()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");

            if (!d.Exists)
            {
                MessageBox.Show("First use Get Gif Parent Site!");
            }
            else
            {
                FileInfo fi = new FileInfo(System.Environment.CurrentDirectory + "\\gifparent.log");

                if (!fi.Exists)
                {
                    MessageBox.Show("First use Get Gif Parent Site!");
                }
                else
                {
                    d = new DirectoryInfo(System.Environment.CurrentDirectory + "\\gifs");
                    if (!d.Exists)
                    {
                        d.Create();
                    }

                    StreamReader sr = new StreamReader(fi.OpenRead());

                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        if (s.Contains("<tr><td valign=\"top\"><img src=\"/icons/image2.gif\" alt=\"[IMG]\"></td><td><a h=\""))
                        {
                            string temp = s.Substring(s.IndexOf("<a h=\"") + 9, s.IndexOf(".gif\">") + 4 - (s.IndexOf("<a h=\"") + 9));
                            WebClient wClientp = new WebClient();
                            wClientp.DownloadFile("http://play.pokemonshowdown.com/sprites/xyani/" + temp, System.Environment.CurrentDirectory + "\\gifs\\" + temp);

                        }
                    }
                    sr.Close();
                    MessageBox.Show("Finished!");
                }
            }
        }

        private void gifchange(int c)
        {
            if (Directory.Exists(System.Environment.CurrentDirectory + "\\gifs"))
            {
                string s;
                switch (c)
                {
                    case 0:                         //Angreifer
                        if (comboBox1.SelectedItem == null)
                        {
                            comboBox1.SelectedItem = "Bulbasaur";
                        }
                        s = comboBox1.SelectedItem.ToString();
                        s = s.ToLower();
                        s = s.Replace("♀", "f");
                        s = s.Replace("♂", "m");
                        s = s.Replace("é", "e");
                        if (comboBox10.SelectedItem == null || comboBox10.SelectedItem.ToString().Equals("Normal"))
                        {
                            if (File.Exists(System.Environment.CurrentDirectory + "\\gifs\\" + s + ".gif"))
                            {
                                pictureBox1.Visible = true;
                                using (var fs = new System.IO.FileStream(System.Environment.CurrentDirectory + "\\gifs\\" + s + ".gif", System.IO.FileMode.Open, System.IO.FileAccess.Read))
                                {
                                    var ms = new System.IO.MemoryStream();
                                    fs.CopyTo(ms);
                                    ms.Position = 0;                               // <=== here
                                    if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
                                    pictureBox1.Image = Image.FromStream(ms);
                                }
                            }
                        }
                        else
                        {
                            string[] f = Directory.GetFiles(System.Environment.CurrentDirectory + "\\gifs");
                            int count = 1;
                            foreach (string fi in f)
                            {
                                if (fi.Contains(s + "-"))
                                {
                                    if (count == int.Parse(comboBox10.SelectedItem.ToString().Substring(6)))
                                    {
                                        pictureBox1.Visible = true;
                                        using (var fs = new System.IO.FileStream(fi, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                                        {
                                            var ms = new System.IO.MemoryStream();
                                            fs.CopyTo(ms);
                                            ms.Position = 0;                               // <=== here
                                            if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
                                            pictureBox1.Image = Image.FromStream(ms);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        count++;
                                    }
                                }
                            }
                        }
                        break;
                    case 1:                         //Gegner
                        if (comboBox3.SelectedItem == null)
                        {
                            comboBox3.SelectedItem = "Bulbasaur";
                        }
                        s = comboBox3.SelectedItem.ToString();
                        s = s.ToLower();
                        s = s.Replace("♀", "f");
                        s = s.Replace("♂", "m");
                        s = s.Replace("é", "e");
                        if (comboBox11.SelectedItem == null || comboBox11.SelectedItem.ToString().Equals("Normal"))
                        {
                            if (File.Exists(System.Environment.CurrentDirectory + "\\gifs\\" + s + ".gif"))
                            {
                                pictureBox2.Visible = true;
                                using (var fs = new System.IO.FileStream(System.Environment.CurrentDirectory + "\\gifs\\" + s + ".gif", System.IO.FileMode.Open, System.IO.FileAccess.Read))
                                {
                                    var ms = new System.IO.MemoryStream();
                                    fs.CopyTo(ms);
                                    ms.Position = 0;                               // <=== here
                                    if (pictureBox2.Image != null) pictureBox2.Image.Dispose();
                                    pictureBox2.Image = Image.FromStream(ms);
                                }
                            }
                        }
                        else
                        {
                            string[] f = Directory.GetFiles(System.Environment.CurrentDirectory + "\\gifs");
                            int count = 1;
                            foreach (string fi in f)
                            {
                                if (fi.Contains(s + "-"))
                                {
                                    if (count == int.Parse(comboBox11.SelectedItem.ToString().Substring(6)))
                                    {
                                        pictureBox2.Visible = true;
                                        using (var fs = new System.IO.FileStream(fi, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                                        {
                                            var ms = new System.IO.MemoryStream();
                                            fs.CopyTo(ms);
                                            ms.Position = 0;                               // <=== here
                                            if (pictureBox2.Image != null) pictureBox2.Image.Dispose();
                                            pictureBox2.Image = Image.FromStream(ms);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        count++;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        Thread t13;
        FolderBrowserDialog dlgFolderOpen;
        private void button16_Click(object sender, EventArgs e)
        {
            dlgFolderOpen = new FolderBrowserDialog();
            dlgFolderOpen.Description = "Open now the gifs Foler";
            dlgFolderOpen.ShowDialog();
            t13 = new Thread(gifchangetrue);
            t13.Start();
        }

        private void gifchangetrue()
        {
            try
            {
                pictureBox1.BeginInvoke((MethodInvoker)delegate
                {
                    pictureBox1.Image.Dispose();
                });
                pictureBox2.BeginInvoke((MethodInvoker)delegate
                {
                    pictureBox2.Image.Dispose();
                });
                DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "\\gifs");
                if (d.Exists)
                {
                    DirectoryInfo dold = new DirectoryInfo(System.Environment.CurrentDirectory + "\\gifs_old");
                    if (dold.Exists)
                    {
                        foreach (FileInfo f in dold.GetFiles())
                        {
                            f.Delete();
                        }
                    }
                    else
                    {
                        dold.Create();
                    }
                    foreach (FileInfo f in d.GetFiles())
                    {
                        f.CopyTo(dold.FullName + "\\" + f.Name);
                        f.Delete();
                    }

                    d = new DirectoryInfo(dlgFolderOpen.SelectedPath);
                    foreach (FileInfo ft in d.GetFiles())
                    {
                        ft.CopyTo(System.Environment.CurrentDirectory + "\\gifs\\" + ft.Name);
                    }
                }
                else
                {
                    d.Create();
                    d = new DirectoryInfo(dlgFolderOpen.SelectedPath);
                    foreach (FileInfo ft in d.GetFiles())
                    {
                        ft.CopyTo(System.Environment.CurrentDirectory + "\\gifs\\" + ft.Name);
                    }
                }
                MessageBox.Show("Finished! Press resh!");
            }
            catch (ArgumentException)
            {
            }
        }

        private void textBox27_TextChanged_1(object sender, EventArgs e)
        {
            if (itemCheck)
            {
                itemCheck = false;
                itemChoice();
                itemDefend();
                modCheck();
                if(stopE){  Wesen1(); Wesen2(); stats();  }
                itemCheck = true;
            }
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemCheck)
            {
                itemChoice();
                itemDefend();
                modCheck();
                stats();
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            close();
            clean();
            try
            {
                pictureBox1.Image.Dispose();
                pictureBox2.Image.Dispose();
            }
            catch (NullReferenceException)
            {
            }
            deletepokemonlog();
            MessageBox.Show("Bye! Now delete manually the Pokemon ToolBox.exe!");
            this.Close();
        }

        private void deletepokemonlog()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "");
            if (d.Exists)
            {
                FileInfo f1 = new FileInfo(System.Environment.CurrentDirectory + "\\Pokemon.data");
                FileInfo f2 = new FileInfo(System.Environment.CurrentDirectory + "\\attacks.data");
                FileInfo f3 = new FileInfo(System.Environment.CurrentDirectory + "\\Items.data");
                FileInfo f4 = new FileInfo(System.Environment.CurrentDirectory + "\\Natures.data");
                FileInfo f5 = new FileInfo(System.Environment.CurrentDirectory + "\\Abilities.data");
                FileInfo f6 = new FileInfo(System.Environment.CurrentDirectory + "\\gifs.zip");
                d = new DirectoryInfo(System.Environment.CurrentDirectory + "\\gifs\\");
                if (f1.Exists)
                {
                    f1.Delete();
                }
                if (f2.Exists)
                {
                    f2.Delete();
                }
                if (f3.Exists)
                {
                    f3.Delete();
                }
                if (f4.Exists)
                {
                    f4.Delete();
                }
                if (f5.Exists)
                {
                    f5.Delete();
                }
                if (f6.Exists)
                {
                    f6.Delete();
                }
                deletefolder(d);
            }
        }

        private void deletefolder(DirectoryInfo d)
        {
            foreach (DirectoryInfo d2 in d.GetDirectories())
            {
                deletefolder(d2);
            }
            foreach (FileInfo f in d.GetFiles())
            {
                f.Delete();
            }
            d.Delete();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                Pokemon p = matcherPokemon[comboBox1.SelectedItem.ToString()];
                String s = "";
                s = s + p.dexnbr + "~";
                s = s + p.height + "~";
                s = s + p.name + "~";
                s = s + String.Join(",", p.sonderheight) + "~";
                s = s + String.Join(",", String.Join(",", p.sonderstats)) + "~";
                s = s + String.Join(",", p.sondertype1) + "~";
                s = s + String.Join(",", p.sondertype2) + "~";
                s = s + String.Join(",", p.sonderweight) + "~";
                s = s + String.Join(",", p.stats) + "~";
                s = s + p.type1 + "~";
                s = s + p.type2 + "~";
                s = s + p.weight + "~";
                s = s + textBox26.Text + "~";
                s = s + textBox25.Text + "~";
                s = s + textBox24.Text + "~";
                s = s + textBox23.Text + "~";
                s = s + textBox22.Text + "~";
                s = s + textBox21.Text + "~";
                s = s + textBox1.Text + "~";
                s = s + textBox2.Text + "~";
                s = s + textBox3.Text + "~";
                s = s + textBox4.Text + "~";
                s = s + textBox5.Text + "~";
                s = s + textBox6.Text + "~";
                s = s + textBox34.Text + "~";
                s = s + textBox35.Text + "~";
                s = s + textBox36.Text + "~";
                s = s + textBox37.Text + "~";
                s = s + textBox38.Text + "~";
                if (comboBox8.SelectedItem == null)
                {
                    s = s + "Hardy" + "~";
                }
                else
                {
                    s = s + comboBox8.SelectedItem.ToString() + "~";
                }
                if (comboBox10.SelectedItem == null)
                {
                    s = s + "Normal" + "~";
                }
                else
                {
                    s = s + comboBox10.SelectedItem.ToString() + "~";
                }
                if (comboBox4.SelectedItem == null)
                {
                    s = s + "No Item" + "~";
                }
                else
                {
                    s = s + comboBox4.SelectedItem.ToString() + "~";
                }
                s = s + textBox13.Text + "~";
                if (comboBox6.SelectedItem == null)
                {
                    s = s + "Unaware" + "~";
                }
                else
                {
                    s = s + comboBox6.SelectedItem.ToString() + "~";
                }
                SaveFileDialog dlgFileOpen = new SaveFileDialog();               
                    dlgFileOpen.Title = "Where do you want to save your .pkm File";
                    dlgFileOpen.Filter = "Pokemon File|*.pkm";
                    try
                    {
                    dlgFileOpen.ShowDialog();
                    
                        FileInfo f = new FileInfo(dlgFileOpen.FileName);
                        if (f.Exists)
                        {
                            f.Delete();
                        }
                        StreamWriter sw = new StreamWriter(f.OpenWrite());
                        sw.WriteLine(s);
                        sw.Close();
                    }
                    catch (ArgumentException)
                    {
                    }
                
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgFileOpen = new OpenFileDialog();
            dlgFileOpen.Title = "Where is your .pkm File";
            dlgFileOpen.Filter = "Pokemon File|*.pkm";
            try
            {
                dlgFileOpen.ShowDialog();
                FileInfo f = new FileInfo(dlgFileOpen.FileName);
                StreamReader sr = new StreamReader(f.OpenRead());
                String s = sr.ReadLine();
                sr.Close();
                String[] t = s.Split('~');
                comboBox1.SelectedItem = t[2];
                textBox26.Text = t[12];
                textBox25.Text = t[13];
                textBox24.Text = t[14];
                textBox23.Text = t[15];
                textBox22.Text = t[16];
                textBox21.Text = t[17];
                textBox1.Text = t[18];
                textBox2.Text = t[19];
                textBox3.Text = t[20];
                textBox4.Text = t[21];
                textBox5.Text = t[22];
                textBox6.Text = t[23];
                textBox34.Text = t[24];
                textBox35.Text = t[25];
                textBox36.Text = t[26];
                textBox37.Text = t[27];
                textBox38.Text = t[28];
                comboBox8.SelectedItem = t[29];
                comboBox10.SelectedItem = t[30];
                comboBox4.SelectedItem = t[31];
                textBox13.Text = t[32];
                comboBox6.SelectedItem = t[33];
            }
            catch (Exception)
            {
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem != null)
            {
                Pokemon p = matcherPokemon[comboBox3.SelectedItem.ToString()];
                String s = "";
                s = s + p.dexnbr + "~";
                s = s + p.height + "~";
                s = s + p.name + "~";
                s = s + String.Join(",", p.sonderheight) + "~";
                s = s + String.Join(",", p.sonderstats) + "~";
                s = s + String.Join(",", p.sondertype1) + "~";
                s = s + String.Join(",", p.sondertype2) + "~";
                s = s + String.Join(",", p.sonderweight) + "~";
                s = s + String.Join(",", p.stats) + "~";
                s = s + p.type1 + "~";
                s = s + p.type2 + "~";
                s = s + p.weight + "~";
                
                s = s + textBox20.Text + "~";
                s = s + textBox19.Text + "~";
                s = s + textBox18.Text + "~";
                s = s + textBox17.Text + "~";
                s = s + textBox16.Text + "~";
                s = s + textBox15.Text + "~";
                s = s + textBox12.Text + "~";
                s = s + textBox11.Text + "~";
                s = s + textBox10.Text + "~";
                s = s + textBox9.Text + "~";
                s = s + textBox8.Text + "~";
                s = s + textBox7.Text + "~";
                s = s + textBox28.Text + "~";
                s = s + textBox29.Text + "~";
                s = s + textBox30.Text + "~";
                s = s + textBox31.Text + "~";
                s = s + textBox32.Text + "~";
                if (comboBox9.SelectedItem == null)
                {
                    s = s + "Hardy" + "~";
                }
                else
                {
                    s = s + comboBox9.SelectedItem.ToString() + "~";
                }
                if (comboBox11.SelectedItem == null)
                {
                    s = s + "Normal" + "~";
                }
                else
                {
                    s = s + comboBox11.SelectedItem.ToString() + "~";
                }
                if (comboBox5.SelectedItem == null)
                {
                    s = s + "No Item" + "~";
                }
                else
                {
                    s = s + comboBox5.SelectedItem.ToString() + "~";
                }
                s = s + textBox14.Text + "~";
                if (comboBox7.SelectedItem == null)
                {
                    s = s + "Unaware" + "~";
                }
                else
                {
                    s = s + comboBox7.SelectedItem.ToString() + "~";
                }
                SaveFileDialog dlgFileOpen = new SaveFileDialog();
                dlgFileOpen.Title = "Where do you want to save your .pkm File";
                dlgFileOpen.Filter = "Pokemon File|*.pkm";
                try
                {
                dlgFileOpen.ShowDialog();
                
                    FileInfo f = new FileInfo(dlgFileOpen.FileName);
                    if (f.Exists)
                    {
                        f.Delete();
                    }
                    StreamWriter sw = new StreamWriter(f.OpenWrite());
                    sw.WriteLine(s);
                    sw.Close();
                }
                catch (ArgumentException)
                {
                }

            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgFileOpen = new OpenFileDialog();
            dlgFileOpen.Title = "Where is your .pkm File";
            dlgFileOpen.Filter = "Pokemon File|*.pkm";
            try
            {
                dlgFileOpen.ShowDialog();
                FileInfo f = new FileInfo(dlgFileOpen.FileName);
                StreamReader sr = new StreamReader(f.OpenRead());
                String s = sr.ReadLine();
                sr.Close();
                String[] t = s.Split('~');
                comboBox3.SelectedItem = t[2];
                textBox12.Text = t[18];
                textBox11.Text = t[19];
                textBox10.Text = t[20];
                textBox9.Text = t[21];
                textBox8.Text = t[22];
                textBox7.Text = t[23];
                textBox20.Text = t[12];
                textBox19.Text = t[13];
                textBox18.Text = t[14];
                textBox17.Text = t[15];
                textBox16.Text = t[16];
                textBox15.Text = t[17];
                textBox28.Text = t[24];
                textBox29.Text = t[25];
                textBox30.Text = t[26];
                textBox31.Text = t[27];
                textBox32.Text = t[28];
                comboBox9.SelectedItem = t[29];
                comboBox11.SelectedItem = t[30];
                comboBox5.SelectedItem = t[31];
                textBox14.Text = t[32];
                comboBox7.SelectedItem = t[33];
            }
            catch (Exception)
            {
            }
        }

        bool stopE = true;

        private void button22_Click(object sender, EventArgs e)
        {
            itemCheck = false;
            itemtemp = false;
            itemtempa = false;
            stopE = false;
            String temp1 = null;
            if (comboBox10.SelectedItem != null)
            {
                temp1 = comboBox10.SelectedItem.ToString();
            }
            String temp2 = null;
            if (comboBox11.SelectedItem != null)
            {
                temp2 = comboBox11.SelectedItem.ToString();
            }
            SwapC( comboBox1,   comboBox3);
            SwapC( comboBox9,   comboBox8);
            SwapC( comboBox10,   comboBox11);
            SwapC( comboBox4,   comboBox5);
            SwapC( comboBox6,   comboBox7);
            SwapT( textBox13,   textBox14);
            SwapT( textBox26,   textBox20);
            SwapT( textBox25,   textBox19);
            SwapT( textBox24,   textBox18);
            SwapT( textBox23,   textBox17);
            SwapT( textBox22,   textBox16);
            SwapT( textBox21,   textBox15);
            SwapT( textBox1,   textBox12);
            SwapT( textBox2,   textBox11);
            SwapT( textBox3,   textBox10);
            SwapT( textBox4,   textBox9);
            SwapT( textBox5,   textBox8);
            SwapT( textBox6,   textBox7);
            SwapT( textBox34,   textBox28);
            SwapT( textBox35,   textBox29);
            SwapT( textBox36,   textBox30);
            SwapT( textBox37,   textBox31);
            SwapT( textBox38,   textBox32);
            extraformset(0);
            extraformset(1);
            comboBox10.SelectedItem = temp2;
            comboBox11.SelectedItem = temp1;
            itemChoice();
            itemDefend();
            modCheck();
            
            Wesen1();
            Wesen2();
            stats();
            
            gifchange(0);
            gifchange(1);
            itemCheck = true;
            itemtemp = true;
            itemtempa = true;
            stopE = true;
        }


        static void SwapT(TextBox lhs, TextBox rhs)
        {
            String temp;
            temp = lhs.Text+"";
            lhs.Text = rhs.Text+"";
            rhs.Text = temp;
        }

        static void SwapC(ComboBox lhs, ComboBox rhs)
        {
            String temp;
            if (lhs.SelectedItem == null)
            {
                temp = null;
            }
            else
            {
                temp = lhs.SelectedItem.ToString();
            }
            if (rhs.SelectedItem == null)
            {
                lhs.SelectedItem = null;
            }
            else
            {
                lhs.SelectedItem = rhs.SelectedItem.ToString();
            }
            rhs.SelectedItem = temp;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            exportCalc();            
        }

        private void exportCalc()
        {
            try
            {
                if (comboBox1.SelectedItem != null && comboBox3.SelectedItem != null && comboBox2.SelectedItem != null)
                {
                    Pokemon p = matcherPokemon[comboBox1.SelectedItem.ToString()];
                    String s = "";
                    s = s + p.dexnbr + "~";
                    s = s + p.height + "~";
                    s = s + p.name + "~";
                    s = s + String.Join(",", p.sonderheight) + "~";
                    s = s + String.Join(",", String.Join(",", p.sonderstats)) + "~";
                    s = s + String.Join(",", p.sondertype1) + "~";
                    s = s + String.Join(",", p.sondertype2) + "~";
                    s = s + String.Join(",", p.sonderweight) + "~";
                    s = s + String.Join(",", p.stats) + "~";
                    s = s + p.type1 + "~";
                    s = s + p.type2 + "~";
                    s = s + p.weight + "~";
                    s = s + textBox26.Text + "~";
                    s = s + textBox25.Text + "~";
                    s = s + textBox24.Text + "~";
                    s = s + textBox23.Text + "~";
                    s = s + textBox22.Text + "~";
                    s = s + textBox21.Text + "~";
                    s = s + textBox1.Text + "~";
                    s = s + textBox2.Text + "~";
                    s = s + textBox3.Text + "~";
                    s = s + textBox4.Text + "~";
                    s = s + textBox5.Text + "~";
                    s = s + textBox6.Text + "~";
                    s = s + textBox34.Text + "~";
                    s = s + textBox35.Text + "~";
                    s = s + textBox36.Text + "~";
                    s = s + textBox37.Text + "~";
                    s = s + textBox38.Text + "~";
                    if (comboBox8.SelectedItem == null)
                    {
                        s = s + "Hardy" + "~";
                    }
                    else
                    {
                        s = s + comboBox8.SelectedItem.ToString() + "~";
                    }
                    if (comboBox10.SelectedItem == null)
                    {
                        s = s + "Normal" + "~";
                    }
                    else
                    {
                        s = s + comboBox10.SelectedItem.ToString() + "~";
                    }
                    if (comboBox4.SelectedItem == null)
                    {
                        s = s + "No Item" + "~";
                    }
                    else
                    {
                        s = s + comboBox4.SelectedItem.ToString() + "~";
                    }
                    s = s + textBox13.Text + "~";
                    if (comboBox6.SelectedItem == null)
                    {
                        s = s + "Unaware" + "~";
                    }
                    else
                    {
                        s = s + comboBox6.SelectedItem.ToString() + "~";
                    }
                    SaveFileDialog dlgFileOpen = new SaveFileDialog();
                    dlgFileOpen.Title = "Where do you want to save your .calc File";
                    dlgFileOpen.Filter = "Calculation File|*.calc";
                    StreamWriter sw;

                    dlgFileOpen.ShowDialog();

                    FileInfo f = new FileInfo(dlgFileOpen.FileName);
                    if (f.Exists)
                    {
                        f.Delete();
                    }
                    sw = new StreamWriter(f.OpenWrite());
                    sw.WriteLine(s);

                    s = "";
                    Attacks a = matcherAttacks[comboBox2.SelectedItem.ToString()];
                    s = s + a.name + "~";
                    try
                    {
                        s = s + int.Parse(textBox27.Text) + "~";
                    }
                    catch (FormatException)
                    {
                        s = s + a.strength + "~";
                    }
                    if (comboBox13.SelectedItem != null)
                    {
                        s = s + comboBox13.SelectedItem.ToString() + "~";
                    }
                    else
                    {
                        s = s + a.type + "~";
                    }
                    if (comboBox12.SelectedItem != null)
                    {
                        s = s + comboBox12.SelectedItem.ToString() + "~";
                    }
                    else
                    {
                        s = s + "No Weather" + "~";
                    }
                    s = s + checkBox1.Checked + "~";
                    s = s + checkBox3.Checked + "~";
                    s = s + checkBox4.Checked + "~";
                    s = s + checkBox5.Checked + "~";
                    sw.WriteLine(s);

                    p = matcherPokemon[comboBox3.SelectedItem.ToString()];
                    s = "";
                    s = s + p.dexnbr + "~";
                    s = s + p.height + "~";
                    s = s + p.name + "~";
                    s = s + String.Join(",", p.sonderheight) + "~";
                    s = s + String.Join(",", p.sonderstats) + "~";
                    s = s + String.Join(",", p.sondertype1) + "~";
                    s = s + String.Join(",", p.sondertype2) + "~";
                    s = s + String.Join(",", p.sonderweight) + "~";
                    s = s + String.Join(",", p.stats) + "~";
                    s = s + p.type1 + "~";
                    s = s + p.type2 + "~";
                    s = s + p.weight + "~";

                    s = s + textBox20.Text + "~";
                    s = s + textBox19.Text + "~";
                    s = s + textBox18.Text + "~";
                    s = s + textBox17.Text + "~";
                    s = s + textBox16.Text + "~";
                    s = s + textBox15.Text + "~";
                    s = s + textBox12.Text + "~";
                    s = s + textBox11.Text + "~";
                    s = s + textBox10.Text + "~";
                    s = s + textBox9.Text + "~";
                    s = s + textBox8.Text + "~";
                    s = s + textBox7.Text + "~";
                    s = s + textBox28.Text + "~";
                    s = s + textBox29.Text + "~";
                    s = s + textBox30.Text + "~";
                    s = s + textBox31.Text + "~";
                    s = s + textBox32.Text + "~";
                    if (comboBox9.SelectedItem == null)
                    {
                        s = s + "Hardy" + "~";
                    }
                    else
                    {
                        s = s + comboBox9.SelectedItem.ToString() + "~";
                    }
                    if (comboBox11.SelectedItem == null)
                    {
                        s = s + "Normal" + "~";
                    }
                    else
                    {
                        s = s + comboBox11.SelectedItem.ToString() + "~";
                    }
                    if (comboBox5.SelectedItem == null)
                    {
                        s = s + "No Item" + "~";
                    }
                    else
                    {
                        s = s + comboBox5.SelectedItem.ToString() + "~";
                    }
                    s = s + textBox14.Text + "~";
                    if (comboBox7.SelectedItem == null)
                    {
                        s = s + "Unaware" + "~";
                    }
                    else
                    {
                        s = s + comboBox7.SelectedItem.ToString() + "~";
                    }

                    sw.WriteLine(s);
                    sw.Close();
                }

            }
            catch (ArgumentException)
            {
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            importcalc();
        }

        private void importcalc()
        {
            OpenFileDialog dlgFileOpen = new OpenFileDialog();
            dlgFileOpen.Title = "Where is your .calc File";
            dlgFileOpen.Filter = "Calculation File|*.calc";
            try
            {
                dlgFileOpen.ShowDialog();
                FileInfo f = new FileInfo(dlgFileOpen.FileName);
                StreamReader sr = new StreamReader(f.OpenRead());
                String s = sr.ReadLine();
                String[] t = s.Split('~');
                comboBox1.SelectedItem = t[2];
                textBox26.Text = t[12];
                textBox25.Text = t[13];
                textBox24.Text = t[14];
                textBox23.Text = t[15];
                textBox22.Text = t[16];
                textBox21.Text = t[17];
                textBox1.Text = t[18];
                textBox2.Text = t[19];
                textBox3.Text = t[20];
                textBox4.Text = t[21];
                textBox5.Text = t[22];
                textBox6.Text = t[23];
                textBox34.Text = t[24];
                textBox35.Text = t[25];
                textBox36.Text = t[26];
                textBox37.Text = t[27];
                textBox38.Text = t[28];
                comboBox8.SelectedItem = t[29];
                comboBox10.SelectedItem = t[30];
                comboBox4.SelectedItem = t[31];
                textBox13.Text = t[32];
                comboBox6.SelectedItem = t[33];

                s = sr.ReadLine();
                t = s.Split('~');
                comboBox2.SelectedItem = t[0];
                textBox27.Text = t[1];
                comboBox13.SelectedItem = t[2];
                comboBox12.SelectedItem = t[3];
                checkBox1.Checked = bool.Parse(t[4]);
                checkBox3.Checked = bool.Parse(t[5]);
                checkBox4.Checked = bool.Parse(t[6]);
                checkBox5.Checked = bool.Parse(t[7]);

                s = sr.ReadLine();
                t = s.Split('~');
                comboBox3.SelectedItem = t[2];
                textBox12.Text = t[18];
                textBox11.Text = t[19];
                textBox10.Text = t[20];
                textBox9.Text = t[21];
                textBox8.Text = t[22];
                textBox7.Text = t[23];
                textBox20.Text = t[12];
                textBox19.Text = t[13];
                textBox18.Text = t[14];
                textBox17.Text = t[15];
                textBox16.Text = t[16];
                textBox15.Text = t[17];
                textBox28.Text = t[24];
                textBox29.Text = t[25];
                textBox30.Text = t[26];
                textBox31.Text = t[27];
                textBox32.Text = t[28];
                comboBox9.SelectedItem = t[29];
                comboBox11.SelectedItem = t[30];
                comboBox5.SelectedItem = t[31];
                textBox14.Text = t[32];
                comboBox7.SelectedItem = t[33];
                sr.Close();
            }
            catch (Exception)
            {
            }
        }

        private void importcalc(String st)
        {
            
            try
            {                
                FileInfo f = new FileInfo(st);
                if (f.Exists)
                {
                    StreamReader sr = new StreamReader(f.OpenRead());
                    String s = sr.ReadLine();
                    String[] t = s.Split('~');
                    comboBox1.SelectedItem = t[2];
                    textBox26.Text = t[12];
                    textBox25.Text = t[13];
                    textBox24.Text = t[14];
                    textBox23.Text = t[15];
                    textBox22.Text = t[16];
                    textBox21.Text = t[17];
                    textBox1.Text = t[18];
                    textBox2.Text = t[19];
                    textBox3.Text = t[20];
                    textBox4.Text = t[21];
                    textBox5.Text = t[22];
                    textBox6.Text = t[23];
                    textBox34.Text = t[24];
                    textBox35.Text = t[25];
                    textBox36.Text = t[26];
                    textBox37.Text = t[27];
                    textBox38.Text = t[28];
                    comboBox8.SelectedItem = t[29];
                    comboBox10.SelectedItem = t[30];
                    comboBox4.SelectedItem = t[31];
                    textBox13.Text = t[32];
                    comboBox6.SelectedItem = t[33];

                    s = sr.ReadLine();
                    t = s.Split('~');
                    comboBox2.SelectedItem = t[0];
                    textBox27.Text = t[1];
                    comboBox13.SelectedItem = t[2];
                    comboBox12.SelectedItem = t[3];
                    checkBox1.Checked = bool.Parse(t[4]);
                    checkBox3.Checked = bool.Parse(t[5]);
                    checkBox4.Checked = bool.Parse(t[6]);
                    checkBox5.Checked = bool.Parse(t[7]);

                    s = sr.ReadLine();
                    t = s.Split('~');
                    comboBox3.SelectedItem = t[2];
                    textBox12.Text = t[18];
                    textBox11.Text = t[19];
                    textBox10.Text = t[20];
                    textBox9.Text = t[21];
                    textBox8.Text = t[22];
                    textBox7.Text = t[23];
                    textBox20.Text = t[12];
                    textBox19.Text = t[13];
                    textBox18.Text = t[14];
                    textBox17.Text = t[15];
                    textBox16.Text = t[16];
                    textBox15.Text = t[17];
                    textBox28.Text = t[24];
                    textBox29.Text = t[25];
                    textBox30.Text = t[26];
                    textBox31.Text = t[27];
                    textBox32.Text = t[28];
                    comboBox9.SelectedItem = t[29];
                    comboBox11.SelectedItem = t[30];
                    comboBox5.SelectedItem = t[31];
                    textBox14.Text = t[32];
                    comboBox7.SelectedItem = t[33];
                    sr.Close();
                } else {
                    MessageBox.Show(f.ToString()+" doesn't exist!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An Error occured while reading the file. Please make sure the file is not corrupted.");
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            textBox26.Text = "31";
            textBox25.Text = "31";
            textBox24.Text = "31";
            textBox23.Text = "31";
            textBox22.Text = "31";
            textBox21.Text = "31";
            textBox1.Text = "0";
            textBox2.Text = "0";
            textBox3.Text = "0";
            textBox4.Text = "0";
            textBox5.Text = "0";
            textBox6.Text = "0";
            textBox34.Text = "0";
            textBox35.Text = "0";
            textBox36.Text = "0";
            textBox37.Text = "0";
            textBox38.Text = "0";
        }

        private void button26_Click(object sender, EventArgs e)
        {
            textBox20.Text = "31";
            textBox19.Text = "31";
            textBox18.Text = "31";
            textBox17.Text = "31";
            textBox16.Text = "31";
            textBox15.Text = "31";
            textBox12.Text = "0";
            textBox11.Text = "0";
            textBox10.Text = "0";
            textBox9.Text = "0";
            textBox8.Text = "0";
            textBox7.Text = "0";
            textBox28.Text = "0";
            textBox29.Text = "0";
            textBox30.Text = "0";
            textBox31.Text = "0";
            textBox32.Text = "0";
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.S)
                {
                    e.SuppressKeyPress = true;
                    exportCalc();
                }
                else if (e.KeyCode == Keys.I)
                {
                    e.SuppressKeyPress = true;
                    importcalc();
                }
            }        
        }

       

        private void getDatafromShowdown()
        {
            DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "/logs");
            if (!d.Exists)
            {
                d.Create();
            }
            FileInfo f;
            StreamWriter sw;
            WebClient wClient = new WebClient();
            String s="";
          
            foreach (String p in matcherPokemon.Keys)
            {
                bool b = true;
                try{  
                   s = wClient.DownloadString("http://pokemonshowdown.com/dex/pokemon/"+p);                 
                }
                catch (WebException)
                {
                    b = false;
                }
                if (b)
                {
                    f = new FileInfo(System.Environment.CurrentDirectory + "/logs/" + p + ".log");
                    if (!f.Exists)
                    {
                        FileStream fs = f.Create();
                        fs.Close();
                    }
                    else
                    {
                        f.Delete();
                        FileStream fs = f.Create();
                        fs.Close();
                    }

                    sw = f.AppendText();
                    sw.WriteLine(s);
                    sw.Close();
                }
            }
            MessageBox.Show("Finished!");
        }

        private void button29_Click(object sender, EventArgs e)
        {

            foreach (String s in matcherPokemon.Keys)
            {
                foreach (String s2 in matcherPokemon.Keys)
                {
                    DirectoryInfo d = new DirectoryInfo(System.Environment.CurrentDirectory + "\\calcs\\"+s);
                    if (!d.Exists)
                    {
                        d.Create();
                    }
                    d = new DirectoryInfo(System.Environment.CurrentDirectory + "\\calcs\\" + s +"\\"+ s2);
                    if (!d.Exists)
                    {
                        d.Create();
                    }
                }
            }

            foreach (String s in matcherPokemon.Keys)
            {
                comboBox1.SelectedItem = s;
                if (comboBox10.Items.Count > 1)
                {
                    int c = 0;
                    foreach (object Item in comboBox10.Items)
                    {
                        String s4 = (String)Item;
                        comboBox10.SelectedItem = s4;
                        int count = 0;
                        foreach (String s2 in matcherPokemon.Keys)
                        {
                            comboBox3.SelectedItem = s2;
                            if (comboBox11.Items.Count > 1)
                            {
                                foreach (object Item2 in comboBox11.Items)
                                {
                                    String s5 = (String)Item2;
                                    comboBox10.SelectedItem = s5;
                                    
                                    foreach (String s3 in matcherAttacks.Keys)
                                    {
                                        comboBox2.SelectedItem = s3;
                                        Attacks a = matcherAttacks[s3];
                                        textBox2.Text = "0";
                                        textBox4.Text = "0";
                                        textBox10.Text = "0";
                                        textBox12.Text = "0";
                                        textBox8.Text = "0";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 0 " + s3 + " 0_0 " + s2 + count + ".calc");
                                        if (a.split == 0)
                                        {
                                            textBox2.Text = "252";
                                            exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 252 " + s3 + " 0_0 " + s2 + count + ".calc");
                                            textBox10.Text = "252";
                                            textBox12.Text = "252";
                                            exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 252 " + s3 + " 252_252 " + s2 + count + ".calc");
                                            textBox2.Text = "0";
                                            exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 0 " + s3 + " 252_252 " + s2 + count + ".calc");
                                        }
                                        else
                                        {
                                            textBox4.Text = "252";
                                            exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 252 " + s3 + " 0_0 " + s2 + count + ".calc");
                                            textBox8.Text = "252";
                                            textBox12.Text = "252";
                                            exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 252 " + s3 + " 252_252 " + s2 + count + ".calc");
                                            textBox4.Text = "0";
                                            exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 0 " + s3 + " 252_252 " + s2 + count + ".calc");
                                        }
                                    }
                                    count++;
                                }
                            }
                            else
                            {
                                foreach (String s3 in matcherAttacks.Keys)
                                {
                                    comboBox2.SelectedItem = s3;
                                    Attacks a = matcherAttacks[s3];
                                    textBox2.Text = "0";
                                    textBox4.Text = "0";
                                    textBox10.Text = "0";
                                    textBox12.Text = "0";
                                    textBox8.Text = "0";
                                    exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 0 " + s3 + " 0_0 " + s2 + ".calc");
                                    if (a.split == 0)
                                    {
                                        textBox2.Text = "252";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 252 " + s3 + " 0_0 " + s2 +  ".calc");
                                        textBox10.Text = "252";
                                        textBox12.Text = "252";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 252 " + s3 + " 252_252 " + s2 +  ".calc");
                                        textBox2.Text = "0";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 0 " + s3 + " 252_252 " + s2 +  ".calc");
                                    }
                                    else
                                    {
                                        textBox4.Text = "252";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 252 " + s3 + " 0_0 " + s2 +  ".calc");
                                        textBox8.Text = "252";
                                        textBox12.Text = "252";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + c + " 252 " + s3 + " 252_252 " + s2 + ".calc");
                                        textBox4.Text = "0";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s +c+ " 0 " + s3 + " 252_252 " + s2 +  ".calc");
                                    }
                                }
                            }
                        }
                        c++;
                    }

                }
                else
                {
                    foreach (String s2 in matcherPokemon.Keys)
                    {
                        int count = 0;
                        comboBox3.SelectedItem = s2;
                        if (comboBox11.Items.Count > 1)
                        {
                            foreach (object Item2 in comboBox11.Items)
                            {
                               
                                String s5 = (String)Item2;
                                comboBox10.SelectedItem = s5;

                                foreach (String s3 in matcherAttacks.Keys)
                                {
                                    comboBox2.SelectedItem = s3;
                                    Attacks a = matcherAttacks[s3];
                                    textBox2.Text = "0";
                                    textBox4.Text = "0";
                                    textBox10.Text = "0";
                                    textBox12.Text = "0";
                                    textBox8.Text = "0";
                                    exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 0 " + s3 + " 0_0 " + s2 + count + ".calc");
                                    if (a.split == 0)
                                    {
                                        textBox2.Text = "252";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 252 " + s3 + " 0_0 " + s2 + count + ".calc");
                                        textBox10.Text = "252";
                                        textBox12.Text = "252";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 252 " + s3 + " 252_252 " + s2 + count + ".calc");
                                        textBox2.Text = "0";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 0 " + s3 + " 252_252 " + s2 + count + ".calc");
                                    }
                                    else
                                    {
                                        textBox4.Text = "252";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 252 " + s3 + " 0_0 " + s2 + count + ".calc");
                                        textBox8.Text = "252";
                                        textBox12.Text = "252";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 252 " + s3 + " 252_252 " + s2 + count + ".calc");
                                        textBox4.Text = "0";
                                        exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 0 " + s3 + " 252_252 " + s2 + count + ".calc");
                                    }
                                }
                                count++;
                            }
                        }
                        else
                        {
                            foreach (String s3 in matcherAttacks.Keys)
                            {
                                comboBox2.SelectedItem = s3;
                                Attacks a = matcherAttacks[s3];
                                textBox2.Text = "0";
                                textBox4.Text = "0";
                                textBox10.Text = "0";
                                textBox12.Text = "0";
                                textBox8.Text = "0";
                                exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 0 " + s3 + " 0_0 " + s2 + ".calc");
                                if (a.split == 0)
                                {
                                    textBox2.Text = "252";
                                    exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 252 " + s3 + " 0_0 " + s2 + ".calc");
                                    textBox10.Text = "252";
                                    textBox12.Text = "252";
                                    exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 252 " + s3 + " 252_252 " + s2 + ".calc");
                                    textBox2.Text = "0";
                                    exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 0 " + s3 + " 252_252 " + s2 + ".calc");
                                }
                                else
                                {
                                    textBox4.Text = "252";
                                    exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 252 " + s3 + " 0_0 " + s2 + ".calc");
                                    textBox8.Text = "252";
                                    textBox12.Text = "252";
                                    exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 252 " + s3 + " 252_252 " + s2 + ".calc");
                                    textBox4.Text = "0";
                                    exportCalc(System.Environment.CurrentDirectory + "\\calcs\\" + s + "\\" + s2 + "\\" + s + " 0 " + s3 + " 252_252 " + s2 + ".calc");
                                }
                            }
                        }
                    }
                }
            }
        }
        private void exportCalc(String fname)
        {
            try
            {
                if (comboBox1.SelectedItem != null && comboBox3.SelectedItem != null && comboBox2.SelectedItem != null)
                {
                    Pokemon p = matcherPokemon[comboBox1.SelectedItem.ToString()];
                    String s = "";
                    s = s + p.dexnbr + "~";
                    s = s + p.height + "~";
                    s = s + p.name + "~";
                    s = s + String.Join(",", p.sonderheight) + "~";
                    s = s + String.Join(",", String.Join(",", p.sonderstats)) + "~";
                    s = s + String.Join(",", p.sondertype1) + "~";
                    s = s + String.Join(",", p.sondertype2) + "~";
                    s = s + String.Join(",", p.sonderweight) + "~";
                    s = s + String.Join(",", p.stats) + "~";
                    s = s + p.type1 + "~";
                    s = s + p.type2 + "~";
                    s = s + p.weight + "~";
                    s = s + textBox26.Text + "~";
                    s = s + textBox25.Text + "~";
                    s = s + textBox24.Text + "~";
                    s = s + textBox23.Text + "~";
                    s = s + textBox22.Text + "~";
                    s = s + textBox21.Text + "~";
                    s = s + textBox1.Text + "~";
                    s = s + textBox2.Text + "~";
                    s = s + textBox3.Text + "~";
                    s = s + textBox4.Text + "~";
                    s = s + textBox5.Text + "~";
                    s = s + textBox6.Text + "~";
                    s = s + textBox34.Text + "~";
                    s = s + textBox35.Text + "~";
                    s = s + textBox36.Text + "~";
                    s = s + textBox37.Text + "~";
                    s = s + textBox38.Text + "~";
                    if (comboBox8.SelectedItem == null)
                    {
                        s = s + "Hardy" + "~";
                    }
                    else
                    {
                        s = s + comboBox8.SelectedItem.ToString() + "~";
                    }
                    if (comboBox10.SelectedItem == null)
                    {
                        s = s + "Normal" + "~";
                    }
                    else
                    {
                        s = s + comboBox10.SelectedItem.ToString() + "~";
                    }
                    if (comboBox4.SelectedItem == null)
                    {
                        s = s + "No Item" + "~";
                    }
                    else
                    {
                        s = s + comboBox4.SelectedItem.ToString() + "~";
                    }
                    s = s + textBox13.Text + "~";
                    if (comboBox6.SelectedItem == null)
                    {
                        s = s + "Unaware" + "~";
                    }
                    else
                    {
                        s = s + comboBox6.SelectedItem.ToString() + "~";
                    }
                    
                    StreamWriter sw;

                    FileInfo f = new FileInfo(fname);
                    if (f.Exists)
                    {
                        f.Delete();
                    }
                    sw = new StreamWriter(f.OpenWrite());
                    sw.WriteLine(s);

                    s = "";
                    Attacks a = matcherAttacks[comboBox2.SelectedItem.ToString()];
                    s = s + a.name + "~";
                    try
                    {
                        s = s + int.Parse(textBox27.Text) + "~";
                    }
                    catch (FormatException)
                    {
                        s = s + a.strength + "~";
                    }
                    if (comboBox13.SelectedItem != null)
                    {
                        s = s + comboBox13.SelectedItem.ToString() + "~";
                    }
                    else
                    {
                        s = s + a.type + "~";
                    }
                    if (comboBox12.SelectedItem != null)
                    {
                        s = s + comboBox12.SelectedItem.ToString() + "~";
                    }
                    else
                    {
                        s = s + "No Weather" + "~";
                    }
                    s = s + checkBox1.Checked + "~";
                    s = s + checkBox3.Checked + "~";
                    s = s + checkBox4.Checked + "~";
                    s = s + checkBox5.Checked + "~";
                    sw.WriteLine(s);

                    p = matcherPokemon[comboBox3.SelectedItem.ToString()];
                    s = "";
                    s = s + p.dexnbr + "~";
                    s = s + p.height + "~";
                    s = s + p.name + "~";
                    s = s + String.Join(",", p.sonderheight) + "~";
                    s = s + String.Join(",", p.sonderstats) + "~";
                    s = s + String.Join(",", p.sondertype1) + "~";
                    s = s + String.Join(",", p.sondertype2) + "~";
                    s = s + String.Join(",", p.sonderweight) + "~";
                    s = s + String.Join(",", p.stats) + "~";
                    s = s + p.type1 + "~";
                    s = s + p.type2 + "~";
                    s = s + p.weight + "~";

                    s = s + textBox20.Text + "~";
                    s = s + textBox19.Text + "~";
                    s = s + textBox18.Text + "~";
                    s = s + textBox17.Text + "~";
                    s = s + textBox16.Text + "~";
                    s = s + textBox15.Text + "~";
                    s = s + textBox12.Text + "~";
                    s = s + textBox11.Text + "~";
                    s = s + textBox10.Text + "~";
                    s = s + textBox9.Text + "~";
                    s = s + textBox8.Text + "~";
                    s = s + textBox7.Text + "~";
                    s = s + textBox28.Text + "~";
                    s = s + textBox29.Text + "~";
                    s = s + textBox30.Text + "~";
                    s = s + textBox31.Text + "~";
                    s = s + textBox32.Text + "~";
                    if (comboBox9.SelectedItem == null)
                    {
                        s = s + "Hardy" + "~";
                    }
                    else
                    {
                        s = s + comboBox9.SelectedItem.ToString() + "~";
                    }
                    if (comboBox11.SelectedItem == null)
                    {
                        s = s + "Normal" + "~";
                    }
                    else
                    {
                        s = s + comboBox11.SelectedItem.ToString() + "~";
                    }
                    if (comboBox5.SelectedItem == null)
                    {
                        s = s + "No Item" + "~";
                    }
                    else
                    {
                        s = s + comboBox5.SelectedItem.ToString() + "~";
                    }
                    s = s + textBox14.Text + "~";
                    if (comboBox7.SelectedItem == null)
                    {
                        s = s + "Unaware" + "~";
                    }
                    else
                    {
                        s = s + comboBox7.SelectedItem.ToString() + "~";
                    }

                    sw.WriteLine(s);
                    sw.Close();
                }

            }
            catch (ArgumentException)
            {
            }
        }
    }



}