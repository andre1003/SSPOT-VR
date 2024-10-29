using JetBrains.Annotations;
using UnityEngine;

namespace SSpot.Utilities
{
    public class DelayedDestroy : MonoBehaviour
    {
        public float DelaySeconds;
        
        [UsedImplicitly]
        public void DestroySelf() => Destroy(gameObject, DelaySeconds);
    }
}