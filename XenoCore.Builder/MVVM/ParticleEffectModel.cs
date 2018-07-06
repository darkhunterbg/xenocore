using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Particles;

namespace XenoCore.Builder.MVVM
{

    public class ParticleEffectModel : BaseModel
    {
        public ParticleEffectDescription Effect { get; private set; }

        public ObservableCollection<ParticleEmitterModel> Emitters { get; private set; }
        = new ObservableCollection<ParticleEmitterModel>();

 
        public override string ToString()
        {
            return Effect.Name;
        }

        public ParticleEffectModel(ParticleEffectDescription effect)
        {
            this.Effect = effect;

            foreach (var emitter in effect.Emitters)
            {
                Emitters.Add(new ParticleEmitterModel(emitter));
            }
        }
    }
}
