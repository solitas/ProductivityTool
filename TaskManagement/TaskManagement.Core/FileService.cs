using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using TaskManagement.Core.Model;

namespace TaskManagement.Core
{
    public static class FileService
    {
        private static string __FILE_PATH__ = "Data/tasks.xml";
        public static void Save(List<UserTask> tasks)
        {
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
           
            //Serialize(tasks, "Data/tasks.json");
            SerializeToXml(tasks, __FILE_PATH__);
        }

        public static List<UserTask> Load()
        {
            if (File.Exists(__FILE_PATH__))
            {
                //return Deserialize(__FILE_PATH__) as List<UserTask>;
                return DeserializeFromXml(__FILE_PATH__) as List<UserTask>;
            }

            throw new FileNotFoundException();
        }

        public static void Serialize(object obj, string filePath)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public static bool SerializeToXml(object obj, string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(List<UserTask>));
                    ser.Serialize(fileStream, obj);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static object Deserialize(string path)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamReader(path))
            using (var reader = new JsonTextReader(sw))
            {
                return serializer.Deserialize(reader);
            }
        }

        public static object DeserializeFromXml(string path)
        {
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    var ser = new XmlSerializer(typeof(List<UserTask>));
                    return ser.Deserialize(fileStream);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
