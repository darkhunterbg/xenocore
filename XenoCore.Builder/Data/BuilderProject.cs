using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Builder.Data
{
    public class BuilderProject
    {
        public String ProjectName { get; set; }
        public String OutputContentDir { get; set; }

        public static BuilderProject Load(String file)
        {
            var json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<BuilderProject>(json);
        }

        public void SaveTo(String file)
        {
            if (!File.Exists(file))
                File.Create(file).Close();

            using (FileStream fs = File.Open(file, FileMode.Truncate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    using (JsonWriter jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jw, this);
                    }
                }
            }
        }
    }
}
