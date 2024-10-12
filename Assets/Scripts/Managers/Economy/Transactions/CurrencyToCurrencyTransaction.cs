using UnityEngine;

namespace QuarterAsset.Economy.Transactions
{
    public class CurrencyToCurrencyTransaction : Transaction
    {
        public Currency ReducedCurrency;
        public int Cost;
        public Currency GainedCurrency;
        public int Gain;

        [ContextMenu("Execute")]
        public override void Execute()
        {
            ReducedCurrency.Decrement(Cost);
            GainedCurrency.Increment(Gain);
        }

        public override bool IsPossible() => ReducedCurrency.GetAmount() >= Cost;
    }
}