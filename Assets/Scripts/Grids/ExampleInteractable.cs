using UnityEngine;

namespace SSpot.Grids
{
    public class ExampleInteractable : MonoBehaviour
    {
        [field: SerializeField] 
        public AnimationClip InteractionAnimation { get; private set; }
        
        [field: SerializeField] 
        public string Value { get; private set; }
    }
}