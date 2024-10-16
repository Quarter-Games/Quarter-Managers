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
}