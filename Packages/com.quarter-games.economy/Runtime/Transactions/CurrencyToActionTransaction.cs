using System;

namespace QG.Managers.Economy.Transactions
{
    [Serializable]
    public class CurrencyToActionTransaction : Transaction
    {
        public Currency ReducedCurrency;
        public SerializedBigInteger Cost;
        public Action GainedAction;

        public override void Execute(ICurrencyHandler sender, ICurrencyHandler reciever, bool SaveImediate = false)
        {
            if (!IsPossible(sender)) return;
            ReducedCurrency.Decrement(Cost, sender, SaveImediate);
            GainedAction?.Invoke();
        }

        public override void ExecuteFirst(ICurrencyHandler sender, bool SaveImediate = false)
        {
            if (!IsPossible(sender)) return;
            ReducedCurrency.Decrement(Cost, sender, SaveImediate);
        }

        public override void ExecuteSecond(ICurrencyHandler reciever, bool SaveImediate = false)
        {
            GainedAction?.Invoke();
        }

        public override string GetCostValue()
        {
            return new CurrencyValue(Cost).GetStringValue();
        }
        public override Transaction GetCTCTransaction()
        {
            return new CurrencyToActionTransaction
            {
                Cost = Cost,
                ReducedCurrency = ReducedCurrency,
                GainedAction = GainedAction
            };
        }

        public override bool IsPossible(ICurrencyHandler sender) => ReducedCurrency.IsPossible(-Cost.ActualValue, sender);
    }
}