using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using QG.Managers.Economy.Transactions;

static public class TransactionContextMenuExtension
{
    public static List<Type> TransactionTypes;
    [InitializeOnLoadMethod]
    static public void Init()
    {
        EditorApplication.contextualPropertyMenu += HandleContexMenuOpen;
        TransactionTypes = (
                from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in domainAssembly.GetTypes()
                where typeof(Transaction).IsAssignableFrom(type)
                && !type.IsAbstract
                select type).ToList();
    }
    private static void HandleContexMenuOpen(GenericMenu menu, SerializedProperty property)
    {
        if (property == null)
            return;
        if (property.propertyType != SerializedPropertyType.ManagedReference && property.propertyType != SerializedPropertyType.Generic)
            return;
        if (property.isArray && property.arrayElementType == "managedReference<>")
        {
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Convert empty to C2C"), false, () =>
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    var type = property.GetArrayElementAtIndex(i).managedReferenceValue;
                    if (type is not null)
                        continue;
                    property.GetArrayElementAtIndex(i).managedReferenceValue = Activator.CreateInstance(TransactionTypes[0]) as Transaction;
                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();
                }
            });
            return;
        }
        if (property.managedReferenceValue is not Transaction)
            return;
        menu.AddSeparator("");

        foreach (var type in TransactionTypes)
        {
            menu.AddItem(new GUIContent(string.Concat(type.Name.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ')), false, () =>
            {
                property.managedReferenceValue = Activator.CreateInstance(type) as Transaction;
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            });
        }

    }
}