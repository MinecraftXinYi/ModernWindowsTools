using Mile.Xaml;
using LoopbackManagerModern;
using System;
using System.Windows.Forms;

namespace LoopbackManager
{
    public partial class MainWindow : Form
    {
        public static MainWindow Current { get; private set; }

        private readonly WindowsXamlHost xamlHost;

        public MainWindow()
        {
            InitializeComponent();

            xamlHost = new ();
            Current = this;

            this.Controls.Add(xamlHost);
            xamlHost.AutoSize = true;
            xamlHost.Dock = DockStyle.Fill;
            xamlHost.Child = new MainPage();

            this.Load += MainWindow_Load;

            this.Text = Strings.LoopbackManagerRes.Title;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
