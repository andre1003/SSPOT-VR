using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSPot
{
    public class SpeakAtStart : MonoBehaviour
    {
		[SerializeField] AudioObject[] clipsP1;
		[SerializeField] AudioObject[] clipsP2;
		public static SpeakAtStart instance;

		public void Awake()
		{
			if(instance != null)
			{
				Destroy(this);
			}

			instance = this;
		}

		public void Start()
		{
			StartSpeaking();
		}

		public void StartSpeaking()
        {
			if (PhotonNetwork.IsMasterClient)
				Voice.instance.Speak(clipsP1);
			else
				Voice.instance.Speak(clipsP2);
		}
	}
}
