using UnityEngine;

namespace QuarterAsset.Economy
{
    [CreateAssetMenu(fileName = "Currency", menuName = "Quarter Asset/Economy/Currency")]
    public class Currency : ScriptableObject
    {
        public event System.Action<Currency, float> OnCurrencyChanged;
        public string currencyName;
        [Tooltip("Unique ID for the currency. This is used to reference the currency in code.")]
        public string currencyID;
        public Sprite sprite;
        public float StartingAmount;
        [Tooltip("If Max Amount is equal to Starting Amount, the currency will have no limit.")]
        public float MaxAmount;
        private void OnValidate()
        {
            currencyID = currencyName.Replace(" ", "_").ToUpper();
        }
        public float GetAmount() => EconomyManager.GetCurrencyAmount(this);
        public void Increment(float amount)
        {
            EconomyManager.IncrementCurrency(this, amount);
            OnCurrencyChanged?.Invoke(this, amount);
        }
        public void Decrement(float amount)
        {
            EconomyManager.DecrementCurrency(this, amount);
            OnCurrencyChanged?.Invoke(this, -amount);
        }
    }
}