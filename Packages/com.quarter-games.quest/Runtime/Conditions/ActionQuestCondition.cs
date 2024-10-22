using System;
using UnityEngine;
using UnityEngine.Events;

namespace QG.Managers.QuestSystem
{
    public class ActionQuestCondition : QuestCondition
    {
        private bool _isConditionMet;
        public UnityEvent Condition;

        public override float GetProgress()
        {
            return _isConditionMet ? 1 : 0;
        }
        public override bool IsConditionMet()
        {
            return _isConditionMet;
        }

        public override void StartFollowing()
        {
            Condition.AddListener(OnCondition);
        }

        private void OnCondition()
        {
            _isConditionMet = true;
            OnConditionMet?.Invoke();
            OnConditionProgressChanged?.Invoke();
            Condition.RemoveListener(OnCondition);
        }
    }
}
