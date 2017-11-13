using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XenoCore.Editor.Assets;
using XenoCore.Editor.Controls.Data;

#region Class Data

namespace XenoCore.Editor.Controls.Data
{
    public abstract class AssetExplorerItem : BaseVM
    {
        private String _name;
        private bool _selected;
        private bool _expanded;

        public Object Item { get; protected set; }
        public String Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public ImageSource Image { get; protected set; }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                _expanded = value;
                OnPropertyChanged();
            }
        }
    }
    public abstract class AssetExplorerItemContainer : AssetExplorerItem
    {
        public ObservableCollection<AssetExplorerItem> Children { get; private set; } = new ObservableCollection<AssetExplorerItem>();
    }

    public class AssetExplorerProject : AssetExplorerItemContainer
    {
        public new AssetProject Item { get { return base.Item as AssetProject; } private set { base.Item = value; } }

        public AssetExplorerProject(AssetProject project)
        {
            Item = project;
            Name = project.Name;
            Image = App.Current.Resources["ContentProjectIcon"] as ImageSource;
        }
    }
    public class AssetExplorerDirectory : AssetExplorerItemContainer
    {
        public new AssetDirectory Item { get { return base.Item as AssetDirectory; } private set { base.Item = value; } }

        public AssetExplorerDirectory(AssetDirectory dir)
        {
            Item = dir;
            Name = dir.Name;
            Image = App.Current.Resources["DirectoryIcon"] as ImageSource;
        }
    }
    public class AssetExplorerResource : AssetExplorerItem
    {
        public new AssetResource Item { get { return base.Item as AssetResource; } private set { base.Item = value; } }
        public AssetExplorerResource(AssetResource resource)
        {
            Item = resource;
            Name = resource.Name;
            Image = resource.Image;
        }
    }
}

#endregion

namespace XenoCore.Editor.Controls
{
    public partial class AssetsExplorer : BaseUserControl, IDisposable
    {
        private ObservableCollection<AssetExplorerItem> allItems = new ObservableCollection<AssetExplorerItem>();

        private ObservableCollection<AssetExplorerItem> _items;

        private String _searchTerm = String.Empty;
        public String SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
                Filter();
            }
        }

        public ObservableCollection<AssetExplorerItem> Items { get { return _items; } private set { _items = value; OnPropertyChanged(); } }

        public AssetsExplorer()
        {
            Items = allItems;

            InitializeComponent();

            Filter();
        }

        private void Filter()
        {
            Items.Clear();

            IEnumerable<AssetResource> collection = AssetsManagerService.Instance.AllResources;

            bool expand = false;

            if (!String.IsNullOrEmpty(SearchTerm))
            {
                var st = SearchTerm.ToLower();
                collection = collection.Where(p => p.Name.ToLower().Contains(st));
                expand = true;
            }


            foreach (var resources in collection.OrderByDescending(p=>p.Depth).ThenBy(p=>p.RelaitvePath))
            {
                 AddResource(resources, expand);
            }

            if(!expand)
            {
                foreach (var i in Items)
                    i.Expanded = true;
            }
        }

        private AssetExplorerResource AddResource(AssetResource resource, bool expandParents)
        {
            Stack<AssetEntry> path = new Stack<AssetEntry>();
            AssetEntry pathItem = resource.Parent;
            while (pathItem != null)
            {
                path.Push(pathItem);
                pathItem = pathItem.Parent;
            }

            AssetExplorerItemContainer parent = null;

            while (path.Count > 0)
            {
                var prev = parent;

                var collection = prev == null ? Items : parent.Children;

                pathItem = path.Pop();

                parent = collection.FirstOrDefault(p => p.Item == pathItem) as AssetExplorerItemContainer;
                if (parent == null)
                {
                    if (pathItem is AssetProject)
                        parent = new AssetExplorerProject(pathItem as AssetProject);
                    else
                        parent = new AssetExplorerDirectory(pathItem as AssetDirectory);

                    if (expandParents)
                        parent.Expanded = true;

                    collection.Add(parent);

                }
            }

            var item = new AssetExplorerResource(resource);
            parent.Children.Add(item);

            return item;
        }


        public void Dispose()
        {
        }

        private void tvAssets_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (e.OriginalSource as TreeViewItem).Header as AssetExplorerItem;
            if (item != null)
                item.Expanded = true;
        }

        private void tvAssets_Collapsed(object sender, RoutedEventArgs e)
        {
            var item = (e.OriginalSource as TreeViewItem).Header as AssetExplorerItem;
            if (item != null)
                item.Expanded = false;
        }
    }
}
