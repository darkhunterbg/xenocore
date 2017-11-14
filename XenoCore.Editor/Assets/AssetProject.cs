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
    public class AssetProject : AssetEntry
    {
        internal ContentPipelineProject PipelineProject { get; private set; }

        public List<AssetResource> AllResources { get; private set; } = new List<AssetResource>();
        public List<AssetDirectory> AllDirectories { get; protected set; } = new List<AssetDirectory>();

        public String ContentLocation { get; private set; }
        public String ContentOutputLocation { get; private set; }
        public String ProjectFilePath { get; private set; }

        public AssetProject(ContentPipelineProject project)
        {
            this.PipelineProject = project;

            ProjectFilePath = Path.GetFullPath(project.FilePath.NormalizePath());
            ContentLocation = Path.GetFullPath(project.Location.NormalizePath());
            ContentOutputLocation = Path.Combine(ContentLocation, project.OutputDir.NormalizePath());

            RelativePath = project.Project.Name;

        }


        public void Delete(AssetEntry entry)
        {
            if (entry is AssetDirectory)
                AssetsManagerService.Instance.DeleteDirectories(this, entry as AssetDirectory);
            else
            if (entry is AssetResource)
                AssetsManagerService.Instance.DeleteResources(this, entry as AssetResource);
        }

        public void Rename(AssetEntry entry, String newName)
        {
            var path = Path.Combine(Path.GetDirectoryName(entry.RelativePath), newName);

            if (entry is AssetDirectory)
                AssetsManagerService.Instance.MoveDirectory(this, entry as AssetDirectory, path);
            else
            if (entry is AssetResource)
                AssetsManagerService.Instance.MoveResource(this, entry as AssetResource, path);
        }
    }
}
