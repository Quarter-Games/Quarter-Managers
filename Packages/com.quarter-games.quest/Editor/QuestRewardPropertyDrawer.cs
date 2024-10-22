using UnityEngine;
namespace QG.Managers.QuestSystem.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;

    [CustomPropertyDrawer(typeof(QuestReward))]
    public class QuestRewardPropertyDrawer : GenericPropertyDrawer<QuestReward>
    {
    }
}