using System.Numerics;

namespace QG.Managers.Economy.Transactions
{
    [System.Serializable]
    public class RewardTransaction : Transaction
    {
        public Currency Currency;
        public BigInteger Amount;

        public override void Execute(ICurrencyHandler sender, ICurrencyHandler reciever)
        {
            Currency.Increment(Amount, reciever);
        }

        public override void ExecuteFirst(ICurrencyHandler sender)
        {
            return;
        }

        public override void ExecuteSecond(ICurrencyHandler reciever)
        {
            Currency.Increment(Amount, reciever);
        }

        public override string GetCostValue()
        {
            return new CurrencyValue(Amount).GetStringValue();
        }

        public override bool IsPossible(ICurrencyHandler sender) => true;
        public override Transaction GetCTCTransaction()
        {
            return new RewardTransaction
            {
                Currency = Currency,
                Amount = Amount
            };
        }
    }
}