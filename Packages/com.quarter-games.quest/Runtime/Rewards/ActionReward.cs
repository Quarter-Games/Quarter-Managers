using UnityEngine;
using UnityEngine.Events;

namespace QG.Managers.QuestSystem
{
    public class ActionReward : QuestReward
    {
        public UnityEvent reward;
        public override void GiveReward()
        {
            reward.Invoke();
        }
    }
}
