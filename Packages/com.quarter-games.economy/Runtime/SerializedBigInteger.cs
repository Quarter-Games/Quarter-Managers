using System;
using System.Numerics;
using UnityEditor;
using UnityEngine;

namespace QG.Managers.Economy
{
    [Serializable]
    public class SerializedBigInteger : ISerializationCallbackReceiver
    {
        [SerializeField]
        //this is the actual text value that Unity will serialize and save to disk
        //since there is no inbuilt support for BigInteger serialization
        private string _textValue;

        public BigInteger ActualValue;

        // Implicit conversion: UnitySerializableBigInteger -> BigInteger
        public static implicit operator BigInteger(SerializedBigInteger unityBigInt)
        {
            return unityBigInt.ActualValue;
        }

        // Implicit conversion: BigInteger -> UnitySerializableBigInteger
        public static implicit operator SerializedBigInteger(BigInteger bigInt)
        {
            var unityBigInt = new SerializedBigInteger(bigInt);
            return unityBigInt;
        }
        public SerializedBigInteger(BigInteger rawValue)
        {
            ActualValue = rawValue;
        }

        public void OnBeforeSerialize()
        {
            _textValue = ActualValue.ToString();
        }

        public void OnAfterDeserialize()
        {
            try
            {
                ActualValue = BigInteger.Parse(_textValue);
            }
            catch
            {

                Debug.LogError($"Could not parse value '{_textValue}' into big integer. Set to zero");
                ActualValue = BigInteger.Zero;
            }

        }


#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(SerializedBigInteger))]
        class SerializableBigIntegerPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                string bstr;
                property = property.FindPropertyRelative(nameof(_textValue));
                SerializedBigInteger sbint = null;
                if (property == null)
                {
                    sbint = new SerializedBigInteger(BigInteger.Zero);
                    property.stringValue = sbint.ActualValue.ToString();
                }
                sbint = new SerializedBigInteger(BigInteger.Parse(property.stringValue));

                bstr = EditorGUI.DelayedTextField(position, label, sbint.ActualValue.ToString());

                //set the value we get from text field               
                property.stringValue = BigInteger.Parse(bstr).ToString();

            }

        }
#endif
    }
}
