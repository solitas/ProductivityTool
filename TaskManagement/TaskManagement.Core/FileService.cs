using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TaskManagement.Core.Model;

namespace TaskManagement.Core
{
    public static class FileService
    {
        public static void Save(List<UserTask> tasks)
        {
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
           
            Serialize(tasks, "Data/tasks.json");
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

        public static object Deserialize(string path)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamReader(path))
            using (var reader = new JsonTextReader(sw))
            {
                return serializer.Deserialize(reader);
            }
        }
    }
}
