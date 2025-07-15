
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalParser", menuName = "QG/AnimalParser")]
internal class AnimalParser : TableHandler<AnimalData>
{
    public override void CallBack(GoogleTable table)
    {
        if (table == null)
        {
            Debug.LogError($"Table {TableName} not found");
            return;
        }
        for (int i = 0; i < table.list.Count; i++)
        {
            var row = AnimalData.CreateInstance("Assets/Test", table.list[i][1].Value.ToString(), table.list[i]);
            if (row == null)
            {
                Debug.LogError($"Failed to create instance of {table.list[i][1].Value.ToString()}");
                continue;
            }
            TableRows.Add(row);
        }
        UnityEditor.AssetDatabase.SaveAssets();
    }

    public override List<Type> GetScheme()
    {
        return AnimalData.types;
    }
}

public class AnimalData : ScriptableObject, ITableRowData
{
    public string ReferenceIdentifier => Name;
    public string ID;
    public string Name;
    public string Description;
    public int Level;
    public float BaseTime;
    public CurrencyType CurrencyType;
    public int CurrencyAmount;
    public CurrencyType Resource;
    public int ResourceAmount;
    public List<RewardType> Rewards;
    public static List<Type> types = new()
    {
        typeof(StringCell),
        typeof(StringCell),
        typeof(StringCell),
        typeof(IntCell),
        typeof(FloatCell),
        typeof(EnumCell<CurrencyType>),
        typeof(IntCell),
        typeof(EnumCell<CurrencyType>),
        typeof(IntCell),
        typeof(ListCell<EnumCell<RewardType>>)
    };
    public static AnimalData CreateInstance(string Path, string Name, List<GoogleCell> data)
    {
        var instance = CreateInstance<AnimalData>();
        instance.ID = GoogleCell.GetValue<string>(data[0]);
        instance.Name = GoogleCell.GetValue<string>(data[1]);
        instance.Description = GoogleCell.GetValue<string>(data[2]);
        instance.Level = GoogleCell.GetValue<int>(data[3]);
        instance.BaseTime = GoogleCell.GetValue<float>(data[4]);
        instance.CurrencyType = GoogleCell.GetValue<CurrencyType>(data[5]);
        instance.CurrencyAmount = GoogleCell.GetValue<int>(data[6]);
        instance.Resource = GoogleCell.GetValue<CurrencyType>(data[7]);
        instance.ResourceAmount = GoogleCell.GetValue<int>(data[8]);
        instance.Rewards = GoogleCell.GetValue<List<EnumCell<RewardType>>>(data[9]).Select(x => x.Data).ToList(); 
        instance.name = Name;
        Name = Name.Replace("?", "");
        UnityEditor.AssetDatabase.CreateAsset(instance, Path + "/" + Name + ".asset");
        UnityEditor.EditorUtility.SetDirty(instance);
        return instance;
    }
}

public enum CurrencyType
{
    Soft_Currency,
    Hard_Currency,
    Medicine,
}