using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QG.Managers.Economy.Editor
{
    using QG.Managers.Economy.Transactions;
    using System.Collections.Generic;
    using UnityEditor.UIElements;
    [CustomPropertyDrawer(typeof(Transaction))]
    public class TransactionPropertyDrawer : PropertyDrawer
    {

        static List<Type> TransactionTypes = new();
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {

            var result = new Foldout();
            //Parsing all types that inherit from Transaction
            if (TransactionTypes.Count == 0)
            {
                TransactionTypes = (
                    from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in domainAssembly.GetTypes()
                    where typeof(Transaction).IsAssignableFrom(type)
                    && !type.IsAbstract
                    select type).ToList();
            }

            var currentType = property.managedReferenceFullTypename;
            Debug.Log(currentType);
            int currentValue = TransactionTypes.FindIndex(x => x.Name == currentType.Split(".")[^1]);
            var dropDown = new PopupField<string>("Transaction Type", TransactionTypes.Select(x => x.Name).ToList(), currentValue == -1 ? 0 : currentValue);
            dropDown.RegisterValueChangedCallback((evt) =>
            {

                property.managedReferenceValue = Activator.CreateInstance(TransactionTypes.Find(x => x.Name == evt.newValue));
                property.serializedObject.ApplyModifiedProperties();
            });
            var propField = new PropertyField(property);
            result.Add(dropDown);
            result.Add(propField);
            propField.label = "Data";
            result.text = property.displayName + " (" + property.managedReferenceValue?.GetType().Name + ")";
            return result;
        }
    }
}
