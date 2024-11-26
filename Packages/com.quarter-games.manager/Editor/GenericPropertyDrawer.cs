using UnityEngine;
namespace QG.Managers.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;

    public class GenericPropertyDrawer<T> : PropertyDrawer
    {

        static List<Type> TargetTypes = new();
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {

            var result = new Foldout();
            //Parsing all types that inherit from Transaction
            if (TargetTypes.Count == 0)
            {
                TargetTypes = (
                    from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in domainAssembly.GetTypes()
                    where typeof(T).IsAssignableFrom(type)
                    && !type.IsAbstract
                    select type).ToList();
            }

            var currentType = property.managedReferenceFullTypename;
            Debug.Log(currentType);
            int currentValue = TargetTypes.FindIndex(x => x.Name == currentType.Split(".")[^1]);
            var dropDown = new PopupField<string>("Quest Type", TargetTypes.Select(x => x.Name).ToList(), currentValue == -1 ? 0 : currentValue);
            dropDown.RegisterValueChangedCallback((evt) =>
            {
                property.managedReferenceValue = Activator.CreateInstance(TargetTypes.Find(x => x.Name == evt.newValue));
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