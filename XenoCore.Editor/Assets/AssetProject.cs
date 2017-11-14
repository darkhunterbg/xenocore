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
    public abstract class AssetParent : AssetEntry
    {


        public ObservableCollection<AssetEntry> Children { get; private set; } = new ObservableCollection<AssetEntry>();
    }


    public abstract class AssetEntry : BaseVM
    {
        private AssetParent _parent;
        private String _name;
        public String Name { get { return _name; } protected set { _name = value; OnPropertyChanged(); } }


        public AssetParent Parent
        {
            get { return _parent; }
            protected set
            {
                _parent = value; OnPropertyChanged();
                Depth = (Parent?.Depth ?? -1) + 1;
            }
        }

        public int Depth { get; private set; }
    }

    public class AssetProject : AssetParent
    {
        internal ContentPipelineProject PipelineProject { get; private set; }


        public ObservableCollection<AssetResource> AllResources { get; private set; } = new ObservableCollection<AssetResource>();

        public String ContentLocation { get; private set; }
        public String ContentOutputLocation { get; private set; }
        public String ProjectFilePath { get; private set; }

        public AssetProject(ContentPipelineProject project)
        {
            this.PipelineProject = project;

            ProjectFilePath = Path.GetFullPath(project.FilePath.NormalizePath());
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
            AssetParent parent = this;

            if (!String.IsNullOrEmpty(item.Location))
            {
                String[] dirs = item.Location.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var dir in dirs)
                {
                    var prev = parent;
                    parent = collection.FirstOrDefault(p => p.Name == dir && p is AssetDirectory) as AssetDirectory;
                    if (parent == null)
                        collection.Add(parent = new AssetDirectory(dir, prev, this));

                    collection = (parent as AssetDirectory).Children;
                }
            }

            var asset = new AssetResource(item, parent, this);

            AllResources.Add(asset);
            collection.Add(asset);
        }

        public void DeleteResource(AssetResource resource)
        {
            AssetsManagerService.Instance.DeleteResources(this, resource);
        }

        public void DeleteDirectory(AssetDirectory directory)
        {
            AssetsManagerService.Instance.DeleteDirectories(this, directory);
        }
    }
}
