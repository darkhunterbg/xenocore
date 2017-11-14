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
    public class AssetsManagerService
    {
        public static AssetsManagerService Instance { get { return App.CurrentApp.Services.GetService<AssetsManagerService>(); } }

        public ObservableCollection<AssetProject> Projects { get; private set; } = new ObservableCollection<AssetProject>();

        public ObservableCollection<AssetResource> AllResources { get; private set; } = new ObservableCollection<AssetResource>();

        public event EventHandler<AssetDirectory> OnDirectoryDeleted;
        public event EventHandler<AssetDirectory> OnDirectoryAdded;

        public AssetsManagerService()
        {

        }

        public void AddProject(String projectPath)
        {
            var project = ContentPipelineProject.Open(projectPath);
            Projects.Add(new AssetProject(project));

            foreach (var r in Projects.Last().AllResources)
                AllResources.Add(r);
        }

        public void DeleteResources(AssetProject project, params AssetResource[] resources)
        {
            foreach (var resource in resources)
            {
                project.PipelineProject.RemoveItem(resource.PipelineItem);
                var path = Path.Combine(project.ContentLocation, resource.RelaitvePath);
                File.Delete(path);

                resource.Parent.Children.Remove(resource);
                AllResources.Remove(resource);

            }

            project.PipelineProject.Save();
        }


        public void DeleteDirectories(AssetProject project, params AssetDirectory[] directories)
        {
            foreach (var directory in directories)
            {
                var resources = directory.Children.Where(p => p is AssetResource).Select(p => p as AssetResource).ToArray();
                if (resources.Length > 0)
                    DeleteResources(project, resources);

                var dirs = directory.Children.Where(p => p is AssetDirectory).Select(p => p as AssetDirectory).ToArray();
                if (dirs.Length > 0)
                    DeleteDirectories(project, dirs);

                var path = Path.Combine(project.ContentLocation, directory.Name);
                Directory.Delete(path);

                directory.Parent.Children.Remove(directory);
                OnDirectoryDeleted?.Invoke(this, directory);

            }

            project.PipelineProject.Save();
        }

    }
}
