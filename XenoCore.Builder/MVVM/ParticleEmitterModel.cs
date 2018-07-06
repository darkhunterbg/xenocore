using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Particles;

namespace XenoCore.Builder.MVVM
{
    public class ParticleEmitterModel : BaseModel
    {
        public ParticleEmitterDescription Emitter { get; private set; }

        public ObjectEditorModel EditorModel { get; private set; }

        public ObjectPropertyModel Name { get; private set; }

        public override string ToString()
        {
            return Name.Value.ToString();
        }

        public ParticleEmitterModel(ParticleEmitterDescription emitter)
        {
            EditorModel = new ObjectEditorModel(emitter);
            Name = EditorModel.GetProperty<ObjectPropertyModel>("Name");
            this.Emitter = emitter;
        }
    }
}
