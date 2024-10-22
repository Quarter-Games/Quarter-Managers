using QG.Managers.Economy;
using System;
using UnityEngine;

namespace QG.Managers.QuestSystem
{
    public class CurrencyAmountQuestCondition : QuestCondition
    {
        public Currency currencyToCollect;
        public int amountToCollect;
        public override float GetProgress()
        {
            return currencyToCollect.GetAmount() / (float)amountToCollect;
        }

        public override bool IsConditionMet()
        {
            return currencyToCollect.GetAmount() >= amountToCollect;
        }

        public override void StartFollowing()
        {
            currencyToCollect.OnCurrencyChanged += OnCurrencyAmountChanged;
        }

        private void OnCurrencyAmountChanged(Currency currency, float arg2)
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
