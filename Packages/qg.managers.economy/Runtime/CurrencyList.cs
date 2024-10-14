using System.Collections.Generic;
using UnityEngine;

namespace QG.Managers.Economy
{
    [CreateAssetMenu(fileName = "CurrencyList", menuName = "Quarter Asset/Economy/CurrencyList")]
    public class CurrencyList : ScriptableObject
    {
        public List<Currency> GameCurrencies;
    }
}