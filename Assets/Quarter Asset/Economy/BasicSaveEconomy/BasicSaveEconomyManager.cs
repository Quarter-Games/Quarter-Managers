using System.Threading.Tasks;
using UnityEngine;

public class BasicSaveEconomyManager : EconomyManager
{
    public override void Init()
    {
        base.Init();
        if (CurrencyList == null)
        {
            Debug.LogError("Currency List is not set in the Economy Manager");
        }
        foreach (var currency in CurrencyList.GameCurrencies)
        {
            if (currency.currencyID == "")
            {
                Debug.LogError("Currency ID is not set for " + currency.currencyName);
            }
            else
            {
                var amount = GetAmount(currency).Result;
                Debug.Log("Currency: " + currency.currencyName + " Amount: " + amount);
            }
        }

    }
    async public override Task Decrement(Currency currency, int amount)
    {
        BasicSaveLoadManager.SetData(currency.currencyID, await GetAmount(currency) - amount);
    }

    async public override Task<int> GetAmount(Currency currency)
    {
        return await BasicSaveLoadManager.GetData(currency.currencyID, currency.StartingAmount);
    }

    async public override Task Increment(Currency currency, int amount)
    {
        BasicSaveLoadManager.SetData(currency.currencyID, await GetAmount(currency) + amount);
    }

    public override bool IsReady() => true;
}