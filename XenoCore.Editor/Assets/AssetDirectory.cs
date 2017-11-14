using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XenoCore.Editor.Assets
{
    public class AssetDirectory : AssetParent
    {
        public AssetProject Project { get; private set; }

        public String RelativePath { get; private set; } = String.Empty;

        public AssetDirectory(String name, AssetParent parent, AssetProject project) 
        {
            this.Project = project;
            base.Parent = parent;
            this.Name = name;

            if (parent is AssetDirectory)
                RelativePath = Path.Combine((parent as AssetDirectory).RelativePath, Name);
        }
    }
}
