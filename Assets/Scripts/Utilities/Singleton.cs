using UnityEngine;

namespace SSpot.Utilities
{
    public abstract class Singleton<T> : MonoBehaviour where T: Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindObjectOfType<T>();
                
                return _instance;
            }
        }
        
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
        }
    }
}