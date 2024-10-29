using SSpot.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SSpot.UI
{
    public class TextScreen : MonoBehaviour
    {
        [SerializeField] private Text uiText;
        [SerializeField] private float showTime = 5f;
        
        private Coroutine _deactivateCoroutine;
        
        public void ShowText(string text)
        {
            if (_deactivateCoroutine != null)
            {
                StopCoroutine(_deactivateCoroutine);
                _deactivateCoroutine = null;
            }
            
            uiText.text = text;
            gameObject.SetActive(true);
            
            _deactivateCoroutine = StartCoroutine(CoroutineUtilities.WaitThen(showTime, Close));
        }

        public void Close()
        {
            if (_deactivateCoroutine != null)
            {
                StopCoroutine(_deactivateCoroutine);
                _deactivateCoroutine = null;
            }
            
            gameObject.SetActive(false);
        }
    }
}