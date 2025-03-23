namespace QG.Managers.Economy.Editor
{
    using UnityEngine;
    using QG.Managers.SaveSystem.Basic;
    using UnityEditor;
    [CustomEditor(typeof(Currency))]
    public class CurrencyEditor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            Currency currency = (Currency)target;
            base.OnInspectorGUI();
            GUILayout.Space(10);
            Debug.Log("Currency ID: " + currency.currencyID);
            var isPressed = GUILayout.Button("Add Value");
            var value = BasicSaveLoadManager.GetData("Player/" + currency.currencyID, currency.StartingAmount.ActualValue, null);
            GUILayout.Label(new CurrencyValue(value).GetStringValue());

        }
    }
}
