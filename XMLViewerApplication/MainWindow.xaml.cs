using System;
using System.Diagnostics;
using System.Windows;
using System.Xml;
using XMLViewer;

namespace XMLViewerApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly String _appTitle = "XML Viewer";
        public MainWindow()
        {
            InitializeComponent();
            this.Title = _appTitle;
            vXMLViwer.CalculateSelectedNodePath = true;
        }

        
        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files(*.*)|*.*";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() != true) { return; }

            XmlDocument XMLdoc = new XmlDocument();
            try
            {
                XMLdoc.Load(openFileDialog.FileName);
            }
            catch (XmlException)
            {
                MessageBox.Show("The XML file is invalid");
                return;
            }
            this.Title = _appTitle + openFileDialog.SafeFileName;
            vXMLViwer.XmlDocument = XMLdoc;
        }

        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {
            this.Title = _appTitle;
            textBlockFilePath.Text = string.Empty;
            vXMLViwer.XmlDocument = null;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.MenuClose_Click(sender,e);
            Application.Current.Shutdown();
        }

        private void XMLViwer_SelectedItemChanged(object sender, EventArgs e)
        {
            SelectedItemChangedEventArgs? selectedItemChangedEventArgs = e as SelectedItemChangedEventArgs;
            if (selectedItemChangedEventArgs != null)
            {
                textBlockFilePath.Text = "Path: " + selectedItemChangedEventArgs.SelectedNodePath;
            }            
        }
    }
}
