using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
namespace QG.Managers.SaveSystem.File.JSON
{
    [AddComponentMenu("QG/Managers/JSON Save Manager")]
    sealed internal class LocalJSONSaveManager : FileSaveManager
    {
        public const string FILE_EXTENSION = ".json";
        public override Dictionary<string, DateTime> AvaliableSaves()
        {
            System.IO.DirectoryInfo directory = new(PERSISTANT_DATA_PATH + DIVIDER + SAVE_FOLDER);
            if (!directory.Exists) return new Dictionary<string, DateTime>();
            System.IO.FileInfo[] files = directory.GetFiles("*.json");
            if (files.Length == 0) return new Dictionary<string, DateTime>();
            Dictionary<string, DateTime> saves = new();
            foreach (System.IO.FileInfo file in files)
            {
                saves.Add(file.Name, file.CreationTime);
            }
            return saves;
        }

        public override bool IsReady() => true;
        public override Dictionary<string, object> LoadFile(string name)
        {
            System.IO.DirectoryInfo directory = new(PERSISTANT_DATA_PATH + DIVIDER + SAVE_FOLDER);
            if (!directory.Exists) throw new System.IO.FileNotFoundException("Save folder not found");
            string json = System.IO.File.ReadAllText(directory.FullName + DIVIDER + name + FILE_EXTENSION);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public override void Save(Dictionary<string, object> saveData, string name)
        {
            System.IO.DirectoryInfo directory = new(PERSISTANT_DATA_PATH + DIVIDER + SAVE_FOLDER);
            if (!directory.Exists) directory.Create();
            string json = JsonConvert.SerializeObject(saveData);
            System.IO.File.WriteAllText(directory.FullName + DIVIDER + name + FILE_EXTENSION, json);
        }
    }
}