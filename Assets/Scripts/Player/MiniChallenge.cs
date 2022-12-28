using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MiniChallenge : MonoBehaviourPun
{
    #region Singleton
    public static MiniChallenge instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    #region Attributes
    #region Public
    [Space]
    [Header("Challenge Information")]
    [Space]

    // Mini challenge config
    public float limitTime = 0f;
    public int maxErrors = 3;


    [Space]
    [Header("Challenge UI")]
    [Space]

    // Timer text reference
    public Text timerText;
    public Text errorsText;

    // Goal fail text color
    public Color failColor;
    

    [Space]
    [Header("Challenge Result UI")]
    [Space]

    // Challenge result canvas
    public GameObject challengeResultCanvas;

    // Challenge stars
    public List<GameObject> stars;


    [Space]
    [Header("Challenge Audio")]
    [Space]

    public AudioClip newStarAudioClip;
    public AudioClip challengeFinishedAudioClip;
    #endregion


    #region Private
    // Timer value
    private float timer = 0f;

    // Current number of errors
    private int errors = 0;

    // Challenge completition controllers
    private bool isTimeChallengeComplete = true;
    private bool isErrorsChallengeComplete = true;

    private AudioSource audioSource;
    #endregion
    #endregion

    #region Methods
    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        // Get audio source component and set it's clip to newStarAudioClip
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = newStarAudioClip;

        // If this player is master client, start timer synced
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartTimerRpc", RpcTarget.AllBuffered);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If this player is master client, increase the timer for all players
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("IncreaseTimerRpc", RpcTarget.AllBuffered, Time.deltaTime);
        }

        // Update UI
        UpdateUI();
    }
    #endregion

    #region Time Challenge
    /// <summary>
    /// Start timer via RPC.
    /// </summary>
    [PunRPC]
    private void StartTimerRpc()
    {
        timer = 0f;
    }

    /// <summary>
    /// Increase timer via RPC.
    /// </summary>
    /// <param name="timeToAdd">Time to add. <para>Usually Timer.deltaTime.</para></param>
    [PunRPC]
    private void IncreaseTimerRpc(float timeToAdd)
    {
        // Add time to timer
        timer += timeToAdd;

        // If timer value exceds the mini challenge limit,
        // set isTimeChallengeComplete to false and timer text color to failColor
        if(timer >= limitTime)
        {
            isTimeChallengeComplete = false;
            timerText.color = failColor;
        }
    }
    #endregion

    #region Error Challenge
    /// <summary>
    /// Incremente errors.
    /// </summary>
    public void IncreaseError()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("IncreaseErrorRpc", RpcTarget.AllBuffered);
        }
    }

    /// <summary>
    /// Increment errors via RPC.
    /// </summary>
    [PunRPC]
    private void IncreaseErrorRpc()
    {
        // Increment errors
        errors++;

        // If errors is bigger or equal maximum errors,
        // set isErrorsChallengeComplete to false and errors text color to failColor
        if(errors >= maxErrors)
        {
            isErrorsChallengeComplete = false;
            errorsText.color = failColor;
        }
    }
    #endregion

    #region UI
    /// <summary>
    /// Update challenge UI.
    /// </summary>
    private void UpdateUI()
    {
        // Get rounded timer value
        int roundedTimer = Mathf.RoundToInt(timer);

        // Get timer prefix
        string prefix = "Time: ";
        prefix += roundedTimer < 10 ? "0" : "";

        // Set timer text
        timerText.text = prefix + roundedTimer.ToString() + " / " + (limitTime < 10 ? "0" + limitTime.ToString() : limitTime.ToString());

        // Get errors prefix
        prefix = "Errors: ";
        prefix += errors < 10 ? "0" : "";

        // Set errors text
        errorsText.text = prefix + errors + " / " + (maxErrors < 10 ? "0" + maxErrors.ToString() : maxErrors.ToString());
    }
    #endregion

    #region Check Mini Challenge
    /// <summary>
    /// Check for mini challenge completion.
    /// </summary>
    public void CheckMiniChallenge()
    {
        // Local stars count
        int starsCount = 0;

        // If time challenge was completed, add a star
        if(isTimeChallengeComplete)
        {
            starsCount++;
        }

        // If errors challenge was completed, add a star
        if(isErrorsChallengeComplete)
        {
            starsCount++;
        }

        // Display challenge result canvas
        challengeResultCanvas.SetActive(true);

        // If stars count is bigger than 0, start adding the stars
        if(starsCount > 0)
        {
            StartCoroutine(AddStarWithDelay(starsCount, 1));
        }

        // Start countdown for diabling challenge results canvas
        StartCoroutine(DisableChallengeResultsAfterSeconds(5f));
    }

    /// <summary>
    /// Add a star to challenge result canvas after 0.75 second.
    /// </summary>
    /// <param name="starsCount">Number of stars to add.</param>
    /// <param name="startIndex">Stars list index to add.</param>
    private IEnumerator AddStarWithDelay(int starsCount, int startIndex)
    {
        // Wait for 0.75 second
        yield return new WaitForSeconds(0.75f);

        // Add a star to UI and play sound
        stars[startIndex].SetActive(true);
        audioSource.Play();

        // Decrease stars count
        starsCount--;

        // If there are stars remaining, call AddStarWithDelay
        if(starsCount > 0)
        {
            StartCoroutine(AddStarWithDelay(starsCount, startIndex + 1));
        }

        // If there is no stars remaining
        else
        {
            // Wait for 0.75 second
            yield return new WaitForSeconds(0.75f);

            // Play challenge finished audio clip
            audioSource.clip = challengeFinishedAudioClip; 
            audioSource.Play();
        }
    }

    /// <summary>
    /// Disable challenge results after a given amount of seconds.
    /// </summary>
    /// <param name="seconds">Seconds to wait.</param>
    private IEnumerator DisableChallengeResultsAfterSeconds(float seconds)
    {
        // Wait some seconds
        yield return new WaitForSeconds(seconds);

        // Disable challenge result canvas
        challengeResultCanvas.SetActive(false);
    }
    #endregion
    #endregion
}
