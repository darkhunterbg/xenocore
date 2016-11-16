using Microsoft.Xna.Framework.Graphics;
using MonoGame.Tools.Pipeline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Builder.Data;
using XenoCore.ContentPipeline.Content;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Particles;
using XenoCore.Engine.Resources;

namespace XenoCore.Builder.MVVM
{

    public abstract class ResourceModel : BaseModel
    {
        private bool selected;
        private bool expanded;
        private bool editing;
        private String resouceInfo;
        private String editingText;
        private String name;
        private String contentPath;

        public Resource Item { get; protected set; }

        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged("Selected");
            }
        }
        public bool Expanded
        {
            get { return expanded; }
            set
            {
                expanded = value;
                OnPropertyChanged("Expanded");
            }

        }
        public bool Editing
        {
            get { return editing; }
            set
            {
                editing = value;
                if (editing)
                    EditingText = Name;
                OnPropertyChanged("Editing");
            }

        }
        public String EditingText
        {
            get { return editingText; }
            set
            {
                editingText = value;
                OnPropertyChanged("EditingText");
            }

        }
        public String ContentPath
        {
            get { return contentPath; }
            set
            {
                contentPath = value;
                OnPropertyChanged("ContentPath");
            }

        }
        public String Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public String ResourceInfo
        {
            get { return resouceInfo; }
            set
            {
                resouceInfo = value;
                OnPropertyChanged("ResourceInfo");
            }
        }

        public ObservableCollection<ResourceModel> Items { get; private set; } = new ObservableCollection<ResourceModel>();

        public abstract void OnModified();
    }


    public class ResourceDirModel : ResourceModel
    {
        private static NewResourceModel[] newMenuOptions = new NewResourceModel[]
      {
               new NewResourceModel(ResourceType.Directory),
               new NewResourceModel(ResourceType.SpriteFont ),
               new NewResourceModel(ResourceType.ParticleEffect),

      };

        public IEnumerable<NewResourceModel> NewOptions { get; set; } = newMenuOptions;

        public ResourceDir Directory { get; private set; }

        public ResourceDirModel(ResourceDir dir)
        {
            Item = this.Directory = dir;
            OnModified();
        }

        public override void OnModified()
        {
            ResourceInfo = $"Path: {Directory.XnbPath}";
            Name = Directory.Name;
            ContentPath = Directory.ContentPath;
        }
    }

    public class ResourceObjModel : ResourceModel
    {
        public ResourceObj Resource { get; private set; }

        public ResourceObjModel(ResourceObj resource)
        {
            Item = this.Resource = resource;
            OnModified();
        }

        public override void OnModified()
        {
            Name = Resource.Name;
            ContentPath = Resource.ContentPath;

            switch (Resource.Type)
            {

                case ResourceType.SpriteFont:
                    {
                        var r = (Resource.Instance as SpriteFontDescription).ContentFont;
                        var size = r.MeasureString(" ");
                        ResourceInfo = $"Size: {r.Texture.Width}x{r.Texture.Height}, GlyphMaxSize: {size.X}x{size.Y}";
                    }
                    break;
                case ResourceType.Texture:
                    {
                        var t = Resource.Instance as Texture2D;
                        ResourceInfo = $"Size:{ t.Width}x{t.Height}";
                    }
                    break;
                case ResourceType.ParticleEffect:
                    {
                        var e = Resource.Instance as ParticleEffectDescription;
                        ResourceInfo = $"Name: {e.Name}, Emitters: {e.Emitters.Count}";
                    }
                    break;
            }
        }
    }
}
