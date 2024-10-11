using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace QuarterAsset.SaveSystem.FileSaveSystem
{

    abstract public class SaveLoadVisitor<T> : SaveLoadVisitor where T : ISaveable<T>
    {
        public override Type GetT() => typeof(T);
        abstract public Dictionary<string, object> Save(T saveable);
        public override Dictionary<string, object> Save()
        {
            Dictionary<string, object> objectData = new Dictionary<string, object>();
            var instances = GetAllInstances<T>(this);
            foreach (T saveable in instances)
            {
                var instanceData = Save(saveable);
                objectData.Add(instances.IndexOf(saveable).ToString(), instanceData);
            }
            return objectData;
        }

        abstract public void Load(T saveable, Dictionary<string, object> data);
        public override void Load(Dictionary<string, object> data)
        {
            var instances = GetAllInstances<T>(this);
            foreach (T saveable in instances)
            {
                Load(saveable, data[instances.IndexOf(saveable).ToString()] as Dictionary<string, object>);
            }
        }
    }
    abstract public class SaveLoadVisitor : ScriptableObject
    {
        public abstract Type GetT();
        public virtual List<T> GetAllInstances<T>(SaveLoadVisitor visitor) where T : ISaveable<T>
        {
            return ISaveable<T>.Instances;
        }
        abstract public Dictionary<string, object> Save();
        abstract public void Load(Dictionary<string, object> data);
    }
}