using System;
using System.Numerics;
using UnityEngine.Events;

namespace QG.Managers.Economy.Transactions
{
    [Serializable]
    public class CurrencyToGameActionTransaction : Transaction
    {
        public Currency ReducedCurrency;
        public SerializedBigInteger Cost;
        public UnityEvent Action;
        public void SetAction(UnityEvent action) => Action = action;
        public override void Execute(ICurrencyHandler sender, ICurrencyHandler reciever, bool SaveImediate = false)
        {
            if (!IsPossible(sender)) return;
            ReducedCurrency.Decrement(Cost, sender, SaveImediate);
            Action?.Invoke();
        }

        public override bool IsPossible(ICurrencyHandler sender) => ReducedCurrency.IsPossible(-Cost.ActualValue, sender);

        public override void ExecuteFirst(ICurrencyHandler sender, bool SaveImediate = false)
        {
            if (ReducedCurrency.GetAmount(sender) < Cost) return;
            ReducedCurrency.Decrement(Cost, sender, SaveImediate);
        }

        public override void ExecuteSecond(ICurrencyHandler reciever, bool SaveImediate = false)
        {
            Action?.Invoke();
        }

        public override string GetCostValue()
        {
            return new CurrencyValue(Cost).GetStringValue();
        }

        public override Transaction GetCTCTransaction()
        {
            return new CurrencyToCurrencyTransaction
            {
                Cost = Cost,
                ReducedCurrency = ReducedCurrency,
                GainedCurrency = ReducedCurrency,
                Gain = Cost
            };
        }
    }
}