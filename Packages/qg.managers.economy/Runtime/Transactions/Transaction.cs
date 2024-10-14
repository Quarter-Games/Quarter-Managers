namespace QG.Managers.Economy.Transactions
{
    [System.Serializable]
    abstract public class Transaction
    {
        abstract public void Execute();
        abstract public bool IsPossible();
    }
}