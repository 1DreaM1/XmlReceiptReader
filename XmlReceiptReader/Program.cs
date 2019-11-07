using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XmlReceiptReader
{
    static class Program
    {
        public static string DB_PATH = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\database.db";
        public static bool LOAD_DATA = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
