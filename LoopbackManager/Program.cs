using System;
using System.Windows.Forms;
using LoopbackManagerModern;

namespace LoopbackManager
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            App app = new ();

            Application.Run(new MainWindow());

            app.Close();
        }
    }
}
