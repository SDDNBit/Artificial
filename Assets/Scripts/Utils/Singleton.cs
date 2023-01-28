using UnityEngine;

namespace SoftBit.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance
        {
            get
            {
                if(instance != null)
                {
                    return instance;
                }
                var newInstance = new GameObject(typeof(T).ToString(), typeof(T));
                instance = newInstance.GetComponentInChildren<T>();
                
                DontDestroyOnLoad(newInstance);
                
                return instance;
            }
        }

        private static T instance;

        protected virtual void Awake()
        {
            if(instance == null)
            {
                instance = (T)this;
            }
            else
            {
                if(instance != (T)this)
                {
                    Destroy(this);
                }
            }
        }
    }
}
