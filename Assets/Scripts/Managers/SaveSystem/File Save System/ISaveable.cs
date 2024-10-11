using System.Collections.Generic;
using UnityEngine;

namespace QuarterAsset.SaveSystem.FileSaveSystem
{
    public interface ISaveable<T>
    {
        public static List<T> Instances;
        public void OnCreate(T created)
        {
            if (Instances == null) Instances = new List<T>();
            Instances.Add(created);
        }
        public void OnDestroy(T destroyed)
        {
            Instances.Remove(destroyed);
        }
    }
}
