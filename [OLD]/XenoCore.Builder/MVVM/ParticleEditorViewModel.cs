using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Builder.Data;
using XenoCore.Builder.Screens;
using XenoCore.Builder.Services;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Particles;
using XenoCore.Engine.Resources;
using XenoCore.Engine.World;

namespace XenoCore.Builder.MVVM
{
    public class ParticleEditorViewModel : EditorViewModel
    {
        private ParticleEffectModel effect;
        private ParticleEmitterModel emitter;

        public ResourceObj Resource { get; private set; }

        public String EffectName
        {
            get { return effect?.Effect.Name; }
            set
            {
                Screen.ParticleSystem.RenameParticleEffect(effect.Effect, value);
                OnPropertyChanged("EffectName");
                Modified = true;
            }
        }
        public ParticleEffectModel EditingEffect
        {
            get { return effect; }
            set
            {
                effect = value;
                OnPropertyChanged("EditingEffect");
                OnPropertyChanged("EffectName");

                EditingEmitter = effect.Emitters.FirstOrDefault();
              
            }
        }
        public ObjectEditorModel EditingModule
        {
            get
            {

                return emitter?.EditorModel.GetProperty<ObjectPropertyListModel>("Modules")?.Selected;
            }
            set
            {
                emitter.EditorModel.GetProperty<ObjectPropertyListModel>("Modules").Selected = value;

                OnPropertyChanged("EditingModule");
                OnPropertyChanged("CanRemoveModule");
            
            }
        }

        public bool CanRemoveModule
        {
            get { return EditingModule != null && !(EditingModule.Object is RequiredModule); }

        }
        public bool CanRemoveEmitter
        {
            get { return EditingEmitter != null; }
        }
        public bool EmitterSelected
        {
            get { return EditingEmitter != null; }
        }

        public String Title
        {
            get { return $"{Resource?.ContentPath}{ (Modified ? "*" : String.Empty )}"; }
        }

        public ParticleEmitterModel EditingEmitter
        {
            get { return emitter; }
            set
            {
                if (emitter != null)
                    emitter.EditorModel.PropertyChanged -= EditingEmitter_PropertyChanged;

                emitter = value;

                OnPropertyChanged("EditingEmitter");
                OnPropertyChanged("CanRemoveEmitter");
                OnPropertyChanged("EmitterSelected");

                if (emitter != null)
                {
                    emitter.EditorModel.PropertyChanged += EditingEmitter_PropertyChanged;
                    EditingModule = emitter?.EditorModel.GetProperty<ObjectPropertyListModel>("Modules").Collection.FirstOrDefault();
                }
            }
        }

        public ParticleEffectScreen Screen { get; private set; }
        public bool Initialized
        {
            get { return Screen != null; }
        }


        public void  Initialize(ResourceObj resource)
        {
            this.Resource = resource;

            Screen = new ParticleEffectScreen();

            var effect = Resource.Instance as ParticleEffectDescription;


            Screen.SetParticleEffect(effect);

          
            EditingEffect = new ParticleEffectModel(effect);
            EditingEffect.PropertyChanged += EditingEffect_PropertyChanged;

            Modified = false;

            this.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == "Modified")
                {
                    OnPropertyChanged("Title");
                }
            };
        }


        public void Save()
        {
            if (!Modified)
                return;

            Modified = false;
            ResourceManagerService.SaveResource(Resource);
        }

        public void RemoveSelectedModule()
        {
            var module = EditingModule.Object as ParticleModule;
            EditingEmitter.Emitter.Modules.Remove(module);
            EditingEmitter.EditorModel.GetProperty<ObjectPropertyListModel>("Modules").Collection.Remove(EditingModule);
            Screen.Restart();
        }
        public void AddModule(Type moduleType)
        {
            var module = Activator.CreateInstance(moduleType) as ParticleModule;
            EditingEmitter.Emitter.Modules.Add(module);
            var editingModule = new ObjectEditorModel(module);
            EditingEmitter.EditorModel.GetProperty<ObjectPropertyListModel>("Modules").Collection.Add(editingModule);
            editingModule.PropertyChanged += EditingEmitter.EditorModel.Property_PropertyChanged;
            EditingModule = editingModule;
            Screen.Restart();
        }
        public void RemoveSelectedEmitter()
        {
            EditingEffect.Effect.Emitters.Remove(EditingEmitter.Emitter);
            EditingEffect.Emitters.Remove(EditingEmitter);
            Screen.Restart();
        }
        public void AddEmitter()
        {
            ParticleEmitterDescription emitter = new ParticleEmitterDescription($"Emitter{EditingEffect.Emitters.Count}");
 
            emitter.Modules.Add(new LifetimeModule());
            emitter.Modules.Add(new SpawnContiniouslyModule());

            var model = new ParticleEmitterModel(emitter);

            EditingEffect.Effect.Emitters.Add(emitter);
            EditingEffect.Emitters.Add(model);

            EditingEmitter = model;
            Screen.Restart();
        }

        private void EditingEffect_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Modified = true;

            if (e.PropertyName == "Name")
                return;

            Screen.Restart();
        }
        private void EditingEmitter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            Modified = true;

            if (e.PropertyName == "Name" || e.PropertyName == "DebugColor")
            {
                OnPropertyChanged($"EditingEmitter.{e.PropertyName}");
                return;
            }
            if (e.PropertyName == "Modules.Selected")
            {
                OnPropertyChanged("EditingModule");
                OnPropertyChanged("CanRemoveModule");
                return;
            }
            Screen.Restart();
        }
    }
}
