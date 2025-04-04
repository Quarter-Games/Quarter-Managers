using UnityEngine;

namespace QG.Managers
{
    abstract public class SingletonManager<T> : SingletonManager where T : SingletonManager<T>
    {
        static protected T Instance;
        override public void Init()
        {

            if (Instance == null)
            {
                Instance = (T)this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        static protected T LoadFallBackManager()
        {
            var manager = Instantiate(Resources.LoadAll<T>("Managers")[0]);
            
            manager.Init();
            return manager;
        }
    }
    abstract public class SingletonManager : MonoBehaviour
    {
        abstract public bool IsReady();
        abstract public void Init();
    }
}