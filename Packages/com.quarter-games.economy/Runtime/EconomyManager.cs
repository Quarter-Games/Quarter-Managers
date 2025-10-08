using System;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace QG.Managers.Economy
{
    abstract public class EconomyManager : SingletonManager<EconomyManager>, ICurrencyHandler
    {
        public CurrencyList CurrencyList;
        public SpriteLibrary Icons;

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
                handler.SetCurrencyData(currency, GetAmount(currency, handler));
            }
        }
        public static Sprite GetIcon(Currency currency, string Key)
        {
            if (Instance == null) LoadFallBackManager();
            return Instance.Icons.GetSprite(currency.name, Key);
        }
        private void OnDisable()
        {
            ICurrencyHandler.CurrencyLoadRequest -= CurrencyLoadRequest;
        }
        public string UniqueID => "Player/";
        #region Static Methods
        public static BigInteger GetCurrencyAmount(Currency currency, ICurrencyHandler handler)
        {
            if (Instance == null) LoadFallBackManager();
            return Instance.GetAmount(currency, handler);
        }
        public static void IncrementCurrency(Currency currency, BigInteger amount, ICurrencyHandler handler, bool SaveImediate = false)
        {
            if (Instance == null) LoadFallBackManager();
            Instance.Increment(currency, amount, handler, SaveImediate);
        }
        public static void DecrementCurrency(Currency currency, BigInteger amount, ICurrencyHandler handler, bool SaveImediate = false)
        {
            if (Instance == null) LoadFallBackManager();
            Instance.Decrement(currency, amount, handler, SaveImediate);
        }
        #endregion
        #region Abstract Methods
        abstract public BigInteger GetAmount(Currency currency, ICurrencyHandler handler);
        abstract public void Increment(Currency currency, BigInteger amount, ICurrencyHandler handler, bool SaveImediate = false);
        abstract public void Decrement(Currency currency, BigInteger amount, ICurrencyHandler handler, bool SaveImediate = false);

        public void SetCurrencyData(Currency currency, BigInteger amount)
        {

        }

        #endregion
    }
}