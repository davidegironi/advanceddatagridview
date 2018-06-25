using System;
using System.Configuration;
using System.Windows.Forms;

namespace AdvancedDataGridViewSample
{
    static class Program
    {
        /// <summary>
        /// Enable high DPI
        /// </summary>
        static readonly bool HighDPIEnabled = false;

        /// <summary>
        /// Load for high DPI
        /// </summary>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (HighDPIEnabled && Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["culture"]))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["culture"]);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["culture"]);
            }

            FormMain formMain = new FormMain();
            if (HighDPIEnabled)
                formMain.AutoScaleMode = AutoScaleMode.Dpi;
            Application.Run(formMain);
        }
    }
}
