using UnityEngine;

namespace QG.Managers
{

    static public class Debugger
    {
        public static DebuggerSettings Settings { get; private set; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Init()
        {
            Settings = Resources.Load<DebuggerSettings>("Debugger/DebuggerSettings");
        }
        public static void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
