using System;
using System.Collections.Generic;
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
        protected static Dictionary<string, object> DataBuffer = new();

        #region Static Methods

        public static void CreateClusterForCaller(object caller)
        {
            if (!BufferClusters.ContainsKey(caller))
            {
                BufferClusters.Add(caller, new Dictionary<string, object>());
            }
        }
        public static void SaveCluster(object caller)
        {
            CreateClusterForCaller(caller);
            if (Instance == null) return;
            Instance.SaveClusterImediate(caller);
        }
        #region GetMethods
        async public static Task<int> GetData(string key, int defaultValue = 0, object caller = null)
        {
            CreateClusterForCaller(caller);

            if (BufferClusters[caller].ContainsKey(key))
            {
                return (int)BufferClusters[caller][key];
            }
            if (Instance == null)
            {
                BufferClusters[caller].Add(key, defaultValue);
                return defaultValue;
            }
            var value = await Instance.GetValue(key, defaultValue);
            BufferClusters[caller].Add(key, value);
            return value;
        }
        async public static Task<string> GetData(string key, string defaultValue = "", object caller = null)
        {
            CreateClusterForCaller(caller);

            if (BufferClusters[caller].ContainsKey(key))
            {
                return (string)BufferClusters[caller][key];
            }
            if (Instance == null)
            {
                BufferClusters[caller].Add(key, defaultValue);
                return defaultValue;
            }
            var value = await Instance.GetValue(key, defaultValue);
            BufferClusters[caller].Add(key, value);
            return value;
        }
        async public static Task<float> GetData(string key, float defaultValue = 0f, object caller = null)
        {
            CreateClusterForCaller(caller);

            if (BufferClusters[caller].ContainsKey(key))
            {
                return (float)BufferClusters[caller][key];
            }
            if (Instance == null)
            {
                BufferClusters[caller].Add(key, defaultValue);
                return defaultValue;
            }
            var value = await Instance.GetValue(key, defaultValue);
            BufferClusters[caller].Add(key, value);
            return value;
        }
        async public static Task<T> GetData<T>(string key, T defaultValue = default, EnumSaveSetting saveType = EnumSaveSetting.AsString, object caller = null) where T : Enum
        {
            CreateClusterForCaller(caller);
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
            T value = await Instance.GetValue(key, defaultValue, saveType);
            if (saveType == EnumSaveSetting.AsString) BufferClusters[caller].Add(key, value.ToString());
            else BufferClusters[caller].Add(key, Convert.ToInt32(value));
            return value;
        }
        #endregion

        #region SetMethods
        public static void SetData(string key, int value, object caller = null, bool saveImediate = false)
        {
            CreateClusterForCaller(caller);
            BufferClusters[caller][key] = value;
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void SetData(string key, string value, object caller, bool saveImediate = false)
        {
            CreateClusterForCaller(caller);
            BufferClusters[caller][key] = value;
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void SetData(string key, float value, object caller = null, bool saveImediate = false)
        {
            CreateClusterForCaller(caller);
            BufferClusters[caller][key] = value;
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void SetData<T>(string key, T value, EnumSaveSetting saveType = EnumSaveSetting.AsString, object caller = null, bool saveImediate = false) where T : Enum
        {
            CreateClusterForCaller(caller);
            if (saveType == EnumSaveSetting.AsString) BufferClusters[caller][key] = value.ToString();
            else BufferClusters[caller][key] = Convert.ToInt32(value);
            if (Instance == null) return;
            if (saveImediate) Instance.SaveClusterImediate(caller);
        }
        public static void ClearData()
        {
            BufferClusters.Clear();
            if (Instance == null) return;
            Instance.ClearAllData();
        }
        public static void ClearCluster(object caller)
        {
            foreach (var key in BufferClusters[caller].Keys)
            {
                ClearKey(key, caller);
            }
            BufferClusters.Remove(caller);
        }
        public static void ClearKey(string key, object caller)
        {
            BufferClusters[caller].Remove(key);
            if (Instance == null) return;

            Instance.ClearData(caller.ToString() + key);
        }
        #endregion

        #endregion

        #region Abstract Methods

        #region GetMethods
        protected abstract Task<int> GetValue(string key, int defaultValue = 0);
        protected abstract Task<string> GetValue(string key, string defaultValue = "");
        protected abstract Task<float> GetValue(string key, float defaultValue = 0f);
        protected abstract Task<TEnum> GetValue<TEnum>(string key, TEnum defaultValue = default, EnumSaveSetting saveType = EnumSaveSetting.AsString) where TEnum : Enum;
        #endregion

        #region SetMethods
        protected abstract void SetValue(string key, int value);
        protected abstract void SetValue(string key, float value);
        protected abstract void SetValue(string key, string value);
        protected abstract void SetValue<T>(string key, T value, EnumSaveSetting saveSetting = EnumSaveSetting.AsString) where T : Enum;
        protected abstract void SaveClusterImediate(object caller);
        protected abstract void ClearAllData();
        protected abstract void ClearData(string key);
        #endregion

        #endregion

        public enum EnumSaveSetting
        {
            AsInt,
            AsString
        }

    }
}