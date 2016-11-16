using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Resources
{
    public class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            JObject jObject = JObject.Load(reader);

            var obj = new Color();
            obj.A = (byte)(jObject["A"].Value<Int32>());
            obj.B = (byte)(jObject["B"].Value<Int32>());
            obj.G = (byte)(jObject["G"].Value<Int32>());
            obj.R = (byte)(jObject["R"].Value<Int32>());

            return obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (Color)value;
            var jObject = new JObject();
            jObject.Add("A", obj.A);
            jObject.Add("B", obj.B);
            jObject.Add("G", obj.G);
            jObject.Add("R", obj.R);

            serializer.Serialize(writer, jObject);
        }

    }

    public class TypeNameSerializationBinder : ISerializationBinder
    {

        public TypeNameSerializationBinder()
        {
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.FullName;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            string resolvedTypeName = typeName;

            return Type.GetType(resolvedTypeName, true);
        }
    }

    public static class XenoCoreJson
    {
        public static String Serialize(Object obj,bool format = true)
        {
            return JsonConvert.SerializeObject(obj, format ? Formatting.Indented : Formatting.None, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new TypeNameSerializationBinder(),
                Converters = new List<JsonConverter>() { new ColorConverter() }
            });
        }

        public static T Deserialize<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new TypeNameSerializationBinder(),
                Converters = new List<JsonConverter>() { new ColorConverter() }
            });
        }

    }
}
