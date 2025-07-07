using System;
using System.Collections.Generic;
using UnityEngine;

abstract public class TableHandler : ScriptableObject
{
    public string TableName;
    abstract public void ParseTable(string tableID);
    abstract public Type GetDataType();
    abstract public List<Type> GetScheme();
}
abstract public class TableHandler<T> : TableHandler where T : ITableRowData
{
    public List<T> TableRows;
    public override Type GetDataType()
    {
        return typeof(T);
    }
    public override void ParseTable(string tableID)
    {
        var types = GetScheme();
        ConfigManager.RequestTable(tableID, TableName, types, CallBack);
    }
    abstract public void CallBack(GoogleTable table);
}