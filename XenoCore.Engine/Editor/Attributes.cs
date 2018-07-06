using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Editor
{
 

    public enum ValueEditor
    {
        Text,
        Slider,
        Vector,
        Color,
        Float,
        Integer,
        Enum,
        List,
        Texture,
        CheckBox,
        Properties
    }

    public class EditorIgnoreAttribute : Attribute
    {
    }
        public class EditorInfoAttribute : Attribute
    {
        public String Format { get; set; } = null;
        public double Min { get; set; } = int.MinValue;
        public double Max { get; set; } = int.MaxValue;
        public double Step { get; set; } = 1.0f;

        public ValueEditor EditorType { get; private set; }
        public EditorInfoAttribute(ValueEditor editorType)
        {
            this.EditorType = editorType;

        }
    }
    public class PropertiesEditor : EditorInfoAttribute
    {
        public PropertiesEditor() : base(ValueEditor.Properties) { }
    }


        public class AlphaEditorAttribute : EditorInfoAttribute
    {
        public AlphaEditorAttribute() : base(ValueEditor.Slider)
        {
            Min = 0;
            Max = 1;
            Step = 0.01;
            Format = "{0:0.00}";
        }
    }

    public class EnumEditorAttribute : EditorInfoAttribute
    {
        public EnumEditorAttribute() : base(ValueEditor.Enum)
        {

        }
    }

    public class BooleanEditorAttribute : EditorInfoAttribute
    {
        public BooleanEditorAttribute() : base(ValueEditor.CheckBox)
        {

        }
    }

    public class TextureEditorAttribute : EditorInfoAttribute
    {
        public TextureEditorAttribute() : base(ValueEditor.Texture)
        {

        }
    }


    public class ColorEditorAttribute : EditorInfoAttribute
    {
        public ColorEditorAttribute() : base(ValueEditor.Color)
        {

        }
    }

    public class DistributionEditorAttribute : EditorInfoAttribute
    {
        public DistributionEditorAttribute() : base(ValueEditor.Slider)
        {
            Min = 0.0;
            Max = 1.0;
            Step = 0.05;
            Format = "{0:P0}";

        }
    }

    public class AngleEditor : EditorInfoAttribute
    {
        public AngleEditor() : base(ValueEditor.Slider)
        {
            Min = -1.0;
            Max = 1.0;
            Step = 0.05;
            Format = "{0:0.00}";

        }
    }


    public class TimeEditorAttribute : EditorInfoAttribute
    {
        public TimeEditorAttribute() : base(ValueEditor.Slider)
        {
            Min = 0.0;
            Max = 10.0;
            Step = 0.1;
            Format = "{0:0.0} sec";
        }
    }

    public enum VectorValueType
    {
        Any,
        Unit,
        UnitAny,
    }

    public class FloatEditorAttribute : EditorInfoAttribute
    {
        public FloatEditorAttribute() : base(ValueEditor.Float)
        {
            Format = "F1";
            Step = 0.1;
        }
    }

    public class IntegerEditorAttribute : EditorInfoAttribute
    {
        public IntegerEditorAttribute() : base(ValueEditor.Integer)
        {
            Step = 0;
        }
    }

    public class VectorEditorAttribute : EditorInfoAttribute
    {
        public VectorEditorAttribute(VectorValueType type = VectorValueType.Any)
            : base(ValueEditor.Vector)
        {
            Format = "F1";

            Step = 1;
            switch (type)
            {
                case VectorValueType.Unit:
                    Min = 0;
                    Max = 1;
                    Step = 0.1;
                    break;
                case VectorValueType.UnitAny:
                    Min = -1;
                    Max = 1;
                    Step = 0.1f;
                    break;
            }
        }
    }


}
