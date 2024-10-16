using System.Collections.Generic;
using UnityEngine;

namespace QG.Managers.SaveSystem.File
{
    [CreateAssetMenu(fileName = "VisitorList", menuName = "Quarter Asset/Save System/VisitorList")]
    public class VisitorList : ScriptableObject
    {
        public List<SaveLoadVisitor> Visitors;
    }
}