using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Manager base class for saving simple data as player name,id or setting options
/// Has static Buffer to keep data where there is no instance of the manager
/// </summary>
abstract public class BasicSaveLoadManager : SingletonManager<BasicSaveLoadManager>
{
    protected static Dictionary<string, object> Buffer = new();

    #region Static Methods

    #region GetMethods
    async public static Task<int> GetData(string key, int defaultValue = 0)
    {
        if (Buffer.ContainsKey(key))
        {
            return (int)Buffer[key];
        }
        if (Instance == null)
        {
            Buffer.Add(key, defaultValue);
            return defaultValue;
        }
        var value = await Instance.GetValue(key, defaultValue);
        Buffer.Add(key, value);
        return value;
    }
    async public static Task<string> GetData(string key, string defaultValue = "")
    {
        if (Buffer.ContainsKey(key))
        {
            return (string)Buffer[key];
        }
        if (Instance == null)
        {
            Buffer.Add(key, defaultValue);
            return defaultValue;
        }
        var value = await Instance.GetValue(key, defaultValue);
        Buffer.Add(key, value);
        return value;
    }
    async public static Task<float> GetData(string key, float defaultValue = 0f)
    {
        if (Buffer.ContainsKey(key))
        {
            return (float)Buffer[key];
        }
        if (Instance == null)
        {
            Buffer.Add(key, defaultValue);
            return defaultValue;
        }
        var value = await Instance.GetValue(key, defaultValue);
        Buffer.Add(key, value);
        return value;
    }
    async public static Task<T> GetData<T>(string key, T defaultValue, EnumSaveSetting saveType = EnumSaveSetting.AsString) where T : Enum
    {
        if (Buffer.ContainsKey(key))
        {
            return (T)Buffer[key];
        }
        if (Instance == null)
        {
            if (saveType == EnumSaveSetting.AsString) Buffer.Add(key, defaultValue.ToString());
            else Buffer.Add(key, Convert.ToInt32(defaultValue));
            return defaultValue;
        }
        T value = await Instance.GetValue(key, defaultValue, saveType);
        if (saveType == EnumSaveSetting.AsString) Buffer.Add(key, value.ToString());
        else Buffer.Add(key, Convert.ToInt32(value));
        return value;
    }
    #endregion

    #region SetMethods
    public static void SetData(string key, int value)
    {
        Buffer[key] = value;
        if (Instance == null) return;
        Instance.SetValue(key, value);
    }
    public static void SetData(string key, string value)
    {
        Buffer[key] = value;
        if (Instance == null) return;
        Instance.SetValue(key, value);
    }
    public static void SetData(string key, float value)
    {
        Buffer[key] = value;
        if (Instance == null) return;
        Instance.SetValue(key, value);
    }
    public static void SetData<T>(string key, T value, EnumSaveSetting saveType = EnumSaveSetting.AsString) where T : Enum
    {
        if (saveType == EnumSaveSetting.AsString) Buffer[key] = value.ToString();
        else Buffer[key] = Convert.ToInt32(value);
        if (Instance == null) return;
        Instance.SetValue(key, value, saveType);
    }
    public static void ClearAll()
    {
        Buffer.Clear();
        if (Instance == null) return;
        Instance.ClearAllData();
    }
    public static void Clear(string key)
    {
        Buffer.Remove(key);
        if (Instance == null) return;
        Instance.ClearData(key);
    }
    #endregion

    #endregion

    #region Abstract Methods

    #region GetMethods
    protected abstract Task<int> GetValue(string key, int defaultValue = 0);
    protected abstract Task<string> GetValue(string key, string defaultValue = "");
    protected abstract Task<float> GetValue(string key, float defaultValue = 0f);
    protected abstract Task<TEnum> GetValue<TEnum>(string key, TEnum defaultValue, EnumSaveSetting saveType = EnumSaveSetting.AsString) where TEnum : Enum;
    #endregion

    #region SetMethods
    protected abstract void SetValue(string key, int value);
    protected abstract void SetValue(string key, float value);
    protected abstract void SetValue(string key, string value);
    protected abstract void SetValue<T>(string key, T value, EnumSaveSetting saveSetting = EnumSaveSetting.AsString) where T : Enum;
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
