using QG.Managers.Economy;
using QG.Managers.SaveSystem.Basic;
using System;
using System.Numerics;
using UnityEngine;

namespace QG.Managers.QuestSystem
{
    public class CurrencyAccumulationQuestCondition : QuestCondition
    {
        [SerializeField] Currency currencyToCollect;
        [SerializeField] int amountToCollect;
        private BigInteger startAmount;
        private bool _isStarted;
        public override float GetProgress()
        {
            if (!_isStarted) return 0;
            return (float)(currencyToCollect.GetAmount() - startAmount) / amountToCollect;
        }

        public override bool IsConditionMet()
        {
            if (!_isStarted) return false;
            return currencyToCollect.GetAmount() - startAmount >= amountToCollect;
        }

        public override void StartFollowing()
        {
            startAmount = currencyToCollect.GetAmount();
            _isStarted = true;
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
        public override void SaveProgress(string questID)
        {
            base.SaveProgress(questID);
            BasicSaveLoadManager.SetData(questID + "_Condition_StartAmount", startAmount);
        }
    }
}
