using UnityEngine;

namespace QuarterAsset.Economy.Transactions
{
    [System.Serializable]
    abstract public class Transaction
    {
        abstract public void Execute();
        abstract public bool IsPossible();
    }
}