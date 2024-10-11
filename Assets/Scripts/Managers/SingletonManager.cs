using QuarterAsset.SoundSystem;
using UnityEngine;

namespace QuarterAsset
{
    abstract public class SingletonManager<T> : SingletonManager where T : SingletonManager<T>
    {
        static protected T Instance;
        override internal void Init()
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
            var manager = Resources.Load<T>("Managers/");
            manager.Init();
            return manager;
        }
    }
    abstract public class SingletonManager : MonoBehaviour
    {
        abstract public bool IsReady();
        abstract internal void Init();
    }
}