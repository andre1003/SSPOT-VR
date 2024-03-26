using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAudio : MonoBehaviour
{
    // Audio clips
    public AudioClip select;
    public AudioClip click;


    // Audio source
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Play UI select audio clip.
    /// </summary>
    public void PlaySelectSound()
    {
        // Only play if audio source is not playing or the clip is select
        if(!audioSource.isPlaying || audioSource.clip == select)
        {
            audioSource.clip = select;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Play UI click audio clip.
    /// </summary>
    public void PlayClickSound()
    {
        audioSource.clip = click;
        audioSource.Play();
    }
}
