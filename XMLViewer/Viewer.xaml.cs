using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace XMLViewer
{
    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : UserControl
    {
        /// <summary>
        /// The private field that keeps a reference to the selected xml document
        /// </summary>
        private XmlDocument? _xmlDocument = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public Viewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Getters and setters for the XML Document 
        /// </summary>
        public XmlDocument? XmlDocument
        {
            get => _xmlDocument;

            set
            {
                _xmlDocument = value;
                BindXMLDocument();
            }
        }

        /// <summary>
        /// Bind data to the TreeView
        /// </summary>
        private void BindXMLDocument()
        {
            if (_xmlDocument == null)
            {
                xmlTree.ItemsSource = null;
                return;
            }

            XmlDataProvider provider = new XmlDataProvider();
            provider.Document = _xmlDocument;
            Binding binding = new Binding();
            binding.Source = provider;
            binding.XPath = "child::node()";
            xmlTree.SetBinding(TreeView.ItemsSourceProperty, binding);
        }

        /// <summary>
        /// Shows waiting cursor while loading data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeNodeExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem? treeViewItem = e.OriginalSource as TreeViewItem;
            if (treeViewItem != null)
            {
                if (treeViewItem.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                {
                    EventHandler? itemsGenerated = null;
                    itemsGenerated = delegate (object? s, EventArgs args)
                    {
                        if (IsNotNull(s))
                        {
                            ItemContainerGenerator? itemContainerGenerator = s as ItemContainerGenerator;
                            if (IsNotNull(itemContainerGenerator))
                            {
                                if (itemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                                {
                                    itemContainerGenerator.StatusChanged -= itemsGenerated;
                                    treeViewItem.Dispatcher.BeginInvoke(DispatcherPriority.DataBind,
                                        (ThreadStart)delegate
                                        {
                                            Mouse.OverrideCursor = null;
                                        });
                                }
                            }

                        }
                    };
                    treeViewItem.ItemContainerGenerator.StatusChanged += itemsGenerated;
                    Mouse.OverrideCursor = Cursors.Wait;
                }
            }
        }

        /// <summary>
        /// Checks if the passed object is null or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if the passed object is not null, otherwise false.</returns>
        private static bool IsNotNull([NotNullWhen(true)] object? obj) => obj != null;
    }
}