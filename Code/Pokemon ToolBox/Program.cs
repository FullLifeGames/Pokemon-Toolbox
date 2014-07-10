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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
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
