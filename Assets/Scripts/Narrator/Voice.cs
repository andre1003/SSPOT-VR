using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SSPot;

public class Voice : MonoBehaviour
{
	[SerializeField] private AudioSource source;
	private RectTransform[] audioIndicatorBars;

	[SerializeField] private GameObject subtitleBox;
	[SerializeField] private GameObject audioIndicator;
	[SerializeField] private Subtitles subtitles;

	public static Voice instance { get; private set; }

	private Queue<AudioObject[]> narrationQueue = new Queue<AudioObject[]>();
	private bool isPlaying = false;

	public delegate void NarrationRequestHandler(AudioObject[] clips, bool interrupt);
	public static event NarrationRequestHandler OnNarrationRequested;

	private bool enableNarrator = true;

	private AudioObject[] lastRequest;

	public void EnableNarrator()
	{
		OnNarrationRequested?.Invoke(lastRequest, true);
		enableNarrator = true;
	}

	public void DisableNarrator()
	{
		StopSpeaking();
		enableNarrator = false;
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
	}

	private void Start()
	{
		if (audioIndicator.GetComponentInChildren<RectTransform>() == null)
		{
			Debug.LogError("No RectTransform found in Audio Indicator");
			return;
		}
		else
		{
			audioIndicatorBars = audioIndicator.GetComponentsInChildren<RectTransform>();
			audioIndicatorBars = audioIndicatorBars[1..];

			RectTransform[] audioIndicatorBarsTemp = new RectTransform[audioIndicatorBars.Length];

			int left = 0;
			int right = 0;

			for(int i = audioIndicatorBars.Length - 1; i >= 0; i--) 
			{ 
				if (left <= right)
				{
					audioIndicatorBarsTemp[left] = audioIndicatorBars[i];
					left++;
				}
				else
				{
					audioIndicatorBarsTemp[audioIndicatorBars.Length - right - 1] = audioIndicatorBars[i];
					right++;
				}
			}

			for (int j = 0; j < audioIndicatorBars.Length; j++)
			{
				audioIndicatorBars[j] = audioIndicatorBarsTemp[j];
				print(audioIndicatorBars[j].name);
			}
		}
	}

	private void Update()
	{
		int sampleSize = 64;
		for (int j = 0; j < audioIndicatorBars.Length; j++)
		{
			float[] spectrumData = new float[sampleSize * (int) Mathf.Pow(2, j/2)];
			source.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

			float maxAmplitude = 0;
			for (int i = 0; i < sampleSize; i++)
			{
				if (spectrumData[i] > maxAmplitude)
				{
					maxAmplitude = spectrumData[i];
				}
			}

			audioIndicatorBars[j].sizeDelta = new Vector2(audioIndicatorBars[j].sizeDelta.x, Mathf.Lerp(audioIndicatorBars[j].sizeDelta.y, maxAmplitude * 500, Time.deltaTime * 10));
		}
	}

	private void OnEnable()
	{
		OnNarrationRequested += HandleNarrationRequest;
	}

	private void OnDisable()
	{
		OnNarrationRequested -= HandleNarrationRequest;
	}

	private void StopSpeaking()
	{
		narrationQueue.Clear();
		source.Stop();
		StopAllCoroutines();
		subtitles.ClearSubtitle();
		subtitleBox.SetActive(false);
		isPlaying = false;
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
				subtitles.ClearSubtitle();
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
			subtitleBox.SetActive(true);
			AudioObject[] currentClips = narrationQueue.Dequeue();
			foreach (AudioObject clip in currentClips)
			{
				source.clip = clip.clip;
				source.PlayOneShot(clip.clip);
				subtitles.DisplaySubtitle(clip.subtitle);
				yield return new WaitForSeconds(clip.clip.length);
				subtitles.ClearSubtitle();
			}
			subtitleBox.SetActive(false);
		}
		isPlaying = false;
	}

	public void Speak(AudioObject[] clips)
	{
		lastRequest = clips;
		if (enableNarrator) OnNarrationRequested?.Invoke(clips, true);
	}
}