using JetBrains.Annotations;
using UnityEngine;

namespace SSPot.Utilities
{
    public class DelayedDestroy : MonoBehaviour
    {
        [SerializeField] private bool destroyOnEnable = true;
        
        public float DelaySeconds;
        
        [UsedImplicitly]
        public void DestroySelf() => Destroy(gameObject, DelaySeconds);

        private void OnEnable()
        {
            if (destroyOnEnable)
                DestroySelf();
        }
    }
}