using System;
using UnityEngine;

namespace QG.Managers.QuestSystem
{
    [Serializable]
    public class Quest
    {
        public event Action<Quest> OnQuestComplete;
        public event Action<Quest> OnQuestProgressChange;
        public string ID;
        [SerializeField]
        protected string title;
        public string Title { get; }
        [SerializeField]
        protected string description;
        public string Description { get; }
        [SerializeField]
        protected Sprite sprite;
        public Sprite Sprite { get; }

        [SerializeReference]
        public QuestCondition Condition;

        [SerializeReference]
        public QuestReward reward;
        public bool CheckCompletion()
        {
            if (Condition == null)
            {
                Debug.LogWarning("Condition is null");
                return false;
            }
            if (Condition.IsConditionMet())
            {
                Complete();
                return true;
            }
            return false;
        }
        virtual public QuestStatus GetStatus()
        {
            return SaveSystem.Basic.BasicSaveLoadManager.GetData(ID, QuestStatus.NotStarted).Result;
        }
        virtual public void StartQuest()
        {
            if (GetStatus() == QuestStatus.NotStarted)
            {
                if (Condition == null)
                {
                    Debug.LogError("Condition is null");
                    return;
                }
                Condition.OnConditionProgressChanged += ConditionProgressChange;
                Condition.OnConditionMet += ConditionComplete;
                SaveSystem.Basic.BasicSaveLoadManager.SetData(ID, QuestStatus.Active);
            }
            else
            {
                Debug.LogWarning("Quest already started or completed " + ID);
            }
        }

        private void ConditionComplete()
        {
            Complete();
        }

        private void ConditionProgressChange()
        {
            OnQuestProgressChange?.Invoke(this);
        }

        public float GetProgress() { return 1; }
        virtual public void SaveProgress()
        {
            SaveSystem.Basic.BasicSaveLoadManager.SetData(ID, GetStatus());
            Condition.SaveProgress(ID);
        }
        virtual public void Complete()
        {
            SaveSystem.Basic.BasicSaveLoadManager.SetData(ID, QuestStatus.Completed);
            reward.GiveReward();
            Condition.OnConditionProgressChanged -= ConditionProgressChange;
            Condition.OnConditionMet -= ConditionComplete;
            OnQuestComplete?.Invoke(this);
        }
    }
    public enum QuestStatus
    {
        NotStarted,
        Active,
        Completed
    }
}
