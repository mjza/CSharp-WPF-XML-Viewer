using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

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
        /// The selected node path
        /// </summary>
        private String? _selectedNodePath = string.Empty;

        /// <summary>
        /// When false, the path does not calculated
        /// </summary>
        private Boolean _calculateSelectedNodePath = false;

        public event EventHandler? SelectedItemChanged;

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
        /// Returns the selected node's path
        /// </summary>
        public string? SelectedNodePath { get; }

        /// <summary>
        /// Getters and setters for CalculateSelectedNodePath
        /// </summary>
        public Boolean CalculateSelectedNodePath
        {
            get => _calculateSelectedNodePath;
            set {
                _calculateSelectedNodePath = value;
                if (!value) {
                    _selectedNodePath = string.Empty;
                }
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
        /// Checks if the passed object is null or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if the passed object is not null, otherwise false.</returns>
        private static bool IsNotNull([NotNullWhen(true)] object? obj) => obj != null;

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
        /// An event handler for when user select a node in the tree view 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            if (this.CalculateSelectedNodePath == false) {
                return;
            }
            try
            {
                XmlNode? xmlNode = e.NewValue as XmlNode;
                if (IsNotNull(xmlNode))
                {
                    _selectedNodePath = FindXPath(xmlNode);
                }
                else
                {
                    _selectedNodePath = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _selectedNodePath = string.Empty;
                Debug.WriteLine(ex.Message);
            }
            
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(_selectedNodePath));
        }

        /// <summary>
        /// Returns the node's path or throws an error 
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Path of the node</returns>
        /// <exception cref="ArgumentException"></exception>
        static string FindXPath(XmlNode? node)
        {
            StringBuilder builder = new();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Text:
                        string? text = node.Value;
                        builder.Insert(0, (IsNotNull(text) ? string.Concat("/\"", text[..Math.Min(15, text.Length)], "...\"") : string.Empty));
                        XmlText? xmlText = node as XmlText;
                        node = IsNotNull(xmlText) ? xmlText.ParentNode : null;
                        break;
                    case XmlNodeType.Attribute:
                        builder.Insert(0, string.Concat("/@", node.Name));
                        XmlAttribute? xmlAttribute = node as XmlAttribute;
                        node = IsNotNull(xmlAttribute) ? xmlAttribute.OwnerElement : null;
                        break;
                    case XmlNodeType.Element:
                        int index = FindElementIndex((XmlElement)node);
                        builder.Insert(0, string.Concat("/", node.Name, "[", index, "]"));
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new ArgumentException("Only elements and attributes are supported");
                }
            }
            throw new ArgumentException("Node was not in a document");
        }

        /// <summary>
        /// Returns node index or throws an exception
        /// </summary>
        /// <param name="element"></param>
        /// <returns>index of the node between its siblings</returns>
        /// <exception cref="ArgumentException"></exception>
        static int FindElementIndex(XmlElement element)
        {
            XmlNode? parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }
            XmlElement? parent = parentNode as XmlElement;
            if (IsNotNull(parent))
            {
                int index = 1;
                foreach (XmlNode candidate in parent.ChildNodes)
                {
                    if (candidate is XmlElement && candidate.Name == element.Name)
                    {
                        if (candidate == element)
                        {
                            return index;
                        }
                        index++;
                    }
                }
            }
            throw new ArgumentException("Couldn't find element within parent");
        }
    }
}