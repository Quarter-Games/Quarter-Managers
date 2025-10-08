using System.Numerics;

namespace QG.Managers.Economy.Transactions
{
    [System.Serializable]
    public class RewardTransaction : Transaction
    {
        public Currency Currency;
        public SerializedBigInteger Amount;

        public override void Execute(ICurrencyHandler sender, ICurrencyHandler reciever, bool SaveImediate = false)
        {
            Currency.Increment(Amount, reciever, SaveImediate);
        }

        public override void ExecuteFirst(ICurrencyHandler sender, bool SaveImediate = false)
        {
            return;
        }

        public override void ExecuteSecond(ICurrencyHandler reciever, bool SaveImediate = false)
        {
            Currency.Increment(Amount, reciever, SaveImediate);
        }

        public override string GetCostValue()
        {
            return new CurrencyValue(Amount).GetStringValue();
        }

        public override bool IsPossible(ICurrencyHandler sender) => Currency.IsPossible(Amount, sender);
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