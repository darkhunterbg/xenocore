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
        public ObservableCollection<AssetDirectory> AllDirectories { get; private set; } = new ObservableCollection<AssetDirectory>();

        public void AddProject(String projectPath)
        {
            var project = ContentPipelineProject.Open(projectPath);
            var assetProject = new AssetProject(project);
            Projects.Add(assetProject);

            foreach (var item in project.ContentItems)
            {
                var asset = new AssetResource(item, assetProject);
                Add(assetProject, asset);
            }

            foreach (var r in Projects.Last().AllResources)
                AllResources.Add(r);

            foreach (var d in Projects.Last().AllDirectories)
                AllDirectories.Add(d);
        }

        public void DeleteResources(AssetProject project, params AssetResource[] resources)
        {
            foreach (var resource in resources)
            {
                project.PipelineProject.RemoveItem(resource.PipelineItem);
                var path = Path.Combine(project.ContentLocation, resource.RelativePath);

                if (File.Exists(path))
                    File.Delete(path);

                AllResources.Remove(resource);
            }

            project.PipelineProject.Save();
        }
        public void MoveResource(AssetProject project, AssetResource resource, String newPath)
        {
            project.AllResources.Remove(resource);
            AllResources.Remove(resource);

            var oldFullPath = Path.Combine(project.ContentLocation, resource.RelativePath);
            resource.RelativePath = newPath;
            var newFullPath = Path.Combine(project.ContentLocation, resource.RelativePath);
            File.Move(oldFullPath, newFullPath);

            resource.PipelineItem.OriginalPath = resource.RelativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            Add(project, resource);
            AllResources.Add(resource);

            project.PipelineProject.Save();
        }
        public void MoveDirectory(AssetProject project, AssetDirectory directory, String newPath)
        {
            //Wont refresh ui, refresh it manually

            project.AllDirectories.Remove(directory);
            AllDirectories.Remove(directory);

            var oldFullPath = Path.Combine(project.ContentLocation, directory.RelativePath);
            directory.RelativePath = newPath;
            var newFullPath = Path.Combine(project.ContentLocation, directory.RelativePath);
            Directory.Move(oldFullPath, newFullPath);

            Add(project, directory);
            AllDirectories.Add(directory);


            UpdateChildrenPath(project, directory);

            project.PipelineProject.Save();
        }

        public void DeleteDirectories(AssetProject project, params AssetDirectory[] directories)
        {
            foreach (var directory in directories)
            {
                var resources = project.AllResources.Where(p => p.Parent == directory).ToArray();
                if (resources.Length > 0)
                    DeleteResources(project, resources);

                var dirs = project.AllDirectories.Where(p => p.Parent == directory).ToArray();
                if (dirs.Length > 0)
                    DeleteDirectories(project, dirs);

                var path = Path.Combine(project.ContentLocation, directory.Name);

                if (Directory.Exists(path))
                    Directory.Delete(path, true);

                AllDirectories.Remove(directory);
            }

            project.PipelineProject.Save();
        }

        private static void UpdateChildrenPath(AssetProject project, AssetDirectory directory)
        {
            foreach (var resource in project.AllResources.Where(p => p.Parent == directory).ToList())
            {
                resource.RelativePath = Path.Combine(directory.RelativePath, resource.Name);
                resource.PipelineItem.OriginalPath = resource.RelativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            foreach (var dir in project.AllDirectories.Where(p => p.Parent == directory).ToList())
            {
                dir.RelativePath = Path.Combine(directory.RelativePath, dir.Name);
                UpdateChildrenPath(project, dir);
            }
        }
        private static void Add(AssetProject project, AssetEntry entry)
        {
            AssetEntry parent = project;

            var location = Path.GetDirectoryName(entry.RelativePath);

            if (!String.IsNullOrEmpty(location))
            {
                String[] dirs = location.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                StringBuilder path = new StringBuilder();


                for (int i = 0; i < dirs.Length; ++i)
                {
                    string dir = dirs[i];
                    if (i == 0)
                        path.Append(dir);
                    else
                        path.Append($"{Path.DirectorySeparatorChar}{dir}");



                    var directory = project.AllDirectories.FirstOrDefault(p => p.RelativePath == path.ToString());

                    if (directory == null)
                    {
                        project.AllDirectories.Add(directory = new AssetDirectory(path.ToString(), project)
                        {
                            Parent = parent
                        });
                    }
                    parent = directory;

                }
            }

            entry.Parent = parent;
            if (entry is AssetResource)
                project.AllResources.Add(entry as AssetResource);
            else
                project.AllDirectories.Add(entry as AssetDirectory);
        }
    }
}
