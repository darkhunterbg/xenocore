using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace XenoCore.Editor.Controls.Data
{
    public class AssetExplorerItem : BaseVM
    {
        private String _name;
        private bool _selected;
        private bool _expanded;
        private bool _editing;
        private String _editingText;

        public AssetEntry Asset { get; protected set; }
        public String Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public ImageSource Image { get; protected set; }

        public ObservableCollection<AssetExplorerItem> Children { get; private set; } = new ObservableCollection<AssetExplorerItem>();

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
        public bool Editing
        {
            get { return _editing; }
            set
            {
                _editing = value;
                if (_editing)
                    EditingText = Name;
                OnPropertyChanged();
            }
        }
        public String EditingText
        {
            get { return _editingText; }
            set
            {
                _editingText = value;
                OnPropertyChanged();
            }
        }


        public AssetExplorerItem(AssetEntry entry)
        {
            Asset = entry;
            Name = entry.Name;

            if (entry is AssetProject)
                Image = App.Current.Resources["ContentProjectIcon"] as ImageSource;
            else
            if (entry is AssetDirectory)
                Image = App.Current.Resources["DirectoryIcon"] as ImageSource;
            else
                Image = (entry as AssetResource).Image;
        }
    }
}

namespace XenoCore.Editor.Controls
{
    public partial class AssetsExplorer : BaseUserControl, IDisposable
    {
        private Func<AssetEntry, bool> filerPredicate;

        private AssetExplorerItem _selectedItem;
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
        public AssetExplorerItem SelectedItem
        {
            get { return _selectedItem; }
            private set { _selectedItem = value; OnPropertyChanged(); }
        }

        public ObservableCollection<AssetExplorerItem> Items { get; private set; } = new ObservableCollection<AssetExplorerItem>();

        public AssetsExplorer()
        {
            AssetsManagerService.Instance.AllResources.CollectionChanged += AllResources_CollectionChanged;
            AssetsManagerService.Instance.AllDirectories.CollectionChanged += AllResources_CollectionChanged;

            InitializeComponent();

            Filter();
        }

        public void Dispose()
        {
            AssetsManagerService.Instance.AllResources.CollectionChanged -= AllResources_CollectionChanged;
            AssetsManagerService.Instance.AllDirectories.CollectionChanged -= AllResources_CollectionChanged;
        }

        private void AllResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (AssetEntry item in e.NewItems)
                            if (filerPredicate?.Invoke(item) ?? true)
                                AddEntry(item, false);
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (AssetEntry item in e.OldItems)
                            if (filerPredicate?.Invoke(item) ?? true)
                                RemoveEntry(item);
                        break;
                    }
                default:
                    throw new NotSupportedException(e.Action.ToString());
            }

        }

        private void Filter()
        {
            Items.Clear();

            IEnumerable<AssetEntry> collection = AssetsManagerService.Instance.AllResources;

            bool expand = false;

            filerPredicate = null;

            if (!String.IsNullOrEmpty(SearchTerm))
            {

                var st = SearchTerm.ToLower();
                filerPredicate = p => p.Name.ToLower().Contains(st);

                collection = collection.Where(filerPredicate);
                expand = true;
            }

            foreach (var resources in collection.OrderByDescending(p => p.Depth).ThenBy(p => p.RelativePath))
            {
                AddEntry(resources, expand);
            }

            if (filerPredicate == null)
            {
                if (Items.Count != AssetsManagerService.Instance.Projects.Count)
                {
                    foreach (var p in AssetsManagerService.Instance.Projects)
                    {
                        if (Items.All(q => q.Asset != p))
                            Items.Add(new AssetExplorerItem(p));
                    }
                }

                foreach (var i in Items)
                    i.Expanded = true;
            }
        }

        private AssetExplorerItem AddEntry(AssetEntry entry, bool expandParents)
        {
            Stack<AssetEntry> path = new Stack<AssetEntry>();
            AssetEntry pathItem = entry.Parent;
            while (pathItem != null)
            {
                path.Push(pathItem);
                pathItem = pathItem.Parent;
            }

            AssetExplorerItem parent = null;

            while (path.Count > 0)
            {
                var prev = parent;

                var collection = prev == null ? Items : parent.Children;

                pathItem = path.Pop();

                parent = collection.FirstOrDefault(p => p.Asset == pathItem) as AssetExplorerItem;
                if (parent == null)
                {
                    parent = new AssetExplorerItem(pathItem);

                    if (expandParents)
                        parent.Expanded = true;

                    collection.Add(parent);

                }
            }

            AssetExplorerItem item = parent.Children.FirstOrDefault(p => p.Asset == entry);

            if (item == null)
                item = new AssetExplorerItem(entry);

            parent.Children.Add(item);

            return item;
        }
        private void RemoveEntry(AssetEntry entry)
        {
            Stack<AssetEntry> path = new Stack<AssetEntry>();
            AssetEntry pathItem = entry.Parent;
            while (pathItem != null)
            {
                path.Push(pathItem);
                pathItem = pathItem.Parent;
            }

            AssetExplorerItem parent = null;

            while (path.Count > 0)
            {
                var prev = parent;

                var collection = prev == null ? Items : parent.Children;

                pathItem = path.Pop();

                parent = collection.FirstOrDefault(p => p.Asset == pathItem) as AssetExplorerItem;
            }

            var item = parent.Children.FirstOrDefault(p => p.Asset == entry);

            if (item != null)
                parent.Children.Remove(item);
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

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void tvAssets_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;

                var cm = tvAssets.Resources["contextMenu"] as ContextMenu;
                var miOpen = cm.Items[0] as MenuItem;
                var miRename = cm.Items[1] as MenuItem;
                var miDelete = cm.Items[2] as MenuItem;


                var item = treeViewItem.Header as AssetExplorerItem;

                treeViewItem.ContextMenu = item.Asset is AssetProject ? null : cm;

                miOpen.Visibility = item.Asset is AssetResource ? Visibility.Visible : Visibility.Collapsed;
            }

        }

        private void miDelete_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem.Asset.Project.Delete(SelectedItem.Asset);
        }

        private void miRename_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem.Editing = true;
        }

        private void tvAssets_KeyUp(object sender, KeyEventArgs e)
        {

            if (SelectedItem == null)
                return;

            switch (e.Key)
            {
                case Key.F2:
                    {
                        miRename_Click(this, new RoutedEventArgs());
                        break;
                    }
                case Key.Enter:
                    {
                        if (SelectedItem.Editing)
                        {
                            var newName = SelectedItem.EditingText;
                            SelectedItem.Editing = false;

                            if (SelectedItem.Name != newName)
                            {
                                if(SelectedItem.Asset is AssetDirectory)
                                {
                                    //Workaround: refresh ui
                                    //btnRefresh_Click(sender, new RoutedEventArgs());
                                }
                                SelectedItem.Asset.Project.Rename(SelectedItem.Asset, newName);
                            }
                        }
                        break;
                    }
                case Key.Escape:
                    {
                        if (SelectedItem.Editing)
                            SelectedItem.Editing = false;
                        break;
                    }
                case Key.Delete:
                    {
                        if (!SelectedItem.Editing)
                            miDelete_Click(this, new RoutedEventArgs());
                        break;
                    }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void tvAssets_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue as AssetExplorerItem;
        }
    }
}
