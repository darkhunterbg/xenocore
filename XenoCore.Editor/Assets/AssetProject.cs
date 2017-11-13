using MonoGame.Tools.Pipeline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.ContentPipeline;

namespace XenoCore.Editor.Assets
{
    public abstract class AssetEntry : BaseVM
    {
        private AssetEntry _parent;
        private String _name;
        public String Name { get { return _name; } protected set { _name = value; OnPropertyChanged(); } }

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

    public class AssetProject : AssetEntry
    {
        private ContentPipelineProject pipelineProject;

        public ObservableCollection<AssetEntry> Children { get; private set; } = new ObservableCollection<AssetEntry>();

        public ObservableCollection<AssetResource> AllResources { get; private set; } = new ObservableCollection<AssetResource>();

        public String ContentLocation { get; private set; }
        public String ContentOutputLocation { get; private set; }

        public AssetProject(ContentPipelineProject project)
        {
            this.pipelineProject = project;

            ContentLocation = Path.GetFullPath(project.Location.NormalizePath());
            ContentOutputLocation = Path.Combine(ContentLocation, project.OutputDir.NormalizePath());

            Name = Path.GetFileName(project.FilePath);


            foreach (var item in project.ContentItems)
            {
                AddResource(item);
            }
        }

        private void AddResource(ContentItem item)
        {
            ObservableCollection<AssetEntry> collection = Children;
            AssetEntry parent = this;

            if (!String.IsNullOrEmpty(item.Location))
            {
                String[] dirs = item.Location.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var dir in dirs)
                {
                    var prev = parent;
                    parent = collection.FirstOrDefault(p => p.Name == dir && p is AssetDirectory) as AssetDirectory;
                    if (parent == null)
                        collection.Add(parent = new AssetDirectory(dir, prev));

                    collection = (parent as AssetDirectory).Children;
                }
            }

            var asset = new AssetResource(item, parent);

            AllResources.Add(asset);
            collection.Add(asset);
        }
    }
}
