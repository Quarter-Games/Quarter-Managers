using System;
using UnityEngine;
using UnityEngine.Events;

namespace QuarterAsset.Economy.Transactions
{
    public class CurrencyToGameActionTransaction : Transaction
    {
        public Currency ReducedCurrency;
        public int Cost;
        public UnityEvent Action;
        public void SetAction(UnityEvent action) => Action = action;
        public override void Execute()
        {
            ReducedCurrency.Decrement(Cost);
            Action?.Invoke();
        }

        public override bool IsPossible() => ReducedCurrency.GetAmount() >= Cost;
    }
}