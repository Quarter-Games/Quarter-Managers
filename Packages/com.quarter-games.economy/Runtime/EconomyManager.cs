using System;
using System.Threading.Tasks;
using UnityEngine;

namespace QG.Managers.Economy
{
    abstract public class EconomyManager : SingletonManager<EconomyManager>, ICurrencyHandler
    {
        public CurrencyList CurrencyList;

        private void OnEnable()
        {
            ICurrencyHandler.CurrencyLoadRequest += CurrencyLoadRequest;
        }
        public override void Init()
        {
            base.Init();
            CurrencyLoadRequest(this);
        }

        virtual protected void CurrencyLoadRequest(ICurrencyHandler handler)
        {
            if (CurrencyList == null)
            {
                Debug.LogError("Currency List is not set in the Economy Manager");
            }
            foreach (var currency in CurrencyList.GameCurrencies)
            {
                if (currency.currencyID == "")
                {
                    Debug.LogError("Currency ID is not set for " + currency.currencyName);
                    continue;
                }
                handler.SetCurrencyData(currency, GetAmount(currency, handler).Result);
            }
        }

        private void OnDisable()
        {
            ICurrencyHandler.CurrencyLoadRequest -= CurrencyLoadRequest;
        }
        public string UniqueID => "Player/";
        #region Static Methods
        async public static Task<int> GetCurrencyAmount(Currency currency, ICurrencyHandler handler)
        {
            if (Instance == null) LoadFallBackManager();
            return await Instance.GetAmount(currency, handler);
        }
        async public static void IncrementCurrency(Currency currency, int amount, ICurrencyHandler handler)
        {
            if (Instance == null) LoadFallBackManager();
            await Instance.Increment(currency, amount, handler);
        }
        async public static void DecrementCurrency(Currency currency, int amount, ICurrencyHandler handler)
        {
            if (Instance == null) LoadFallBackManager();
            await Instance.Decrement(currency, amount, handler);
        }
        #endregion
        #region Abstract Methods
        abstract public Task<int> GetAmount(Currency currency, ICurrencyHandler handler);
        abstract public Task Increment(Currency currency, int amount, ICurrencyHandler handler);
        abstract public Task Decrement(Currency currency, int amount, ICurrencyHandler handler);

        public void SetCurrencyData(Currency currency, int amount)
        {

        }
        #endregion
    }
}