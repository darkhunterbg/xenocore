using MonoGame.Tools.Pipeline;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Builder.Data;
using XenoCore.Builder.Services;
using XenoCore.ContentPipeline;
using XenoCore.Engine.Particles;
using XenoCore.Engine.Resources;
using XenoCore.Engine.Screens;

namespace XenoCore.Builder.MVVM
{
    public class MainWindowViewModel : BaseModel
    {
        private String title;
        public String ProjectFile { get; private set; }

        private BuilderProject project;
        public ContentPipelineProject PipelineProject { get; private set; }

        public BuilderProject CurrentProject
        {
            get { return project; }
            private set
            {
                project = value;
                OnPropertyChanged("CurrentProject");
                if (project == null)
                {
                    Title = "XenoCore Builder";
                }
                else
                {
                    Title = $"XenoCore Builder [{project.ProjectName}]";
                }
                OnPropertyChanged("ProjectLoaded");
            }
        }

        public String Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");

            }
        }

        public bool ProjectLoaded
        {
            get { return project != null; }
        }

        public Engine.Screens.Screen CurrentScreen
        {
            get;
            private set;
        }

        public MainWindowViewModel()
        {
            CurrentProject = null;
        }

        public bool Initialized
        {
            get { return Engine.Services.Initialized; }
        }

        public bool OpenProject(String projectPath)
        {
            try
            {
                ProjectFile = Path.GetFullPath(projectPath);
                CurrentProject = BuilderProject.Load(ProjectFile);
                var file = Path.Combine(Path.GetDirectoryName(ProjectFile), "Content", "Content.mgcb");
                PipelineProject = ContentPipelineProject.Open(file);
                PipelineProject.Build();

                App.CurrentApp.LoadServices(PipelineProject);

                OnPropertyChanged("Initialized");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                OnPropertyChanged("Initialized");
                return false;
            }
        }
        public void NewProject(BuilderProject settings, String directory)
        {
            var dir = Path.GetFullPath(directory);
            var projectFile = Path.Combine(dir, $"{settings.ProjectName}.xcp");

            settings.SaveTo(projectFile);

            var plProjectFile = Path.Combine(dir, "Content", "Content.mgcb");

            PipelineProject = new ContentPipelineProject(plProjectFile, settings.OutputContentDir);
            PipelineProject.Build();

            OpenProject(projectFile);
        }
        public void UpdateSettings()
        {
            if (PipelineProject.OutputDir != CurrentProject.OutputContentDir)
            {
                ResourceManagerService.ChangeOutputDir(CurrentProject.OutputContentDir);
            }

            CurrentProject.SaveTo(ProjectFile);

            OnPropertyChanged("CurrentProject");

        }

    }
}
