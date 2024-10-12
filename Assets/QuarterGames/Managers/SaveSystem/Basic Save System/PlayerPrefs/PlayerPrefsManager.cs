using System;
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
        protected override void ClearAllData() => PlayerPrefs.DeleteAll();
        protected override void ClearData(string key) => PlayerPrefs.DeleteKey(key);
        #endregion

        #region GetMethods
        async protected override Task<int> GetValue(string key, int defaultValue = 0)
        {
            var task = new Task<int>(() => PlayerPrefs.GetInt(key, defaultValue));
            task.RunSynchronously(TaskScheduler.Current);
            return await task.ConfigureAwait(false);
        }
        async protected override Task<string> GetValue(string key, string defaultValue = "")
        {
            var task = new Task<string>(() => PlayerPrefs.GetString(key, defaultValue));
            task.RunSynchronously(TaskScheduler.Current);
            return await task.ConfigureAwait(false);
        }
        async protected override Task<float> GetValue(string key, float defaultValue = 0f)
        {
            var task = new Task<float>(() => PlayerPrefs.GetFloat(key, defaultValue));
            task.RunSynchronously(TaskScheduler.Current);
            return await task.ConfigureAwait(false);
        }
        async protected override Task<T> GetValue<T>(string key, T defaultValue, EnumSaveSetting saveType = EnumSaveSetting.AsString)
        {
            if (saveType == EnumSaveSetting.AsInt) return (T)Enum.ToObject(typeof(T), await GetValue(key, Convert.ToInt32(defaultValue)));
            return (T)Enum.Parse(typeof(T), await GetValue(key, defaultValue.ToString()));
        }
        #endregion
    }
}