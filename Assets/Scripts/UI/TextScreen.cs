using SSPot.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SSpot.UI
{
    public class TextScreen : MonoBehaviour
    {
        [SerializeField] private Text uiText;
        [SerializeField] private GameObject hideObject;
        [SerializeField] private float showTime = 5f;
        [SerializeField] private bool startActive = false;
        
        private Coroutine _deactivateCoroutine;

        private void Awake() => hideObject.SetActive(startActive);

        public void ShowText(string text)
        {
            if (_deactivateCoroutine != null)
            {
                StopCoroutine(_deactivateCoroutine);
                _deactivateCoroutine = null;
            }
            
            uiText.text = text;
            hideObject.SetActive(true);
            
            _deactivateCoroutine = StartCoroutine(CoroutineUtilities.WaitThen(showTime, Close));
        }

        public void Close() => hideObject.SetActive(false);

        private void OnDisable()
        {
            if (_deactivateCoroutine != null)
            {
                StopCoroutine(_deactivateCoroutine);
                _deactivateCoroutine = null;
            }
        }
    }
}