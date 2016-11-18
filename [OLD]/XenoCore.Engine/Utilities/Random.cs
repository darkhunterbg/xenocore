using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Editor;

namespace XenoCore.Engine.Utilities
{
    public class RandomFloat
    {
        public float Max { get; set; }
        public float Min { get; set; }
    
        [BooleanEditor]
        public bool UseRandom { get; set; }

        [EditorIgnore]
        public byte Seed { get; set; }

        public RandomFloat() { }
        public RandomFloat(float val)
        {
            Max = val;
        }
        public RandomFloat(float min, float max, bool useRandom = true)
        {
            Min = min;
            Max = max;
            UseRandom = useRandom;
        }

        public float Get(byte seed)
        {
            if (UseRandom)
                return MathHelper.Lerp(Min, Max, (float)(seed + Seed) / (float)byte.MaxValue);

            return Max;
        }
    }
    public class RandomVector2
    {
        public Vector2 Max { get; set; }
        public Vector2 Min { get; set; }

        [BooleanEditor]
        public bool UseRandom { get; set; }

        [EditorIgnore]
        public byte Seed { get; set; }

        public RandomVector2() { }
        public RandomVector2(Vector2 val)
        {
            Max = val;
        }
        public RandomVector2(Vector2 min, Vector2 max, bool useRandom = true)
        {
            Min = min;
            Max = max;
            UseRandom = useRandom;
        }

        public Vector2 Get(byte seed)
        {
            if (UseRandom)
                return Vector2.Lerp(Min, Max, (float)(seed + Seed) / (float)byte.MaxValue);

            return Max;
        }
    }
    public class RandomColor
    {
        public Color Max { get; set; }
        public Color Min { get; set; }

        [BooleanEditor]
        public bool UseRandom { get; set; }

        [EditorIgnore]
        public byte Seed { get; set; }

        public RandomColor() { }
        public RandomColor(Color val)
        {
            Max = val;
        }
        public RandomColor(Color min, Color max, bool useRandom = true)
        {
            Min = min;
            Max = max;
            UseRandom = useRandom;
        }

        public Color Get(byte seed)
        {
            if (UseRandom)
                return Color.Lerp(Min, Max, (float)(seed +Seed) / (float)byte.MaxValue);

            return Max;
        }
    }


}
