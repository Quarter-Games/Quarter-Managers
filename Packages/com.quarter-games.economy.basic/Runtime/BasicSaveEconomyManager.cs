using System.Threading.Tasks;
using UnityEngine;

namespace QG.Managers.Economy.BasicSave
{
    using QG.Managers.SaveSystem.Basic;
    using System.Numerics;

    [AddComponentMenu("QG/Managers/Basic Economy Manager")]
    sealed internal class BasicSaveEconomyManager : EconomyManager
    {
        public override void Decrement(Currency currency, BigInteger amount, ICurrencyHandler handler)
        {
            if (handler == null) handler = this;
            BasicSaveLoadManager.SetData(handler.UniqueID + currency.currencyID, GetAmount(currency, handler) - amount, handler);
        }

        public override BigInteger GetAmount(Currency currency, ICurrencyHandler handler)
        {
            if (handler == null) handler = this;
            return BasicSaveLoadManager.GetData(handler.UniqueID + currency.currencyID, currency.StartingAmount, handler);
        }

        public override void Increment(Currency currency, BigInteger amount, ICurrencyHandler handler)
        {
            if (handler == null) handler = this;
            BasicSaveLoadManager.SetData(handler.UniqueID + currency.currencyID, GetAmount(currency, handler) + amount, handler);
        }

        public override bool IsReady() => true;

    }
}