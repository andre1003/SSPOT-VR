using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace SSPot
{
    public class Subtitles : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI subtitleText = default;

		public void DisplaySubtitle(string text)
		{
			subtitleText.text = text;
		}

		public void ClearSubtitle()
		{
			subtitleText.text = "";
		}
	}
}
