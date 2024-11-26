using QG.Managers.Economy;
using QG.Managers.Economy.Transactions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyToListCurrenciesTransaction : Transaction
{
    public Currency ReducedCurrency;

    public int Cost;

    public List<CurrencyAndCost> CurrenciesAndCosts;
    public override void Execute()
    {
        if (ReducedCurrency.GetAmount() >= Cost)
        {
            ReducedCurrency.Decrement(Cost);

            foreach (var currencyAndCost in CurrenciesAndCosts)
            {
                currencyAndCost.Currency.Increment(currencyAndCost.Cost);
            }
        }
    }

    public override void ExecuteFirst()
    {
        if (ReducedCurrency.GetAmount() < Cost) return;
        ReducedCurrency.Decrement(Cost);
    }

    public override void ExecuteSecond()
    {
        foreach (var currencyAndCost in CurrenciesAndCosts)
        {
            currencyAndCost.Currency.Increment(currencyAndCost.Cost);
        }
    }

    public void ExecuteSecondPart()
    {
        foreach (var currencyAndCost in CurrenciesAndCosts)
        {
            currencyAndCost.Currency.Increment(currencyAndCost.Cost);
        } 
    }

    public override string GetCostValue()
    {
        return new CurrencyValue(Cost).GetStringValue();
    }

    public override bool IsPossible()
    {
        foreach (var currencyAndCost in CurrenciesAndCosts)
        {
            if (ReducedCurrency.GetAmount() < currencyAndCost.Cost)
            {
                return false;
            }
        }
        return true;
    }
    [Serializable]
    public struct CurrencyAndCost
    {
        public Currency Currency; public int Cost;
    }
}
