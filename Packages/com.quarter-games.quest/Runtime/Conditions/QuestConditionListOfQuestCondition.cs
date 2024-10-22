using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QG.Managers.QuestSystem
{
    public class QuestConditionListOfQuestCondition : QuestCondition
    {
        [SerializeReference]
        public List<QuestCondition> Conditions;
        public override float GetProgress()
        {
            if (Conditions == null)
            {
                Debug.LogError("Conditions list is null");
                return 0;
            }
            if (Conditions.Count == 0)
            {
                Debug.LogError("Conditions list is empty");
                return 0;
            }
            if (Conditions.Any(Condition => Condition == null))
            {
                Debug.LogError("Conditions list contains null elements");
                return 0;
            }
            return Conditions.Average(Condition => Condition.GetProgress());
        }

        public override bool IsConditionMet()
        {
            if (Conditions == null)
            {
                Debug.LogError("Conditions list is null");
                return false;
            }
            if (Conditions.Count == 0)
            {
                Debug.LogError("Conditions list is empty");
                return false;
            }
            if (Conditions.Any(Condition => Condition == null))
            {
                Debug.LogError("Conditions list contains null elements");
                return false;
            }
            return Conditions.All(Condition => Condition.IsConditionMet());
        }

        public override void StartFollowing()
        {
            if (Conditions == null)
            {
                Debug.LogError("Conditions list is null");
                return;
            }
            if (Conditions.Count == 0)
            {
                Debug.LogError("Conditions list is empty");
                return;
            }
            if (Conditions.Any(Condition => Condition == null))
            {
                Debug.LogError("Conditions list contains null elements");
                return;
            }
            foreach (var Condition in Conditions)
            {
                Condition.OnConditionMet += OnListConditionMet;
                Condition.OnConditionProgressChanged += OnConditionProgressChanged;
                Condition.StartFollowing();
            }
        }

        private void OnListConditionMet()
        {
            if (IsConditionMet())
            {
                OnConditionMet?.Invoke();
            }
        }
        public override void SaveProgress(string questID)
        {
            base.SaveProgress(questID);
            for (int i = 0; i < Conditions.Count; i++)
            {
                Conditions[i].SaveProgress(questID + "_Condition_" + i);
            }
        }
    }
}
