using System;
using UnityEngine;

namespace QG.Managers.Economy
{
    public interface ICurrencyHandler
    {
        public string UniqueID { get; }
        public static Action<ICurrencyHandler> CurrencyLoadRequest;
        public void SetCurrencyData(Currency currency, int amount);
    }
}
