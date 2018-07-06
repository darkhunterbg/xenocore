using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using XenoCore.Builder.Data;
using XenoCore.Builder.Services;

namespace XenoCore.Builder.MVVM
{
    public class SpriteFontEditorViewModel : EditorResourceModel
    {
        //private String fontName = "Consolas";
        //private double fontSize = 14;
        //private double spacing = 0;
        //private FontWeight fontWeight = FontWeights.Normal;
        //private FontStyle fontStyle = FontStyles.Normal;


        private SpriteFontDescription description = new SpriteFontDescription();

        public String FontName
        {
            get { return description.FontName; }
            set
            {
                description.FontName = value;
                OnPropertyChanged("FontName");
            }
        }
        public double FontSize
        {
            get { return description.FontSize; }
            set
            {
                description.FontSize = value;
                OnPropertyChanged("FontSize");
            }
        }
        public double Spacing
        {
            get { return description.FontSpacing; }
            set
            {
                description.FontSpacing = value;
                OnPropertyChanged("Spacing");
            }
        }
        public FontWeight FontWeight
        {
            get { return description.IsBold ? FontWeights.Bold  : FontWeights.Normal; }
            set
            {
                description.IsBold = (value == FontWeights.Bold );
                OnPropertyChanged("FontWeight");
            }
        }
        public FontStyle FontStyle
        {
            get { return description.IsItalic ? FontStyles.Italic : FontStyles.Normal; }
            set
            {
                description.IsItalic = (value == FontStyles.Italic);
                OnPropertyChanged("FontStyle");
            }
        }

        private ResourceObj model;

        public SpriteFontEditorViewModel() { }

        public SpriteFontEditorViewModel(ResourceObj model)
        {
            this.model = model;
            description = model.Instance as SpriteFontDescription;
        }

        public override string ToString()
        {
            return $"Font Demo";
        }

        public override void SaveResource()
        {
            ResourceManagerService.SaveResource(model);

      
        }
    }
}
