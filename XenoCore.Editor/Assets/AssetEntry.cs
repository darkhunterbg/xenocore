using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Editor.Assets
{
    public abstract class AssetEntry : BaseVM
    {
        private String _relativePath = String.Empty;
        private AssetEntry _parent;
        private String _name = String.Empty;
        public String Name { get { return _name; } private set { _name = value; OnPropertyChanged(); } }

        public AssetProject Project { get; protected set; }

        public String RelativePath
        {
            get
            {
                return _relativePath;
            }
            set
            {
                _relativePath = value;
                Name = Path.GetFileName(_relativePath);
                int length = _relativePath.Length - Name.Length;
                if (length > 0)
                    length -= 1;
            }
        }

        public AssetEntry Parent
        {
            get { return _parent; }
            set
            {
                _parent = value; OnPropertyChanged();
                Depth = (Parent?.Depth ?? -1) + 1;
            }
        }

        public int Depth { get; private set; }
    }
    //public abstract class AssetParent : AssetEntry
    //{
    //    public List<AssetEntry> Children { get; private set; } = new List<AssetEntry>();
    //}
}
