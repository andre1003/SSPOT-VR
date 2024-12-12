using UnityEngine;
using UnityEngine.Events;

namespace SSpot.UI
{
    public class PointerButton : MonoBehaviour
    {
        [field: SerializeField]
        public UnityEvent OnPointerClickEvent { get; private set; } = new();
        
        public void OnPointerClick() => OnPointerClickEvent.Invoke();
    }
}