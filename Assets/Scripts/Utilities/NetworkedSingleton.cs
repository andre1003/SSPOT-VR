using Photon.Pun;

namespace SSPot.Utilities
{
    public abstract class NetworkedSingleton<T> : MonoBehaviourPun where T: NetworkedSingleton<T>
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
        
        protected void Awake()
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