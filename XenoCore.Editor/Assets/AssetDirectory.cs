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
    public class AssetDirectory : AssetEntry
    {
        public AssetDirectory(String relativePath, AssetProject project)
        {
            this.Project = project;

            RelativePath = relativePath;
        }
    }
}
