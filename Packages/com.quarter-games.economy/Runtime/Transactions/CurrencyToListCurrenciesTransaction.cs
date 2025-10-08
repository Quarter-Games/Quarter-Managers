using QG.Managers.Economy;
using QG.Managers.Economy.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class CurrencyToListCurrenciesTransaction : Transaction
{
    public Currency ReducedCurrency;

    public SerializedBigInteger Cost;

    public List<CurrencyAndCost> CurrenciesAndCosts;
    public override void Execute(ICurrencyHandler sender, ICurrencyHandler reciever, bool SaveImediate = false)
    {
        if (!IsPossible(sender)) return;
        ReducedCurrency.Decrement(Cost, sender, SaveImediate);

        foreach (var currencyAndCost in CurrenciesAndCosts)
        {
            currencyAndCost.Currency.Increment(currencyAndCost.Cost, reciever, SaveImediate);
        }
    }

    public override void ExecuteFirst(ICurrencyHandler sender, bool SaveImediate = false)
    {
        if (!IsPossible(sender)) return;
        ReducedCurrency.Decrement(Cost, sender, SaveImediate);
    }

    public override void ExecuteSecond(ICurrencyHandler reciever, bool SaveImediate = false)
    {
        foreach (var currencyAndCost in CurrenciesAndCosts)
        {
            currencyAndCost.Currency.Increment(currencyAndCost.Cost, reciever, SaveImediate);
        }
    }
    public override string GetCostValue()
    {
        return new CurrencyValue(Cost).GetStringValue();
    }

    public override bool IsPossible(ICurrencyHandler sender)
    {
        return ReducedCurrency.IsPossible(-Cost.ActualValue, sender) && CurrenciesAndCosts.All(x => x.Currency.IsPossible(Cost.ActualValue, sender));
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
