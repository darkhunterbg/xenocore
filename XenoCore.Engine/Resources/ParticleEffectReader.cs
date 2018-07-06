using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Particles;

namespace XenoCore.Engine.Resources
{
    public class ParticleEffectReader : ContentTypeReader<ParticleEffectDescription>
    {
        protected override ParticleEffectDescription Read(ContentReader input, ParticleEffectDescription existingInstance)
        {
            var json = input.ReadString();
            return XenoCoreJson.Deserialize<ParticleEffectDescription>(json);
        }
    }
}
