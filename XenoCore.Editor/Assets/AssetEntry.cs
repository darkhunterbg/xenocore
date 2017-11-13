using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Editor.Assets
{
    public enum AssetEntryType
    {
        Unknown,
        Project,
        Directory,
        Texture,
        SpriteFont,
        Effect,
    }

    public abstract class AssetEntry : BaseVM
    {
        private String _name;

        public AssetEntryType Type { get; private set; }

        public String Name { get { return _name; } protected set { _name = value; OnPropertyChanged(); } }

        public AssetEntry(AssetEntryType type)
        {
            this.Type = type;
        }
    }
}
