using QG.Managers.SaveSystem.Basic;
using System.Numerics;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using SQLite4Unity3d;

public class FileBasicSaveLoadManager : BasicSaveLoadManager
{
    static SQLiteConnection db;
    Dictionary<string, string> CurrentFile;
    public const string SAVE_FILE_NAME = "save.db";
    public static string FILE_PATH => Application.persistentDataPath + Path.DirectorySeparatorChar + SAVE_FILE_NAME;
    public override void Init()
    {
        base.Init();

        if (!File.Exists(FILE_PATH))
        {
            File.Create(FILE_PATH).Dispose();
        }

        db = new SQLiteConnection(FILE_PATH);
        db.Execute("CREATE TABLE IF NOT EXISTS SaveData (Key TEXT PRIMARY KEY, Value TEXT)");
        db.Commit();
        CurrentFile = LoadFile();
    }


    public override void SaveClusterImediate(object caller)
    {
        if (!db.IsInTransaction) db.BeginTransaction();
        foreach (var key in BufferClusters[caller].Keys)
        {
            if (!Delta.Contains(key)) continue;
            var type = BufferClusters[caller][key].GetType();
            if (BufferClusters[caller][key] is int intValue) SetValue(key, intValue);
            else if (BufferClusters[caller][key] is float floatValue) SetValue(key, floatValue);
            else if (BufferClusters[caller][key] is string stringValue) SetValue(key, stringValue);
            else if (BufferClusters[caller][key] is BigInteger big) SetValue(key, big.ToString());
            else if (BufferClusters[caller][key] is bool boolValue) SetValue(key, boolValue);
            else SetValue(key, BufferClusters[caller][key].ToString());
            db.Execute(
                "INSERT OR REPLACE INTO SaveData (Key, Value) VALUES (?, ?)",
                key,
                CurrentFile[key]
            );
            Delta.Remove(key);
        }
        db.Commit();
    }


    static public Dictionary<string, string> LoadFile()
    {
        var result = new Dictionary<string, string>();
        var query = db.Query<RawRow>("SELECT Key, Value FROM SaveData");

        foreach (var row in query)
            result[row.Key] = row.Value;

        return result;
    }

    public static void SaveFile(Dictionary<string, string> CurrentFile)
    {
        db.BeginTransaction();

        foreach (var pair in CurrentFile)
        {
            db.Execute(
                "INSERT OR REPLACE INTO SaveData (Key, Value) VALUES (?, ?)",
                pair.Key,
                pair.Value
            );
        }

        db.Commit();
    }

    public static byte[] CompressData(byte[] data)
    {
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal))
        {
            gzipStream.Write(data, 0, data.Length);
        }
        return memoryStream.ToArray();
    }

    public static byte[] DecompressData(byte[] compressedData)
    {
        using var memoryStream = new MemoryStream(compressedData);
        using var gzipStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
        gzipStream.CopyTo(decompressedStream);
        return decompressedStream.ToArray();
    }

    public override bool IsReady()
    {
        return CurrentFile != null;
    }

    public override void ClearAllData()
    {
        CurrentFile = new();
        SaveAllClusters();
    }
    private void OnDestroy()
    {
        if (db != null)
        {

            db.Close();
        }
    }
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Edit/Delete Save File", priority = 15000)]
    public static void DeleteSave()
    {

        try
        {

            File.Delete(FILE_PATH);
        }
        catch (Exception e)
        {
            db = new SQLiteConnection(FILE_PATH);
            db.Execute("DROP TABLE IF EXISTS SaveData");
            db.Commit();
            db.Close();
            Debug.LogError($"Failed to delete save file: {e.Message}");
        }
    }
#endif
    public override void ClearData(string key)
    {
        CurrentFile.Remove(key);
    }

    public override int GetValue(string key, int defaultValue = 0)
    {
        if (CurrentFile.TryGetValue(key, out var value) && int.TryParse(value, out int result)) return result;

        CurrentFile[key] = defaultValue.ToString();
        return defaultValue;
    }
    public override string GetValue(string key, string defaultValue = "")
    {
        if (CurrentFile.TryGetValue(key, out var value)) return value;

        CurrentFile[key] = defaultValue.ToString();
        return defaultValue;
    }

    public override float GetValue(string key, float defaultValue = 0)
    {
        if (CurrentFile.TryGetValue(key, out var value) && float.TryParse(value, out float result)) return result;
        CurrentFile[key] = defaultValue.ToString();
        return defaultValue;
    }

    public override TEnum GetValue<TEnum>(string key, TEnum defaultValue = default, EnumSaveSetting saveType = EnumSaveSetting.AsString)
    {
        if (saveType == EnumSaveSetting.AsInt) return (TEnum)Enum.ToObject(typeof(TEnum), GetValue(key, Convert.ToInt32(defaultValue)));
        return (TEnum)Enum.Parse(typeof(TEnum), GetValue(key, defaultValue.ToString()));
    }

    public override BigInteger GetValue(string key, BigInteger defaultValue = default)
    {
        var value = GetValue(key, defaultValue.ToString());
        if (BigInteger.TryParse(value, out BigInteger result)) return result;

        CurrentFile[key] = defaultValue.ToString();
        return defaultValue;
    }

    public override bool GetValue(string key, bool defaultValue = false)
    {
        if (CurrentFile.TryGetValue(key, out var value) && bool.TryParse(value, out bool result)) return result;

        CurrentFile[key] = defaultValue.ToString();
        return defaultValue;
    }


    public override void SetValue(string key, int value)
    {
        CurrentFile[key] = value.ToString();
    }

    public override void SetValue(string key, float value)
    {
        CurrentFile[key] = value.ToString();
    }

    public override void SetValue(string key, string value)
    {
        CurrentFile[key] = value.ToString();
    }

    public override void SetValue<T>(string key, T value, EnumSaveSetting saveSetting = EnumSaveSetting.AsString)
    {
        if (saveSetting == EnumSaveSetting.AsString) SetValue(key, value.ToString());
        else SetValue(key, Convert.ToInt32(value));
    }

    public override void SetValue(string key, BigInteger value)
    {
        CurrentFile[key] = value.ToString();
    }

    public override void SetValue(string key, bool value)
    {
        CurrentFile[key] = value.ToString();
    }
    private class RawRow
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}