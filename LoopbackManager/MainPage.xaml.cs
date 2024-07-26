using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Views.Pages;

namespace LoopbackManagerModern
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            mainFrame.Navigate(typeof(LoopbackManagerPage));
        }

    }
}
