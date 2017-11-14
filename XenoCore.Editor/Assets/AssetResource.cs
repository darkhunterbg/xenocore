using MonoGame.Tools.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XenoCore.Editor.Assets
{
    public enum AssetEntryType
    {
        Unknown,
        Texture,
        SpriteFont,
        Effect,
    }


    public class AssetResource : AssetEntry
    {
        internal ContentItem PipelineItem { get; private set; }


        public String RelaitvePath { get; private set; }
        public String Location
        {
            get; private set;
        }
        public AssetEntryType Type { get; private set; }
        public ImageSource Image { get; private set; }

        public AssetProject Project { get; private set; }

        public AssetResource(ContentItem item, AssetParent parent, AssetProject project) 
        {
            this.Project = project;
            base.Parent = parent;
            this.PipelineItem = item;
            RelaitvePath = item.OriginalPath.NormalizePath();
            Location = item.Location.Normalize();
            Name = Path.GetFileName(RelaitvePath);

            Type = GetTypeFromProcessor(item.ProcessorName);

            switch (Type)
            {
                //TODO: use some kind of cache policy for images
                case AssetEntryType.Effect:
                    Image = App.Current.Resources["EffectIcon"] as BitmapImage;
                    break;
                case AssetEntryType.SpriteFont:
                    Image = App.Current.Resources["FontIcon"] as BitmapImage;
                    break;
                default:
                    Image = IconManager.FindIconForFilename(RelaitvePath, false);
                    break;
            }
        }

        public static AssetEntryType GetTypeFromProcessor(String processor)
        {
            switch (processor)
            {
                case "EffectProcessor": return AssetEntryType.Effect;
                case "TextureProcessor": return AssetEntryType.Texture;
                case "FontDescriptionProcessor": return AssetEntryType.SpriteFont;
            }
            return AssetEntryType.Unknown;
        }

   
    }
}
