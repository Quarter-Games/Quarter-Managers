using QG.Managers.Economy;
using QG.Managers.Economy.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

public class CurrenciesListToCurrencyTransaction : Transaction
{
    public List<CurrencyAndCost> ReducedCurrencies;


    public Currency Reward;
    public SerializedBigInteger Gain;
    public override void Execute(ICurrencyHandler sender, ICurrencyHandler reciever, bool SaveImediate = false)
    {
        if (!IsPossible(sender)) return;
        foreach (var currency in ReducedCurrencies)
        {

            currency.Currency.Decrement(currency.Cost, sender, SaveImediate);
        }

        Reward.Increment(Gain, reciever, SaveImediate);
    }

    public override void ExecuteFirst(ICurrencyHandler sender, bool SaveImediate = false)
    {
        if (!IsPossible(sender)) return;
        foreach (var currency in ReducedCurrencies)
        {
            currency.Currency.Decrement(currency.Cost, sender, SaveImediate);
        }
    }

    public override void ExecuteSecond(ICurrencyHandler reciever, bool SaveImediate = false)
    {
        Reward.Increment(Gain, reciever, SaveImediate);
    }
    public override string GetCostValue()
    {
        return new CurrencyValue(ReducedCurrencies[0].Cost).GetStringValue();
    }

    public override bool IsPossible(ICurrencyHandler sender)
    {

        return ReducedCurrencies.All(x=>x.Currency.IsPossible(-x.Cost.ActualValue, sender) && Reward.IsPossible(Gain,sender));
    }
    public override Transaction GetCTCTransaction()
    {
        return new CurrenciesListToCurrencyTransaction
        {
            Gain = Gain,
            Reward = Reward,
            ReducedCurrencies = ReducedCurrencies
        };
    }
    [Serializable]
    public struct CurrencyAndCost
    {
        public Currency Currency; public SerializedBigInteger Cost;
    }
}
