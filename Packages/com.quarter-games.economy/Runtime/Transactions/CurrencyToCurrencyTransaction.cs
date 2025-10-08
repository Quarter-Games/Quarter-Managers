using System;
using System.Numerics;

namespace QG.Managers.Economy.Transactions
{
    [Serializable]
    public class CurrencyToCurrencyTransaction : Transaction
    {
        public Currency ReducedCurrency;
        public SerializedBigInteger Cost;
        public Currency GainedCurrency;
        public SerializedBigInteger Gain;

        public override void Execute(ICurrencyHandler sender, ICurrencyHandler reciever, bool SaveImediate = false)
        {
            if (!IsPossible(sender)) return;
            ReducedCurrency.Decrement(Cost, sender, SaveImediate);
            GainedCurrency.Increment(Gain, reciever, SaveImediate);
        }

        public override void ExecuteFirst(ICurrencyHandler sender, bool SaveImediate = false)
        {
            if (!IsPossible(sender)) return;
            ReducedCurrency.Decrement(Cost, sender, SaveImediate);
        }

        public override void ExecuteSecond(ICurrencyHandler reciever, bool SaveImediate = false)
        {
            GainedCurrency.Increment(Gain, reciever, SaveImediate);
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

        public override bool IsPossible(ICurrencyHandler sender) => ReducedCurrency.IsPossible(-Cost.ActualValue, sender);
    }
}