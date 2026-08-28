using Eplan.EplApi.RemoteClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EPLAN_REMOTE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EplanRemoteClient oClient = new EplanRemoteClient();
        public MainWindow()
        {
            InitializeComponent();

            oClient.Connect("localhost", "49152");  // Default port for Eplan instance is 49152
        }

        private void Open_Part_Management_Click(object sender, RoutedEventArgs e)
        {
            bool oResp = oClient.ExecuteAction("XPartsManagementStart");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            oClient.Disconnect();
            oClient.Dispose();
        }
    }
}