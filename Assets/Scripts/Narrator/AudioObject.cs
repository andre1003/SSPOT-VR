using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSPot
{
    [CreateAssetMenu(fileName = "AudioObject", menuName = "SSPot/New AudioObject", order = 1)]
	public class AudioObject : ScriptableObject
    {
        public AudioClip clip;
        public string subtitle;
	}
}
