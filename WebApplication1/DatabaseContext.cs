using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

namespace WebApplication1
{
    public class DatabaseContext
    {
        private static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "calendarData.json");

        public static Dictionary<string, Dictionary<string, Dictionary<int, CalendarTask>>> LoadData()
        {
            // If the file doesn't exist, create an empty file
            if (!File.Exists(filePath))
            {
                // Create an empty file with an empty dictionary serialized as JSON
                SaveData(new Dictionary<string, Dictionary<string, Dictionary<int, CalendarTask>>>());
            }

            // Read data from the file
            var jsonData = File.ReadAllText(filePath);

            // Deserialize and return the data
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<int, CalendarTask>>>>(jsonData)
                       ?? new Dictionary<string, Dictionary<string, Dictionary<int, CalendarTask>>>();
            }
            catch (Exception)
            {
                return new Dictionary<string, Dictionary<string, Dictionary<int, CalendarTask>>>(); // Return empty if parsing fails
            }
        }

        public static void SaveData(Dictionary<string, Dictionary<string, Dictionary<int, CalendarTask>>> data)
        {
            // Serialize the data to JSON
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

            // Save the JSON data to the file
            File.WriteAllText(filePath, jsonData);
        }
    }
}
