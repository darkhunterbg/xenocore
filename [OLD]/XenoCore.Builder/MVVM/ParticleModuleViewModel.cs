using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Particles;

namespace XenoCore.Builder.MVVM
{
    public class ModuleTypeModel
    {
        public String Name { get; private set; }
        public Type Type { get; private set; }

        public ModuleTypeModel(Type type)
        {
            this.Type = type;
            this.Name = type.Name;
        }
    }

    public class ParticleModuleViewModel : BaseModel
    {
        private ModuleTypeModel selection = null;
        private String search = null;

        public String TypeName { get; private set; }

        public ModuleTypeModel SelectedModule
        {
            get { return selection; }
            set
            {
                selection = value;
                OnPropertyChanged("SelectedModule");
                OnPropertyChanged("IsOK");
            }
        }
        public bool IsOK
        {
            get
            {
                return selection != null;
            }
        }

        public String FilterText
        {
            get { return search; }
            set
            {
                search = value;
                OnPropertyChanged("FilterText");
                if (String.IsNullOrEmpty(search))
                {
                    Modules = AllModules;
                }
                else
                {
                    Modules = AllModules.Where(p => p.Name.ToLower().Contains(search.ToLower()));
                }
                OnPropertyChanged("Modules");
                SelectedModule = null;
                OnPropertyChanged("SelectedModule");
            }
        }

        private List<ModuleTypeModel> AllModules;

        public IEnumerable<ModuleTypeModel> Modules
        {
            get;private set;
        }


        public ParticleModuleViewModel()
        {

            Modules = AllModules = Assembly.Load("XenoCore.Engine").ExportedTypes.Where(p =>
            p.IsSubclassOf(typeof(ParticleModule)) && (p != typeof(RequiredModule)) && !p.IsAbstract)
            .Select(p=> new ModuleTypeModel(p)).OrderBy(p=>p.Name).ToList();

        }
    }
}
