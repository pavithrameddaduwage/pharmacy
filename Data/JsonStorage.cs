using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SmartMedPharmacy.Data
{
    public class JsonStorage<T>
    {
        private readonly string _filePath;

        public JsonStorage(string fileName)
        {
            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(baseDir, fileName);
        }

        public List<T> Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<T>();

                string json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<T>();

                List<T> result = JsonConvert.DeserializeObject<List<T>>(json);
                return result ?? new List<T>();
            }
            catch (IOException)
            {
                return new List<T>();
            }
            catch (JsonException)
            {
                return new List<T>();
            }
        }

        public void Save(List<T> items)
        {
            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
