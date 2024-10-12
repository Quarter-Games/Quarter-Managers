using UnityEngine;

namespace Transactions
{
    public class RewardTransaction : Transaction
    {
        public Currency Currency;
        public int Amount;

        [ContextMenu("Execute")]
        public override void Execute()
        {
            Currency.Increment(Amount);
        }

        public override bool IsPossible() => true;
    }
}