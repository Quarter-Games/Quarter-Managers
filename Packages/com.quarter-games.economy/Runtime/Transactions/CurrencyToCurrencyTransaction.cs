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

        public override void Execute(ICurrencyHandler sender, ICurrencyHandler reciever)
        {
            if (!IsPossible(sender)) return;
            ReducedCurrency.Decrement(Cost, sender);
            GainedCurrency.Increment(Gain, reciever);
        }

        public override void ExecuteFirst(ICurrencyHandler sender)
        {
            if (!IsPossible(sender)) return;
            ReducedCurrency.Decrement(Cost, sender);
        }

        public override void ExecuteSecond(ICurrencyHandler reciever)
        {
            GainedCurrency.Increment(Gain,reciever);
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

        public override bool IsPossible(ICurrencyHandler sender) => ReducedCurrency.GetAmount(sender) >= Cost;
    }
}