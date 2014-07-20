using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pokemon_ToolBox
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                if (args[0].Equals("Damagecalc"))
                {
                    Application.Run(new Form1());
                }
                else if (args[0].Equals("Quiz"))
                {
                    string[] temp = new string[2];
                    Application.Run(new FormQuizControl(temp));
                    Application.Run(new Form2(temp));
                }
                else if (args[0].Contains(".calc"))
                {
                    Application.Run(new Form1(args[0]));
                }
                else
                {
                    string[] s = new string[1];
                    Application.Run(new FormControl(s));
                    try
                    {
                        switch (s[0].ToLower())
                        {
                            case ("quiz"):
                                string[] temp = new string[2];
                                Application.Run(new FormQuizControl(temp));
                                Application.Run(new Form2(temp));
                                break;
                            default:
                                Application.Run(new Form1());
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {                
                string[] s = new string[1];
                Application.Run(new FormControl(s));
                try
                {
                    switch (s[0].ToLower())
                    {
                        case ("quiz"):
                            string[] temp = new string[2];
                            Application.Run(new FormQuizControl(temp));
                            Application.Run(new Form2(temp));
                            break;
                        default:
                            Application.Run(new Form1());
                            break;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }   
}
