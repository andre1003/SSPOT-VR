using Photon.Pun;
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


    // Mini challenge config
    public float limitTime = 0f;
    public int maxErrors = 3;

    // Timer text reference
    public Text timerText;
    public Text errorsText;

    // Goal fail text color
    public Color failColor;


    // Timer value
    private float timer = 0f;

    // Current number of errors
    private int errors = 0;

    // Challenge completition controllers
    private bool isTimeChallengeComplete = true;
    private bool isErrorsChallengeComplete = true;


    // Start is called before the first frame update
    void Start()
    {
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
}
