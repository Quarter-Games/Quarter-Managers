using QG.Managers.Economy;
using System;
using System.Numerics;
using UnityEngine;

namespace QG.Managers.QuestSystem
{
    public class CurrencyAmountQuestCondition : QuestCondition
    {
        public Currency currencyToCollect;
        public BigInteger amountToCollect;
        public override float GetProgress()
        {
            return (float)currencyToCollect.GetAmount() / (float)amountToCollect;
        }

        public override bool IsConditionMet()
        {
            return currencyToCollect.GetAmount() >= amountToCollect;
        }

        public override void StartFollowing()
        {
            currencyToCollect.OnCurrencyChanged += OnCurrencyAmountChanged;
        }

        private void OnCurrencyAmountChanged(Currency currency, BigInteger arg2)
        {
            OnConditionProgressChanged?.Invoke();
            if (IsConditionMet())
            {
                currencyToCollect.OnCurrencyChanged -= OnCurrencyAmountChanged;
                OnConditionMet?.Invoke();
            }
        }
    }
}
