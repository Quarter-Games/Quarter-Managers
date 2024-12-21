using System;
using System.Collections.Generic;
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
        public Sprite sprite;
        public BigInteger StartingAmount;
        [Tooltip("If Max Amount is equal to Starting Amount, the currency will have no limit.")]
        public BigInteger MaxAmount;
        public BigInteger GetAmount(ICurrencyHandler handler = null) => EconomyManager.GetCurrencyAmount(this, handler).Result;
        public string GetAmountString(ICurrencyHandler handler = null) => new CurrencyValue(EconomyManager.GetCurrencyAmount(this, handler).Result).GetStringValue();
        public void Increment(BigInteger amount, ICurrencyHandler handler = null)
        {
            OnCurrencyChanged?.Invoke(this, amount);
            EconomyManager.IncrementCurrency(this, amount, handler);
        }
        public void Decrement(BigInteger amount, ICurrencyHandler handler = null)
        {
            OnCurrencyChanged?.Invoke(this, -amount);
            EconomyManager.DecrementCurrency(this, amount, handler);
        }
    }
    [Serializable]
    public struct CurrencyValue
    {
        public BigInteger Value;
        public CurrencyValue(BigInteger value)
        {
            Value = value;
        }
        public string GetStringValue()
        {
            BigInteger value = Value;
            List<string> suffixes = new List<string> { "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af",
            "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az" };
            int index = 0;
            foreach (string suffix in suffixes)
            {
                if (value < 1000)
                {
                    return value.ToString() + suffix;
                }
                value /= 1000;
                index++;
            }
            return value.ToString() + suffixes[index];
        }
    }
}