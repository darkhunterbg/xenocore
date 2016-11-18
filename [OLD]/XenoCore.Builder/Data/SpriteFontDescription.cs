using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XenoCore.Builder.Data
{
    public class SpriteFontDescription
    {
        public String Name { get; set; }
        public SpriteFont ContentFont { get; set; }

        private String path;

        public String FontName { get; set; }
        public double FontSize { get; set; }
        public double FontSpacing { get; set; }
        public bool IsItalic { get; set; }
        public bool IsBold { get; set; }

        internal SpriteFontDescription() { }
        public SpriteFontDescription(String spritefontPath,String name)
        {
            this.Name = name;
            this.path = spritefontPath;

            XDocument xdoc = XDocument.Load(spritefontPath);

            var nodes = xdoc.Descendants();
            FontName = nodes.First(p => p.Name.LocalName == "FontName").Value;
            FontSize = double.Parse(nodes.First(p => p.Name.LocalName == "Size").Value);
            FontSpacing = double.Parse(nodes.First(p => p.Name.LocalName == "Spacing").Value);
            IsBold = nodes.First(p => p.Name.LocalName == "Style").Value == "Bold";
            IsItalic = nodes.First(p => p.Name.LocalName == "Style").Value == "Italic";
        }

        public void Save()
        {
            XDocument xdoc = XDocument.Load(path);

            var nodes = xdoc.Descendants();
            nodes.First(p => p.Name.LocalName == "FontName").Value = FontName;
            nodes.First(p => p.Name.LocalName == "Size").Value = FontSize.ToString("F0");
            nodes.First(p => p.Name.LocalName == "Spacing").Value = FontSpacing.ToString("F0");
            nodes.First(p => p.Name.LocalName == "Style").Value = IsItalic ? "Italic" : "Regular";
            nodes.First(p => p.Name.LocalName == "Style").Value = IsBold ? "Bold" : "Regular";
            nodes.First(p => p.Name.LocalName == "Start").Value = "32";
            nodes.First(p => p.Name.LocalName == "End").Value = "126";

            xdoc.Save(path);
        }
    }

}
