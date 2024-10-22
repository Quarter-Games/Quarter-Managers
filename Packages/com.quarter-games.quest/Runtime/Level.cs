using UnityEngine;

namespace QG.Managers.QuestSystem
{
    [CreateAssetMenu(fileName = "Level", menuName = "Quarter Asset/Quest System/Level")]
    public class Level : ScriptableObject
    {
        public QuestList quests;
    }
}
