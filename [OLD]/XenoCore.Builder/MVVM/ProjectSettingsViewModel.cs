using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Builder.Data;

namespace XenoCore.Builder.MVVM
{
    public class ProjectSettingsViewModel : BaseModel
    {
        private bool valid;
        private String projectDir;

        public BuilderProject Settings { get; private set; } = new BuilderProject();

        public bool IsNew { get; private set; } = true;

        public String ProjectDir
        {
            get { return projectDir; }
            set
            {
                projectDir = value;
                OnPropertyChanged("ProjectDir");
                Validate();
            }
        }
        public String ProjectName
        {
            get { return Settings.ProjectName; }
            set
            {
                Settings.ProjectName = value;
                OnPropertyChanged("ProjectName");
                Validate();
            }
        }
        public String OutputContentDir
        {
            get { return Settings.OutputContentDir; }
            set
            {
                Settings.OutputContentDir = value;
                OnPropertyChanged("OutputContentDir");
                Validate();
            }
        }

        public bool Valid
        {
            get
            {
                return valid;
            }
            set
            {
                valid = value;
                OnPropertyChanged("Valid");
            }
        }

        public ProjectSettingsViewModel()
        {

        }
        public void UseExistingProject(BuilderProject project)
        {
            this.Settings = project;
            OnPropertyChanged("ProjectName");
            OnPropertyChanged("OutputContentDir");
            IsNew = false;
            OnPropertyChanged("IsNew");
            Validate();

        }

        private void Validate()
        {
            Valid = (!IsNew || Directory.Exists(ProjectDir)) &&
                !String.IsNullOrEmpty(ProjectName) && !ProjectName.Contains(' ') &&
                Directory.Exists(OutputContentDir);
        }
    }
}
