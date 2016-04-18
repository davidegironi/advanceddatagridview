using System;
using System.Configuration;
using System.Windows.Forms;

namespace AdvancedDataGridViewSample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            if(!String.IsNullOrEmpty(ConfigurationManager.AppSettings["culture"]))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["culture"]);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["culture"]);
            }

            Application.Run(new FormMain());
        }
    }
}
