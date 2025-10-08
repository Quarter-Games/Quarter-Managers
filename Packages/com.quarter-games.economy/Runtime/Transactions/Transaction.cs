namespace QG.Managers.Economy.Transactions
{
    [System.Serializable]
    abstract public class Transaction
    {
        abstract public void Execute(ICurrencyHandler sender, ICurrencyHandler reciever, bool SaveImediate = false);
        abstract public void ExecuteFirst(ICurrencyHandler sender, bool SaveImediate = false);
        abstract public void ExecuteSecond(ICurrencyHandler reciever, bool SaveImediate = false);
        abstract public string GetCostValue();
        abstract public bool IsPossible(ICurrencyHandler sender);
        /// <summary>
        /// Returns Currency To Currency Transaction that has same cost and gain values
        /// for the purpose of moving currency between different currency handlers
        /// </summary>
        /// <returns></returns>
        abstract public Transaction GetCTCTransaction();
    }
}