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

namespace XenoCore.Editor.Controls
{
    /// <summary>
    /// Interaction logic for AssetsExplorer.xaml
    /// </summary>
    public partial class AssetsExplorer : BaseUserControl  , IDisposable
    {
        private String _searchTerm;
        public String SearchTerm { get { return _searchTerm; } set { _searchTerm = value; OnPropertyChanged(); } }


        public ObservableCollection<AssetProject> Items { get { return AssetsManagerService.Instance.Projects; } }

        public AssetsExplorer()
        {
            InitializeComponent();

            //foreach (var i in AssetsManagerService.Instance.Projects)
            //    Items.Add(i);

        }

        public void Dispose()
        {
        }
    }
}
