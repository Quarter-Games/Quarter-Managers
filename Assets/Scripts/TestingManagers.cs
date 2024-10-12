using QG.Managers.Economy.Transactions;
using UnityEngine;

public class TestingManagers : MonoBehaviour
{
    [ContextMenuItem("C2C", "CurrencyToCurrency")]
    [ContextMenuItem("C2A", "CurrencyToGameAction")]
    [ContextMenuItem("Reward", "Reward")]
    [SerializeReference]
    public Transaction transaction = new CurrencyToCurrencyTransaction();


    public void CurrencyToCurrency()
    {
        transaction = new QG.Managers.Economy.Transactions.CurrencyToCurrencyTransaction();

    }
    public void CurrencyToGameAction()
    {
        transaction = new QG.Managers.Economy.Transactions.CurrencyToGameActionTransaction();

    }
    public void Reward()
    {
        transaction = new QG.Managers.Economy.Transactions.RewardTransaction();
    }

}
