using System;
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
        /// <summary>
        /// The title of the application
        /// </summary>
        private readonly string _appTitle = "XML Viewer";

        /// <summary>
        /// The constructor. 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Title = _appTitle;
            vXMLViwer.CalculateSelectedNodePath = true;
        }

        /// <summary>
        /// When user presses the Open menu, this event handler is called. 
        /// It opens the file dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                CheckFileExists = true,
                Filter = "XML Files (*.xml)|*.xml|All Files(*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() != true) { return; }

            XmlDocument XMLdoc = new();
            try
            {
                XMLdoc.Load(openFileDialog.FileName);
            }
            catch (XmlException)
            {
                MessageBox.Show("The XML file is invalid");
                return;
            }
            Title = _appTitle + openFileDialog.SafeFileName;
            vXMLViwer.XmlDocument = XMLdoc;
        }

        /// <summary>
        /// When user presses the Close menu, this event handler is called. 
        /// It closes the file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {
            Title = _appTitle;
            textBlockFilePath.Text = string.Empty;
            vXMLViwer.XmlDocument = null;
        }

        /// <summary>
        /// When user presses the Exit menu, this event handler is called. 
        /// It terminates the program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the file has been closed before exit
            MenuClose_Click(sender,e);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Event handler for when user select a node in the tree view. It shows the path in the view. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void XMLViwer_SelectedItemChanged(object sender, EventArgs e)
        {
            if (e is SelectedItemChangedEventArgs selectedItemChangedEventArgs)
            {
                textBlockFilePath.Text = "Path: " + selectedItemChangedEventArgs.SelectedNodePath;
            }
            else
            {
                textBlockFilePath.Text = string.Empty;
            }
        }
    }
}
