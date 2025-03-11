using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace QG.Managers.Economy
{
    [CreateAssetMenu(fileName = "Currency", menuName = "Quarter Asset/Economy/Currency")]
    public class Currency : ScriptableObject
    {
        public event Action<Currency, BigInteger> OnCurrencyChanged;
        public string currencyName;
        [Tooltip("Unique ID for the currency. If it is changed, currency will be cleared")]
        public string currencyID;
        [SerializeField] List<CurrencyIcon> Icons = new();
        public SerializedBigInteger StartingAmount;
        [Tooltip("If Max Amount is equal to Starting Amount, the currency will have no limit.")]
        public SerializedBigInteger MaxAmount;
        public BigInteger GetAmount(ICurrencyHandler handler = null) => EconomyManager.GetCurrencyAmount(this, handler);
        public string GetAmountString(ICurrencyHandler handler = null) => new CurrencyValue(EconomyManager.GetCurrencyAmount(this, handler)).GetStringValue();
        public void Increment(BigInteger amount, ICurrencyHandler handler = null)
        {
            EconomyManager.IncrementCurrency(this, amount, handler);
            OnCurrencyChanged?.Invoke(this, amount);
        }
        public void Decrement(BigInteger amount, ICurrencyHandler handler = null)
        {
            EconomyManager.DecrementCurrency(this, amount, handler);
            OnCurrencyChanged?.Invoke(this, -amount);
        }
        [ContextMenu("Log values")]
        public void LogValue()
        {
            Debug.Log(new CurrencyValue(StartingAmount.ActualValue).GetStringValue() + " " + new CurrencyValue(MaxAmount.ActualValue).GetStringValue());
        }
        public Sprite GetIcon(string Key)
        {
            if (Icons == null||Icons.Count==0) return null;
            return Icons.FirstOrDefault(x => x.Name == Key).Icon;
        }
    }
    [Serializable]
    public struct CurrencyValue
    {
        public SerializedBigInteger Value;
        public CurrencyValue(BigInteger value)
        {
            Value = new SerializedBigInteger(value);
        }
        public string GetStringValue()
        {
            BigInteger value = Value.ActualValue;
            List<string> suffixes = new()
            {
                "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af",
                "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az"
            };
            int index = 0;
            foreach (string suffix in suffixes)
            {
                if (value < 1000000)
                {
                    float fvalue = (((float)value) / 1000);
                    if (value < 1000)
                    {
                        fvalue *= 1000;
                    }
                    else
                    {
                        index++;
                    }
                    return fvalue.ToString("G3") + suffixes[index];
                }
                value /= 1000;
                index++;
            }
            return value.ToString() + suffixes[index];
        }
    }
}