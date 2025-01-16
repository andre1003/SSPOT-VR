using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSPot
{
    public class Voice : MonoBehaviour
    {
        [SerializeField] AudioSource source;
        public static Voice instance;

		private void Awake()
		{
			instance = this;
		}

		public void Speak(AudioObject[] clips)
		{
			if (source.isPlaying)
			{
				source.Stop();
				StopAllCoroutines();
			}

			StartCoroutine(Lines(clips));
		}

		private IEnumerator Lines(AudioObject[] clips)
		{
			foreach (AudioObject clip in clips)
			{
				source.clip = clip.clip;
				source.PlayOneShot(clip.clip);
				Subtitles.instance.DisplaySubtitle(clip.subtitle);

				yield return new WaitForSeconds(clip.clip.length);

				Subtitles.instance.ClearSubtitle();
			}
		}
	}
}
