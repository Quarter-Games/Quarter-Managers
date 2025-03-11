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
    sealed internal class PlayerPrefsManager : BasicSaveLoadManager
    {
        public override bool IsReady() => true;

        #region SetMethods
        protected override void SetValue(string key, int value) => PlayerPrefs.SetInt(key, value);
        protected override void SetValue(string key, string value) => PlayerPrefs.SetString(key, value);
        protected override void SetValue(string key, float value) => PlayerPrefs.SetFloat(key, value);
        protected override void SetValue<T>(string key, T value, EnumSaveSetting saveSetting = EnumSaveSetting.AsString)
        {
            if (saveSetting == EnumSaveSetting.AsString) SetValue(key, value.ToString());
            else SetValue(key, Convert.ToInt32(value));
        }
        protected override void SetValue(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
        protected override void SetValue(string key, BigInteger value) => SetValue(key, value.ToString() as string);
        protected override void ClearAllData() => PlayerPrefs.DeleteAll();
        protected override void ClearData(string key) => PlayerPrefs.DeleteKey(key);
        #endregion

        #region GetMethods
        protected override int GetValue(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        protected override string GetValue(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        protected override float GetValue(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        protected override T GetValue<T>(string key, T defaultValue, EnumSaveSetting saveType = EnumSaveSetting.AsString)
        {
            if (saveType == EnumSaveSetting.AsInt) return (T)Enum.ToObject(typeof(T), GetValue(key, Convert.ToInt32(defaultValue)));
            return (T)Enum.Parse(typeof(T), GetValue(key, defaultValue.ToString()));
        }
        protected override BigInteger GetValue(string key, BigInteger defaultValue = default)
        {
            return BigInteger.Parse(PlayerPrefs.GetString(key, defaultValue.ToString()));
        }
        protected override bool GetValue(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }
        protected override void SaveClusterImediate(object caller)
        {
            foreach (var key in BufferClusters[caller].Keys)
            {
                if (!Delta.Contains(key)) continue;
                var type = BufferClusters[caller][key].GetType();
                if (BufferClusters[caller][key] is int intValue) SetValue(key, intValue);
                else if (BufferClusters[caller][key] is float floatValue) SetValue(key, floatValue);
                else if (BufferClusters[caller][key] is string stringValue) SetValue(key, stringValue);
                else if (BufferClusters[caller][key] is System.Numerics.BigInteger big) SetValue(key, big);
                else if (BufferClusters[caller][key] is bool boolValue) SetValue(key, boolValue);
                else SetValue(key, BufferClusters[caller][key].ToString());
                Delta.Remove(key);
            }
        }
        #endregion
    }
}