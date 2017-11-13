using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public AssetsManagerService()
        {

        }

        public void AddProject(String projectPath)
        {
            var project = ContentPipelineProject.Open(projectPath);
            Projects.Add(new AssetProject(project));
        }
    }
}
