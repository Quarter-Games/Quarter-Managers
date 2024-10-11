using UnityEngine;

namespace QuarterAsset
{
    abstract public class SingletonManager<T> : SingletonManager where T : SingletonManager<T>
    {
        static protected T Instance;
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (T)this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    abstract public class SingletonManager : MonoBehaviour
    {
        abstract public bool IsReady();
    }
}