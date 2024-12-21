using System.Threading.Tasks;
using UnityEngine;

namespace QG.Managers.Economy.BasicSave
{
    using QG.Managers.SaveSystem.Basic;
    using System.Numerics;

    [AddComponentMenu("QG/Managers/Basic Economy Manager")]
    sealed internal class BasicSaveEconomyManager : EconomyManager
    {
        async public override Task Decrement(Currency currency, BigInteger amount, ICurrencyHandler handler)
        {
            if (handler == null) handler = this;
            BasicSaveLoadManager.SetData(handler.UniqueID + currency.currencyID, await GetAmount(currency, handler) - amount, handler);
        }

        async public override Task<BigInteger> GetAmount(Currency currency, ICurrencyHandler handler)
        {
            if (handler == null) handler = this;
            return await BasicSaveLoadManager.GetData(handler.UniqueID + currency.currencyID, currency.StartingAmount, handler);
        }

        async public override Task Increment(Currency currency, BigInteger amount, ICurrencyHandler handler)
        {
            if (handler == null) handler = this;
            BasicSaveLoadManager.SetData(handler.UniqueID + currency.currencyID, await GetAmount(currency, handler) + amount, handler);
        }

        public override bool IsReady() => true;

    }
}