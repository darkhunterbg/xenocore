using MonoGame.Tools.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Builder.Data
{
    public enum ResourceType
    {
        SpriteFont = 0x1,
        Texture = 0x2,
        ParticleEffect = 0x4,
        Unknown = 0xFF,
        Directory = 0
    }



    public abstract class Resource
    {
        public String Name { get; set; }
        public String XnbPath { get; internal set; }
        public String ContentPath { get; internal set; }
        public String AbsolutePath { get; internal set; }
        public ResourceType Type { get; internal set; }
    }

    public class ResourceDir : Resource
    {

    }

    public class ResourceObj : Resource
    {


        public ContentItem PipelineItem { get; internal set; }
        public  Object Instance { get; internal set; }

        internal ResourceObj() { }
    }
}
