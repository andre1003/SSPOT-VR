using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSPot
{
    public class DebugConsole : MonoBehaviour
    {
        public static DebugConsole instace;

		[SerializeField] RectTransform displayRect;
		[SerializeField] Text displayText;

		float initHeight;

		private void Awake()
		{
			if(DebugConsole.instace != null)
				DestroyImmediate(gameObject);
			else DebugConsole.instace = this;

			initHeight = displayRect.anchoredPosition.y;
		}

		public void ChangeDisplayPos(float newPos)
		{
			displayRect.anchoredPosition = new Vector2(displayRect.anchoredPosition.x, initHeight + newPos);
		}
	}
}
