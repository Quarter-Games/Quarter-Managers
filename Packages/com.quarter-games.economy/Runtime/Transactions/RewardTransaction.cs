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

        public override void ExecuteFirst()
        {
            return;
        }

        public override void ExecuteSecond()
        {
            Currency.Increment(Amount);
        }

        public override string GetCostValue()
        {
            return new CurrencyValue(Amount).GetStringValue();
        }

        public override bool IsPossible() => true;
    }
}