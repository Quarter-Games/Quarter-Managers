using QG.Managers.Economy;
using QG.Managers.Economy.Transactions;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CurrencyToListCurrenciesTransaction : Transaction
{
    public Currency ReducedCurrency;

    public SerializedBigInteger Cost;

    public List<CurrencyAndCost> CurrenciesAndCosts;
    public override void Execute(ICurrencyHandler sender, ICurrencyHandler reciever)
    {
        if (!IsPossible(sender)) return;
        ReducedCurrency.Decrement(Cost, sender);

        foreach (var currencyAndCost in CurrenciesAndCosts)
        {
            currencyAndCost.Currency.Increment(currencyAndCost.Cost, reciever);
        }
    }

    public override void ExecuteFirst(ICurrencyHandler sender)
    {
        if (!IsPossible(sender)) return;
        ReducedCurrency.Decrement(Cost, sender);
    }

    public override void ExecuteSecond(ICurrencyHandler reciever)
    {
        foreach (var currencyAndCost in CurrenciesAndCosts)
        {
            currencyAndCost.Currency.Increment(currencyAndCost.Cost, reciever);
        }
    }
    public override string GetCostValue()
    {
        return new CurrencyValue(Cost).GetStringValue();
    }

    public override bool IsPossible(ICurrencyHandler sender)
    {
        foreach (var currencyAndCost in CurrenciesAndCosts)
        {
            if (ReducedCurrency.GetAmount(sender) < currencyAndCost.Cost)
            {
                return false;
            }
        }
        return true;
    }
    public override Transaction GetCTCTransaction()
    {
        return new CurrencyToCurrencyTransaction
        {
            Cost = Cost,
            ReducedCurrency = ReducedCurrency,
            GainedCurrency = ReducedCurrency,
            Gain = Cost
        };
    }
    [Serializable]
    public struct CurrencyAndCost
    {
        public Currency Currency; public SerializedBigInteger Cost;
    }
}
