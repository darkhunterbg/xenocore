using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Particles;
using XenoCore.Engine.Resources;

namespace XenoCore.ContentPipeline.Content
{
    //http://dylanwilson.net/creating-custom-content-importers-for-the-monogame-pipeline

    [ContentImporter(".pe", DefaultProcessor = "ParticleEffectProcessor", DisplayName = "Particle Effect Importer - XenoCore")]
    public class ParticleEffectImporter : ContentImporter<String>
    {
        public override String Import(string filename, ContentImporterContext context)
        {
            return File.ReadAllText(filename);
        }
    }

    [ContentProcessor(DisplayName = "Particle Effect Processor - XenoCore")]
    public class ParticleEffectProcessor : ContentProcessor<String, ParticleEffectDescription>
    {
        public override ParticleEffectDescription Process(String input, ContentProcessorContext context)
        {
            return XenoCoreJson.Deserialize<ParticleEffectDescription>(input);
        }
    }

    [ContentTypeWriter]
    public class ParticleEffectWriter : ContentTypeWriter<ParticleEffectDescription>
    {
        protected override void Write(ContentWriter output, ParticleEffectDescription value)
        {
            output.Write(XenoCoreJson.Serialize(value ,false));
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(ParticleEffectDescription).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ParticleEffectReader).AssemblyQualifiedName;
        }
    }
}
