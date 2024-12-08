using SSpot.AnimatorUtilities;
using UnityEngine;

namespace SSpot.Grids
{
    public class ExampleInteractable : MonoBehaviour
    {
        [field: SerializeField, RobotAnimatorStateName] 
        public HashedString InteractionAnimation { get; private set; }
        
        [field: SerializeField] 
        public string Value { get; private set; }
    }
}