using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalRewardParser", menuName = "QG/AnimalRewardParser", order = 1)]
public class AnimalRewardParser : TableHandler<AnimalRewardData>
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
            var row = AnimalRewardData.CreateInstance("Assets/Test", table.list[i][0].Value.ToString(), table.list[i]);
            if (row == null)
            {
                Debug.LogError($"Failed to create instance of {table.list[i][0].Value.ToString()}");
                continue;
            }
            TableRows.Add(row);
        }
        UnityEditor.AssetDatabase.SaveAssets();
    }
    override public List<Type> GetScheme()
    {
        return AnimalRewardData.types;
    }
}
public class AnimalRewardData : ScriptableObject, ITableRowData
{
    public string ReferenceIdentifier => Reward + " " + ID;
    public string ID;
    public int Evolution;
    public RewardType Reward;
    public float Modifier;
    public static List<Type> types = new()
    {
        typeof(StringCell),
        typeof(IntCell),
        typeof(EnumCell<RewardType>),
        typeof(FloatCell)
    };
    public static AnimalRewardData CreateInstance(string Path, string Name, List<GoogleCell> data)
    {
        var instance = CreateInstance<AnimalRewardData>();
        instance.ID = GoogleCell.GetValue<string>(data[0]);
        instance.Evolution = GoogleCell.GetValue<int>(data[1]);
        instance.Reward = GoogleCell.GetValue<RewardType>(data[2]);
        instance.Modifier = GoogleCell.GetValue<float>(data[3]);
        instance.name = Name;
        UnityEditor.AssetDatabase.CreateAsset(instance, Path + "/" + Name + ".asset");
        UnityEditor.EditorUtility.SetDirty(instance);
        return instance;
    }
}
public enum RewardType
{
    Station_Speed,
    Profits
}