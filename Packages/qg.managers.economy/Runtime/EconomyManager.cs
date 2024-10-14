using System.Threading.Tasks;
using UnityEngine;

namespace QG.Managers.Economy
{
    abstract public class EconomyManager : SingletonManager<EconomyManager>
    {
        public CurrencyList CurrencyList;
        #region Static Methods
        async public static Task<int> GetCurrencyAmount(Currency currency)
        {
            if (Instance == null) LoadFallBackManager();
            return await Instance.GetAmount(currency);
        }
        async public static void IncrementCurrency(Currency currency, int amount)
        {
            if (Instance == null) LoadFallBackManager();
            await Instance.Increment(currency, amount);
        }
        async public static void DecrementCurrency(Currency currency, int amount)
        {
            if (Instance == null) LoadFallBackManager();
            await Instance.Decrement(currency, amount);
        }
        #endregion
        #region Abstract Methods
        abstract public Task<int> GetAmount(Currency currency);
        abstract public Task Increment(Currency currency, int amount);
        abstract public Task Decrement(Currency currency, int amount);
        #endregion
    }
}