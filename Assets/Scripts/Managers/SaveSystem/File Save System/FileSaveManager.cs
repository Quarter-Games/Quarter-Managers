using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuarterAsset.SaveSystem.FileSaveSystem
{
    abstract public class FileSaveManager : SingletonManager<FileSaveManager>
    {
        protected string PERSISTANT_DATA_PATH;
        protected string DIVIDER;
        protected const string SAVE_FOLDER = "Saves";
        public VisitorList TypesToSave;
        internal override void Init()
        {
            PERSISTANT_DATA_PATH = Application.persistentDataPath;
            DIVIDER = System.IO.Path.DirectorySeparatorChar.ToString();
            base.Init();
        }
        #region Static Methods
        static public void Save(string name)
        {
            if (Instance == null) return;
            Dictionary<string, object> saveData = new Dictionary<string, object>();
            foreach (SaveLoadVisitor visitor in Instance.TypesToSave.Visitors)
            {
                saveData.Add(visitor.GetT().FullName, visitor.Save());
            }
            Instance.Save(saveData, name);

        }
        static public void Load(string fileName)
        {
            if (Instance == null) return;
            Dictionary<string, object> saveData = Instance.LoadFile(fileName);
            foreach (SaveLoadVisitor visitor in Instance.TypesToSave.Visitors)
            {
                if (saveData.TryGetValue(visitor.GetT().FullName, out var value))
                {
                    visitor.Load(value as Dictionary<string, object>);
                }
            }

        }
        #endregion
        #region Abstract methods
        abstract public void Save(Dictionary<string, object> saveData, string name);
        abstract public Dictionary<string, DateTime> AvaliableSaves();
        abstract public Dictionary<string, object> LoadFile(string name);
        #endregion
    }
}