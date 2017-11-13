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
    public class AssetResource : AssetEntry
    {
        private ContentItem pipelineItem;

        private String _relativePath;

        public String RelaitvePath
        {
            get { return _relativePath; }

            private set { _relativePath = value; }
        }
        public String Location
        {
            get;private set;
        }



        public ImageSource Image { get; private set; }

        public AssetResource(ContentItem item)
            : base(GetTypeFromProcessor(item.ProcessorName))
        {
            this.pipelineItem = item;
            RelaitvePath = item.OriginalPath.Normalize();
            Location = item.Location.Normalize();
            Name = Path.GetFileName(RelaitvePath);

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
