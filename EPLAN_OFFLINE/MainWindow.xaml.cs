using Eplan.EplApi.DataModel;
using Eplan.EplApi.Starter;
using Eplan.EplApi.System;
using Microsoft.Win32;
using System.Windows;

namespace EPLAN_OFFLINE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EplApplication? m_oEplApp;
        private bool m_bIsEplanAlreadyInitialized = false;
        private string m_strEplanBinFolder = "";

        public MainWindow()
        {
            InitializeComponent();
            
            GetEplanBinFolder();
        }

        private void Page_Count_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select eplan project file",
                Filter = "EPLAN project (*.elk)|*.elk",
                CheckFileExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog(this) != true)
            {
                return;
            }

            if (!EPLInit())
            {
                return;
            }

            try
            {
                using (LockingStep lockingStep = new LockingStep())
                {
                    Project project = new ProjectManager().OpenProject(openFileDialog.FileName);

                    try
                    {
                        int pageCount = project.Pages.Length;
                        MessageBox.Show(
                            $"Project：{project.ProjectName}\nPage count：{pageCount}",
                            "EPLAN project info");
                    }
                    finally
                    {
                        project.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed when reading project info");
            }
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

        private bool EPLInit()
        {
            try
            {
                if (m_bIsEplanAlreadyInitialized)
                {
                    return true;
                }

                if (string.IsNullOrEmpty(m_strEplanBinFolder))
                {
                    MessageBox.Show(
                        "EPLAN version is not confirmed", 
                        "EPLAN initialization failed");
                    return false;
                }

                m_oEplApp = new EplApplication
                {
                    EplanBinFolder = m_strEplanBinFolder
                };

                m_oEplApp.Init("");
                m_bIsEplanAlreadyInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{ex.GetType().FullName}\n" +
                    $"HRESULT: 0x{ex.HResult:X8}\n\n" +
                    ex,
                    "EPLAN initialization failed");
                return false;
            }
        }

        private void EPLExit()
        {
            try
            {
                if (m_bIsEplanAlreadyInitialized && m_oEplApp != null)
                {
                    m_oEplApp.Exit();
                    m_oEplApp = null;
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
