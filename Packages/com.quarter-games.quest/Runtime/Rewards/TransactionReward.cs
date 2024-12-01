using QG.Managers.Economy.Transactions;
using UnityEngine;

namespace QG.Managers.QuestSystem
{
    public class TransactionReward : QuestReward
    {
        public RewardTransaction reward;
        public override void GiveReward()
        {
            reward.Execute(null,null);
        }
    }
}
