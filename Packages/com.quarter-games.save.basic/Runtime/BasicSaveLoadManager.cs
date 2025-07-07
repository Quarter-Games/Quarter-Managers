using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace QG.Managers.SaveSystem.Basic
{
    /// <summary>
    /// Manager base class for saving simple data as player name,id or setting options
    /// Has static DataBuffer to keep data where there is no instance of the manager
    /// </summary>
    abstract public class BasicSaveLoadManager : SingletonManager<BasicSaveLoadManager>
    {
        protected static Dictionary<object, Dictionary<string, object>> BufferClusters = new();
        protected static List<string> Delta = new();
        protected static Dictionary<string, object> DataBuffer = new();

        #region Static Methods

        public static void CreateClusterForCaller(object caller)
        {

            if (Instance == null) LoadFallBackManager();
            if (caller == null) caller = Instance;
            if (!BufferClusters.ContainsKey(caller))
            {
                BufferClusters.Add(caller, new Dictionary<string, object>());
            }
        }
        public static void SaveAllClusters()
        {
            foreach (var caller in BufferClusters.Keys)
            {
                SaveCluster(caller);
            }
        }
        public static void SaveCluster(object caller)
        {
            CreateClusterForCaller(caller);
            if (caller == null) caller = Instance;
            if (Instance == null) return;
            Instance.SaveClusterImediate(caller);
        }
        #region GetMethods
        public static int GetData(string key, int defaultValue = 0, object caller = null)
        {
            CreateClusterForCaller(caller);

            if (caller == null) caller = Instance;
            if (BufferClusters[caller].ContainsKey(key))
            {
                return (int)BufferClusters[caller][key];
            }
            if (Instance == null)
            {
                BufferClusters[caller].Add(key, defaultValue);
                return defaultValue;
            }
            var value = Instance.GetValue(key, defaultValue);
            BufferClusters[caller].Add(key, value);
            return value;
        }
        public static string GetData(string key, string defaultValue = "", object caller = null)
        {
            CreateClusterForCaller(caller);

            if (caller == null) caller = Instance;
            if (BufferClusters[caller].ContainsKey(key))
            {
                return (string)BufferClusters[caller][key];
            }
            if (Instance == null)
            {
                BufferClusters[caller].Add(key, defaultValue);
                return defaultValue;
            }
            var value = Instance.GetValue(key, defaultValue);
            BufferClusters[caller].Add(key, value);
            return value;
        }
        public static float GetData(string key, float defaultValue = 0f, object caller = null)
        {
            CreateClusterForCaller(caller);

            if (caller == null) caller = Instance;
            if (BufferClusters[caller].ContainsKey(key))
            {
                return (float)BufferClusters[caller][key];
            }
            if (Instance == null)
            {
                BufferClusters[caller].Add(key, defaultValue);
                return defaultValue;
            }
            var value = Instance.GetValue(key, defaultValue);
            BufferClusters[caller].Add(key, value);
            return value;
        }
        public static T GetData<T>(string key, T defaultValue = default, EnumSaveSetting saveType = EnumSaveSetting.AsString, object caller = null) where T : Enum
        {
            CreateClusterForCaller(caller);
            if (caller == null) caller = Instance;
            if (BufferClusters[caller].ContainsKey(key))
            {
                if (saveType == EnumSaveSetting.AsString) return (T)Enum.Parse(typeof(T), BufferClusters[caller][key] as string);
                else return (T)Enum.ToObject(typeof(T), BufferClusters[caller][key]);
            }
            if (Instance == null)
            {
                if (saveType == EnumSaveSetting.AsString) BufferClusters[caller].Add(key, defaultValue.ToString());
                else BufferClusters[caller].Add(key, Convert.ToInt32(defaultValue));
                return defaultValue;
            }
            T value = Instance.GetValue(key, defaultValue, saveType);
            if (saveType == EnumSaveSetting.AsString) BufferClusters[caller].Add(key, value.ToString());
            else BufferClusters[caller].Add(key, Convert.ToInt32(value));
            return value;
        }
        public static BigInteger GetData(string key, BigInteger defaultValue = default, object caller = null)
        {
            CreateClusterForCaller(caller);

            if (caller == null) caller = Instance;
            if (BufferClusters[caller].ContainsKey(key))
            {
                return (BigInteger)BufferClusters[caller][key];
            }
            if (Instance == null)
            {
                BufferClusters[caller].Add(key, defaultValue);
                return defaultValue;
            }
            var value = Instance.GetValue(key, defaultValue);
            BufferClusters[caller].Add(key, value);
            return value;
        }
        public static bool GetData(string key, bool defaultValue = false, object caller = null)
        {
            CreateClusterForCaller(caller);

            if (caller == null) caller = Instance;
            if (BufferClusters[caller].ContainsKey(key))
            {
                return (bool)BufferClusters[caller][key];
            }
            if (Instance == null)
            {
                BufferClusters[caller].Add(key, defaultValue);
                return defaultValue;
            }
            var value = Instance.GetValue(key, defaultValue);
            BufferClusters[caller].Add(key, value);
            return value;
        }
        #endregion

        public static void DeltaCheck(string key, object value, object caller)
        {
            bool delta;
            if (BufferClusters[caller].TryGetValue(key, out var oldValue))
            {
                if (oldValue.Equals(value)) delta = false;
                else delta = true;
            }
            else delta = true;
            if (delta) Delta.Add(key);
        }
        #region SetMethods
        public static void SetData(string key, int value, object caller = null, bool saveImediate = false)
        {
            CreateClusterForCaller(caller);
            if (caller == null) caller = Instance;
            DeltaCheck(key, value, caller);
            BufferClusters[caller][key] = value;
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void SetData(string key, string value, object caller, bool saveImediate = false)
        {
            CreateClusterForCaller(caller);
            if (caller == null) caller = Instance;
            DeltaCheck(key, value, caller);
            BufferClusters[caller][key] = value;
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void SetData(string key, float value, object caller = null, bool saveImediate = false)
        {
            CreateClusterForCaller(caller);
            if (caller == null) caller = Instance;
            DeltaCheck(key, value, caller);
            BufferClusters[caller][key] = value;
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void SetData<T>(string key, T value, EnumSaveSetting saveType = EnumSaveSetting.AsString, object caller = null, bool saveImediate = false) where T : Enum
        {
            CreateClusterForCaller(caller);
            if (caller == null) caller = Instance;
            DeltaCheck(key, value, caller);
            if (saveType == EnumSaveSetting.AsString) BufferClusters[caller][key] = value.ToString();
            else BufferClusters[caller][key] = Convert.ToInt32(value);
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void SetData(string key, BigInteger value, object caller = null, bool saveImediate = false)
        {
            CreateClusterForCaller(caller);
            if (caller == null) caller = Instance;
            DeltaCheck(key, value, caller);
            BufferClusters[caller][key] = value;
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void SetData(string key, bool value, object caller = null, bool saveImediate = false)
        {
            CreateClusterForCaller(caller);
            if (caller == null) caller = Instance;
            DeltaCheck(key, value, caller);
            BufferClusters[caller][key] = value;
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void ClearData()
        {
            BufferClusters.Clear();
            Delta.Clear();
            if (Instance == null) return;
            Instance.ClearAllData();
        }
        public static void ClearCluster(object caller)
        {
            foreach (var key in BufferClusters[caller].Keys)
            {
                ClearKey(key, caller);
            }
            Delta.RemoveAll(x => BufferClusters[caller].ContainsKey(x));
            BufferClusters.Remove(caller);
        }
        public static void ClearKey(string key, object caller)
        {
            if (caller == null) caller = Instance;
            Delta.Remove(key);
            BufferClusters[caller].Remove(key);
            if (Instance == null) return;

            Instance.ClearData(caller.ToString() + key);
        }
        #endregion

        #endregion

        #region Abstract Methods

        #region GetMethods
        public abstract int GetValue(string key, int defaultValue = 0);
        public abstract string GetValue(string key, string defaultValue = "");
        public abstract float GetValue(string key, float defaultValue = 0f);
        public abstract TEnum GetValue<TEnum>(string key, TEnum defaultValue = default, EnumSaveSetting saveType = EnumSaveSetting.AsString) where TEnum : Enum;
        public abstract BigInteger GetValue(string key, BigInteger defaultValue = default);
        public abstract bool GetValue(string key, bool defaultValue = false);
        #endregion

        #region SetMethods
        public abstract void SetValue(string key, int value);
        public abstract void SetValue(string key, float value);
        public abstract void SetValue(string key, string value);
        public abstract void SetValue<T>(string key, T value, EnumSaveSetting saveSetting = EnumSaveSetting.AsString) where T : Enum;
        public abstract void SetValue(string key, BigInteger value);
        public abstract void SetValue(string key, bool value);
        public abstract void SaveClusterImediate(object caller);
        public abstract void ClearAllData();
        public abstract void ClearData(string key);
        #endregion

        #endregion

        public enum EnumSaveSetting
        {
            AsInt,
            AsString
        }

    }
}