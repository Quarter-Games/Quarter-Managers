using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "VisitorList", menuName = "Quarter Asset/Save System/VisitorList")]
public class VisitorList : ScriptableObject
{
    public List<SaveLoadVisitor> Visitors;
}