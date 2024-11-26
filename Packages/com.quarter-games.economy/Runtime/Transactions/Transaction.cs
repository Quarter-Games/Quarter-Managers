namespace QG.Managers.Economy.Transactions
{
    [System.Serializable]
    abstract public class Transaction
    {
        abstract public void Execute();
        abstract public void ExecuteFirst();
        abstract public void ExecuteSecond();
        abstract public string GetCostValue();
        abstract public bool IsPossible();
    }
}