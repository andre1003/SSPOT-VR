using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace SSPot
{
    public class EnableNarrator : MonoBehaviour
    {
		bool isNarratorEnabled = true;

		[SerializeField] GameObject disableText;
		[SerializeField] GameObject enableText;

		public void OnPointerClick()
		{
			if (isNarratorEnabled)
			{
				disableText.SetActive(false);
				enableText.SetActive(true);
				Voice.instance.DisableNarrator();
				isNarratorEnabled = false;
			}
			else
			{
				disableText.SetActive(true);
				enableText.SetActive(false);
				Voice.instance.EnableNarrator();
				isNarratorEnabled = true;
			}
		}
	}
}
