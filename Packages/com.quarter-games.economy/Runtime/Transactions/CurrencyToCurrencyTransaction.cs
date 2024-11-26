using System;

namespace QG.Managers.Economy.Transactions
{
    [Serializable]
    public class CurrencyToCurrencyTransaction : Transaction
    {
        public Currency ReducedCurrency;
        public int Cost;
        public Currency GainedCurrency;
        public int Gain;

        public override void Execute()
        {
            if (ReducedCurrency.GetAmount() < Cost) return;
            ReducedCurrency.Decrement(Cost);
            GainedCurrency.Increment(Gain);
        }

        public override void ExecuteFirst()
        {
            if (ReducedCurrency.GetAmount() < Cost) return;
            ReducedCurrency.Decrement(Cost);
        }

        public override void ExecuteSecond()
        {
            GainedCurrency.Increment(Gain);
        }

        public override string GetCostValue()
        {
            return new CurrencyValue(Cost).GetStringValue();
        }

        public override bool IsPossible() => ReducedCurrency.GetAmount() >= Cost;
    }
}