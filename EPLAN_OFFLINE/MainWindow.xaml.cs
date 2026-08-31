using Eplan.EplApi.Starter;
using Eplan.EplApi.System;
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

namespace EPLAN_OFFLINE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EplApplication m_oEplApp = new EplApplication();
        private bool m_bIsEplanAlreadyInitialized = false;
        private string m_strEplanBinFolder = "";

        public MainWindow()
        {
            InitializeComponent();
            GetEplanBinFolder();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            EPLInit();
        }

        private void GetEplanBinFolder()
        {
            // Use the finder to find the correct eplan version if not yet known
            EplanFinder oEplanFinder = new EplanFinder();
            String strBinPath = oEplanFinder.SelectEplanVersion(true);

            // Check if user has selected Eplan version
            if (String.IsNullOrEmpty(strBinPath))
            {
                return;
            }

            // Now use the Assemblyresolver to let the program know where all eplan assemblies can be found.
            AssemblyResolver oResolver = new AssemblyResolver();
            oResolver.SetEplanBinPath(strBinPath);
            // Pin to eplan does the actual preparation. All referenced eplan assemblies are loaded from the bin path.
            oResolver.PinToEplan();

            m_strEplanBinFolder = oResolver.GetEplanBinPath();
        }

        private void EPLInit()
        {
            try
            {
                if (!m_bIsEplanAlreadyInitialized)
                {
                    if (!String.IsNullOrEmpty(m_strEplanBinFolder))
                    {
                        m_oEplApp.EplanBinFolder = m_strEplanBinFolder;
                    }
                    m_oEplApp.Init("");
                    m_bIsEplanAlreadyInitialized = true;
                }
            }
            catch (Exception)
            {
                //ToDo add an exception handling code here
            }

        }


        private void EPLExit()
        {
            try
            {
                if (m_bIsEplanAlreadyInitialized)
                {
                    m_oEplApp.Exit();
                    m_bIsEplanAlreadyInitialized = false;
                }
            }
            catch (Exception)
            {
                //ToDo add an exception handling code here
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EPLExit();
        }
    }
}