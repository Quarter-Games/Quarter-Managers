using System;
using UnityEngine.Events;

namespace QG.Managers.Economy.Transactions
{
    [Serializable]
    public class CurrencyToGameActionTransaction : Transaction
    {
        public Currency ReducedCurrency;
        public int Cost;
        public UnityEvent Action;
        public void SetAction(UnityEvent action) => Action = action;
        public override void Execute()
        {
            if (ReducedCurrency.GetAmount() < Cost) return;
            ReducedCurrency.Decrement(Cost);
            Action?.Invoke();
        }

        public override bool IsPossible() => ReducedCurrency.GetAmount() >= Cost;

        public override void ExecuteFirst()
        {
            if (ReducedCurrency.GetAmount() < Cost) return;
            ReducedCurrency.Decrement(Cost);
        }

        public override void ExecuteSecond()
        {
            Action?.Invoke();
        }

        public override string GetCostValue()
        {
            return new CurrencyValue(Cost).GetStringValue();
        }
    }
}