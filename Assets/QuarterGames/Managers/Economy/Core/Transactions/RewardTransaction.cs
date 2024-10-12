namespace QG.Managers.Economy.Transactions
{
    [System.Serializable]
    public class RewardTransaction : Transaction
    {
        public Currency Currency;
        public int Amount;

        public override void Execute()
        {
            Currency.Increment(Amount);
        }

        public override bool IsPossible() => true;
    }
}