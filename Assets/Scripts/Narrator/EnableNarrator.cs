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

		[SerializeField] GameObject gameObject;

		public void OnPointerClick()
		{
			if (isNarratorEnabled)
			{
				gameObject.SetActive(false);
				Voice.instance.DisableNarrator();
				isNarratorEnabled = false;
			}
			else
			{
				gameObject.SetActive(true);
				Voice.instance.EnableNarrator();
				isNarratorEnabled = true;
			}
		}
	}
}
