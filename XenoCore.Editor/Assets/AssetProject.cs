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
    public class AssetProject : AssetEntry
    {
        private ContentPipelineProject pipelineProject;

        public ObservableCollection<AssetEntry> Children { get; private set; } = new ObservableCollection<AssetEntry>();

        public String ContentLocation { get; private set; }
        public String ContentOutputLocation { get; private set; }

        public AssetProject(ContentPipelineProject project)
            : base(AssetEntryType.Project)
        {
            this.pipelineProject = project;

            ContentLocation = Path.GetFullPath(project.Location.NormalizePath());
            ContentOutputLocation = Path.Combine(ContentLocation, project.OutputDir.NormalizePath());

            Name = Path.GetFileName(project.FilePath);

            foreach (var item in project.ContentItems)
            {
                AddResource(new AssetResource(item));
            }
        }

        private void AddResource(AssetResource item)
        {
            ObservableCollection<AssetEntry> collection = Children;

            if (!String.IsNullOrEmpty(item.Location))
            {
                String[] dirs = item.Location.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var dir in dirs)
                {
                    var parent = collection.FirstOrDefault(p => p.Name == dir);
                    if (parent == null)
                        collection.Add(parent = new AssetDirectory(dir));

                    collection = (parent as AssetDirectory).Children;
                }
            }

            collection.Add(item);
        }
    }
}
