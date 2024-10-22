using QG.Managers.QuestSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QG.Managers.QuestSystem
{
    [System.Serializable]
    public class QuestList
    {
        public int MaxConcurrentQuests;
        [SerializeField]
        private List<Quest> QuestOrder;
        [SerializeField]
        private bool isRandom;
        public bool IsCompleted()
        {
            if (QuestOrder == null)
            {
                Debug.LogError("QuestOrder is null");
                return true;
            }
            if (QuestOrder.Count == 0)
            {
                Debug.LogError("QuestOrder is empty");
                return true;
            }
            for (int i = 0; i < QuestOrder.Count; i++)
            {
                Quest quest = QuestOrder[i];
                if (quest == null)
                {
                    Debug.LogError($"Quest {i} is null");
                    continue;
                }
                if (quest.GetStatus() != QuestStatus.Completed)
                {
                    return false;
                }
            }
            return true;
        }
        public float GetProgress()
        {
            if (QuestOrder == null)
            {
                Debug.LogError("QuestOrder is null");
                return 0;
            }
            if (QuestOrder.Count == 0)
            {
                Debug.LogError("QuestOrder is empty");
                return 0;
            }
            if (QuestOrder.Any(x => x == null))
            {
                Debug.LogError("QuestOrder contains null elements");
                return 0;
            }
            return QuestOrder.Average(x => x.GetProgress());
        }
        public List<Quest> GetActiveQuests()
        {
            if (MaxConcurrentQuests == 0)
            {
                MaxConcurrentQuests = QuestOrder.Count;
            }
            List<Quest> quests;
            if (isRandom)
            {
                quests = QuestOrder.OrderBy(x => Random.value).ToList();
                quests.GetRange(0, Mathf.Max(MaxConcurrentQuests, quests.Count));

            }
            quests = QuestOrder.Where(x => x.GetStatus() != QuestStatus.Completed).ToList().GetRange(0, Mathf.Max(MaxConcurrentQuests, QuestOrder.Count));

            return quests;
        }
        
    }
}
