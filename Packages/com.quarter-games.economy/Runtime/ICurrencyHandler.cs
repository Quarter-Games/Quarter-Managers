using System;
using System.Numerics;
using UnityEngine;

namespace QG.Managers.Economy
{
    public interface ICurrencyHandler
    {
        public string UniqueID { get; }
        public static Action<ICurrencyHandler> CurrencyLoadRequest;
        public void SetCurrencyData(Currency currency, BigInteger amount);
    }
}
