using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XenoCore.Editor.Assets
{
    public class AssetDirectory : AssetEntry
    {
        public ObservableCollection<AssetEntry> Children { get; private set; } = new ObservableCollection<AssetEntry>();

        public AssetDirectory(String name, AssetEntry parent) 
        {
            base.Parent = parent;
            this.Name = name;
        }
    }
}
