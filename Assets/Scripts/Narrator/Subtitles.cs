using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace SSPot
{
    public class Subtitles : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI subtitleText = default;
		public static Subtitles instance;

		private void Awake()
		{
			instance = this;
			ClearSubtitle();
		}

		public void DisplaySubtitle(string text)
		{
			subtitleText.text = text;
			//StartCoroutine(ClearAfterSeconds(delay));

			//return currentText;
		}

		public void ClearSubtitle()
		{
			subtitleText.text = "";
		}

		//private IEnumerator ClearAfterSeconds(float delay)
		//{
		//	yield return new WaitForSeconds(delay);
		//	ClearSubtitle();
		//}
	}
}
