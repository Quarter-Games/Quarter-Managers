using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class GoogleTable
{
    public List<Type> MainRowTypes = new();
    public List<List<GoogleCell>> list;
    public List<string> ColumnNames = new();
    public GoogleTable(string tableJson, List<Type> types)
    {
        var responce = JsonConvert.DeserializeObject<SheetResponse>(tableJson);
        list = new List<List<GoogleCell>>();
        var firstRow = responce.values[0];
        for (int i = 0; i < types.Count; i++)
        {
            var cell = firstRow[i];
            ColumnNames.Add(cell);
        }
        MainRowTypes = new List<Type>(types);

        for (int i = 1; i < responce.values.Count; i++)
        {
            var row = responce.values[i];
            var listRow = new List<GoogleCell>();
            if (string.IsNullOrEmpty(row[0])) break;
            for (int j = 0; j < types.Count; j++)
            {
                var cell = row[j];
                var type = MainRowTypes[j];
                var cellType = (GoogleCell)Activator.CreateInstance(type, cell);
                listRow.Add(cellType);
            }
            list.Add(listRow);
        }
    }
    [Serializable]
    public class SheetResponse
    {
        public string range;
        public string majorDimension;
        public List<List<string>> values;
    }
}




public class BoolCell : GoogleCell<bool>
{
    public BoolCell(string value)
    {
        if (bool.TryParse(value, out var result))
        {
            Value = result;
        }
        else
            Debug.LogError($"Failed to parse {value} to bool.");
    }
}
abstract public class RangeCell<T> : GoogleCell<GoogleCell<T>>
{
    public T SecondValue { get; set; }
    public RangeCell(string value)
    {
        var values = value.Split('-');
        if (values.Length == 2)
        {
            var firstValue = values[0].Trim();
            var secondValue = values[1].Trim();
            if (!string.IsNullOrEmpty(firstValue) && !string.IsNullOrEmpty(secondValue))
            {
                var firstCell = (GoogleCell<T>)Activator.CreateInstance(typeof(GoogleCell<T>), firstValue);
                var secondCell = (GoogleCell<T>)Activator.CreateInstance(typeof(GoogleCell<T>), secondValue);
                if (firstCell != null && secondCell != null)
                {
                    Value = firstCell;
                    SecondValue = secondCell.Data;
                }
                else
                    Debug.LogError($"Failed to parse {value} to RangeCell.");
            }
            else
                Debug.LogError($"Failed to parse {value} to RangeCell.");
        }
        else
            Debug.LogError($"Invalid range format: {value}");
    }
}
public class RefCell<T> : GoogleCell<T> where T : ITableRowData
{

    public RefCell(string value)
    {
        var obj = ConfigManager.Instance.TablesToParse.Find(x => x.GetDataType() == typeof(T));
        if (obj == null)
        {
            Debug.LogError($"Table {typeof(T)} not found.");
            return;
        }
        var table = obj as TableHandler<T>;
        if (table == null)
        {
            Debug.LogError($"Table {typeof(T)} is not a TableHandler.");
            return;
        }
        var row = table.TableRows.Find(x => x.ReferenceIdentifier == value);
        if (row == null)
        {
            Debug.LogError($"Row with reference {value} not found in table {typeof(T)}.");
            return;
        }
        Value = row;
    }
}
public class EnumCell<T> : GoogleCell<T> where T : Enum
{
    public EnumCell(string value)
    {
        value = value.Replace(" ", "_");
        if (Enum.TryParse(typeof(T), value, out var result))
        {
            Data = (T)result;
        }
        else
            Debug.LogError($"Failed to parse {value} to {typeof(T)}.");
    }
}
public class ListCell<T> : GoogleCell<List<T>> where T : GoogleCell
{
    public ListCell(string value)
    {
        var list = new List<T>();
        var values = value.Split(',');
        foreach (var v in values)
        {
            var trimmedValue = v?.Trim();
            if (!string.IsNullOrEmpty(trimmedValue))
            {
                var type = typeof(T);
                T cell = (T)Activator.CreateInstance(type, trimmedValue);
                list.Add(cell);
            }
        }
        Value = list;
    }
}
public class BigIntegerCell : GoogleCell<System.Numerics.BigInteger>
{
    public BigIntegerCell(string value)
    {
        if (System.Numerics.BigInteger.TryParse(value, out var result))
        {
            Value = result;
        }
        else
            Debug.LogError($"Failed to parse {value} to BigInteger.");
    }
}
public class FloatCell : GoogleCell<float>
{
    public static NumberFormatInfo provider;
    public FloatCell(string value)
    {
        if (provider == null)
        {
            provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
        }
        if (float.TryParse(value, NumberStyles.Float, provider, out var result))
        {
            Value = result;
        }
        else
            Debug.LogError($"Failed to parse {value} to float.");
    }
}
public class StringCell : GoogleCell<string>
{
    public StringCell(string value)
    {
        Value = value;
    }
}
public class IntCell : GoogleCell<int>
{
    public IntCell(string value)
    {
        if (int.TryParse(value, out int result))
            Value = result;
        else
            Debug.LogError($"Failed to parse {value} to int.");
    }
}
abstract public class GoogleCell<T> : GoogleCell
{
    public T Data { get; set; }
    public override object Value { get => Data; set => Data = (T)value; }

}
abstract public class GoogleCell
{
    public virtual object Value { get; set; }
    public static T GetValue<T>(GoogleCell cell)
    {
        return (T)cell.Value;
    }
}
