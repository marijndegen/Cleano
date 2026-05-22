using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Cleano.Models;

namespace Cleano.Services
{
    public class DataService
    {
        private readonly string _dataPath;

        public DataService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "Clearo");
            Directory.CreateDirectory(appFolder);
            _dataPath = Path.Combine(appFolder, "taskdata.json");
        }

        public List<TaskGroup> LoadData()
        {
            if (!File.Exists(_dataPath))
            {
                return new List<TaskGroup>();
            }

            try
            {
                var json = File.ReadAllText(_dataPath);
                return JsonSerializer.Deserialize<List<TaskGroup>>(json) ?? new List<TaskGroup>();
            }
            catch
            {
                return new List<TaskGroup>();
            }
        }

        public void SaveData(List<TaskGroup> taskGroups)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(taskGroups, options);
            File.WriteAllText(_dataPath, json);
        }
    }
}
