using System;
using System.Collections.Generic;
using UnityEngine;

namespace QG.Managers.Economy
{
    [CreateAssetMenu(fileName = "Currency", menuName = "Quarter Asset/Economy/Currency")]
    public class Currency : ScriptableObject
    {
        public event System.Action<Currency, float> OnCurrencyChanged;
        public string currencyName;
        [Tooltip("Unique ID for the currency. If it is changed, currency will be cleared")]
        public string currencyID;
        public Sprite sprite;
        public int StartingAmount;
        [Tooltip("If Max Amount is equal to Starting Amount, the currency will have no limit.")]
        public int MaxAmount;
        public int GetAmount() => EconomyManager.GetCurrencyAmount(this).Result;
        public string GetAmountString() =>new CurrencyValue(EconomyManager.GetCurrencyAmount(this).Result).GetStringValue();
        public void Increment(int amount)
        {
            EconomyManager.IncrementCurrency(this, amount);
            OnCurrencyChanged?.Invoke(this, amount);
        }
        public void Decrement(int amount)
        {
            EconomyManager.DecrementCurrency(this, amount);
            OnCurrencyChanged?.Invoke(this, -amount);
        }
    }
    [Serializable]
    public struct CurrencyValue
    {
        public int Value;
        public CurrencyValue(int value)
        {
            Value = value;
        }
        public string GetStringValue()
        {
            float value = Value;
            List<string> suffixes = new List<string> { "", "K", "M", "B", "T", "Q", "QQ", "S", "SS", "O", "N", "D", "UN", "DD", "TR", "QT", "QN", "SD", "SP", "O", "N", "V", "U", "DUO", "TRE", "QU" };
            int index = 0;
            foreach (string suffix in suffixes) {
                if (value < 1000) {
                    return value.ToString() + suffix;
                }
                value /= 1000;
                index++;
            }
            return value.ToString() + suffixes[index];
        }
    }
}