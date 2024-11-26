using System;
using UnityEngine;

namespace QG.Managers
{
    abstract public class DebugVisitor<T>
    {
        [ColorUsage(false)] public Color color;
        public bool isEnabled;
    }
}
