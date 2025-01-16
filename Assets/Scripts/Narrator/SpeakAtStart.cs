using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSPot
{
    public class SpeakAtStart : MonoBehaviour
    {
        [SerializeField] AudioObject[] clips;

		void Start()
        {
			Voice.instance.Speak(clips);
		}
	}
}
