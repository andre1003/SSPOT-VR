using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SSPot;

public class Voice : MonoBehaviour
{
	[SerializeField] private AudioSource source;
	public static Voice instance { get; private set; }

	private Queue<AudioObject[]> narrationQueue = new Queue<AudioObject[]>();
	private bool isPlaying = false;

	public delegate void NarrationRequestHandler(AudioObject[] clips, bool interrupt);
	public static event NarrationRequestHandler OnNarrationRequested;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
	}

	private void OnEnable()
	{
		OnNarrationRequested += HandleNarrationRequest;
	}

	private void OnDisable()
	{
		OnNarrationRequested -= HandleNarrationRequest;
	}

	private void HandleNarrationRequest(AudioObject[] clips, bool interrupt)
	{
		if (interrupt)
		{
			narrationQueue.Clear();
			if (isPlaying)
			{
				source.Stop();
				StopAllCoroutines();
				Subtitles.instance.ClearSubtitle();
				isPlaying = false;
			}
		}

		narrationQueue.Enqueue(clips);

		if (!isPlaying)
		{
			StartCoroutine(ProcessNarrationQueue());
		}
	}

	private IEnumerator ProcessNarrationQueue()
	{
		isPlaying = true;
		while (narrationQueue.Count > 0)
		{
			AudioObject[] currentClips = narrationQueue.Dequeue();
			foreach (AudioObject clip in currentClips)
			{
				source.clip = clip.clip;
				source.PlayOneShot(clip.clip);
				Subtitles.instance.DisplaySubtitle(clip.subtitle);
				yield return new WaitForSeconds(clip.clip.length);
				Subtitles.instance.ClearSubtitle();
			}
		}
		isPlaying = false;
	}

	// Optional: Maintain original Speak method as an interruptible request
	public void Speak(AudioObject[] clips)
	{
		OnNarrationRequested?.Invoke(clips, true);
	}
}