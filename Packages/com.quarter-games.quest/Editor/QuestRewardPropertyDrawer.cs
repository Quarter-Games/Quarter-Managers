namespace QG.Managers.QuestSystem.Editor
{
    using UnityEngine;
    using QG.Managers.Editor;
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