using QG.Managers.SaveSystem.Basic;
using UnityEngine;

namespace QG.Managers.QuestSystem
{
    [System.Serializable]
    abstract public class QuestCondition
    {
        public System.Action OnConditionMet;
        public System.Action OnConditionProgressChanged;
        public abstract bool IsConditionMet();
        public abstract float GetProgress();
        public abstract void StartFollowing();
        virtual public void SaveProgress(string questID)
        {
            BasicSaveLoadManager.SetData(questID + "_Condition", GetProgress());
        }
    }
}
