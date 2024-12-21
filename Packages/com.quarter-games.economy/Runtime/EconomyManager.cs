using System;
using System.Numerics;
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
        async public static Task<BigInteger> GetCurrencyAmount(Currency currency, ICurrencyHandler handler)
        {
            if (Instance == null) LoadFallBackManager();
            return await Instance.GetAmount(currency, handler);
        }
        async public static void IncrementCurrency(Currency currency, BigInteger amount, ICurrencyHandler handler)
        {
            if (Instance == null) LoadFallBackManager();
            await Instance.Increment(currency, amount, handler);
        }
        async public static void DecrementCurrency(Currency currency, BigInteger amount, ICurrencyHandler handler)
        {
            if (Instance == null) LoadFallBackManager();
            await Instance.Decrement(currency, amount, handler);
        }
        #endregion
        #region Abstract Methods
        abstract public Task<BigInteger> GetAmount(Currency currency, ICurrencyHandler handler);
        abstract public Task Increment(Currency currency, BigInteger amount, ICurrencyHandler handler);
        abstract public Task Decrement(Currency currency, BigInteger amount, ICurrencyHandler handler);

        public void SetCurrencyData(Currency currency, BigInteger amount)
        {
            
        }

        #endregion
    }
}