using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace QG.Managers.SaveSystem.Basic.Prefs
{

    [AddComponentMenu("QG/Managers/Player Prefs Manager")]
    /// <summary>
    /// Player Prefs Implementation of BasicSaveLoadManager
    /// It unity main thread awaitable to make sure the data is saved and loaded on the main thread
    /// </summary>
    public sealed class PlayerPrefsManager : BasicSaveLoadManager
    {
        public override bool IsReady() => true;

        #region SetMethods
        public override void SetValue(string key, int value) => PlayerPrefs.SetInt(key, value);
        public override void SetValue(string key, string value) => PlayerPrefs.SetString(key, value);
        public override void SetValue(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public override void SetValue<T>(string key, T value, EnumSaveSetting saveSetting = EnumSaveSetting.AsString)
        {
            if (saveSetting == EnumSaveSetting.AsString) SetValue(key, value.ToString());
            else SetValue(key, Convert.ToInt32(value));
        }
        public override void SetValue(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
        public override void SetValue(string key, BigInteger value) => SetValue(key, value.ToString() as string);
        public override void ClearAllData() => PlayerPrefs.DeleteAll();
        public override void ClearData(string key) => PlayerPrefs.DeleteKey(key);
        #endregion

        #region GetMethods
        public override int GetValue(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        public override string GetValue(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        public override float GetValue(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        public override T GetValue<T>(string key, T defaultValue, EnumSaveSetting saveType = EnumSaveSetting.AsString)
        {
            if (saveType == EnumSaveSetting.AsInt) return (T)Enum.ToObject(typeof(T), GetValue(key, Convert.ToInt32(defaultValue)));
            return (T)Enum.Parse(typeof(T), GetValue(key, defaultValue.ToString()));
        }
        public override BigInteger GetValue(string key, BigInteger defaultValue = default)
        {
            return BigInteger.Parse(PlayerPrefs.GetString(key, defaultValue.ToString()));
        }
        public override bool GetValue(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }
        public override void SaveClusterImediate(object caller)
        {
            foreach (var key in BufferClusters[caller].Keys)
            {
                if (!Delta.Contains(key)) continue;
                var type = BufferClusters[caller][key].GetType();
                if (BufferClusters[caller][key] is int intValue) SetValue(key, intValue);
                else if (BufferClusters[caller][key] is float floatValue) SetValue(key, floatValue);
                else if (BufferClusters[caller][key] is string stringValue) SetValue(key, stringValue);
                else if (BufferClusters[caller][key] is System.Numerics.BigInteger big) SetValue(key, big.ToString() as string);
                else if (BufferClusters[caller][key] is bool boolValue) SetValue(key, boolValue);
                else SetValue(key, BufferClusters[caller][key].ToString());
                Delta.Remove(key);
            }
        }
        #endregion
    }
}