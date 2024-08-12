using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabirinthManager : MonoBehaviour
{
    #region Singleton
    public static LabirinthManager instance;

    void Awake()
    {
        if(instance != null)
            Destroy(gameObject);
        instance = this;

        roof1.SetActive(false);
        roof2.SetActive(false);
    }
    #endregion

    [SerializeField] private GameObject computer1;
    [SerializeField] private GameObject computer2;

    [SerializeField] private GameObject roof1;
    [SerializeField] private GameObject roof2;

    [SerializeField] private GameObject tv1;
    [SerializeField] private GameObject tv2;

    [SerializeField] private GameObject instructions1;
    [SerializeField] private GameObject instructions2;

    [SerializeField] private GameObject projector;

    [Header("UI")]
    [SerializeField] private Text instructionPlayer1;
    [SerializeField] private Text instructionPlayer2;

    private bool checkpoint = false;

    void Start()
    {
        // Find current player's index on network
        int index = 0;
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if(PhotonNetwork.PlayerList[i].IsLocal)
            {
                index = i;
                break;
            }
        }

        // Player roof setup based on player number
        if(index == 0)
        {
            roof1.SetActive(true);
            roof2.SetActive(false);
        }
        else
        {
            roof1.SetActive(false);
            roof2.SetActive(true);
        }

        // Activate only tv2
        tv1.SetActive(false);
        tv2.SetActive(true);
    }

    // Set labirinth checkpoint
    public void SetCheckpoint()
    {
        // Set checkpoint
        checkpoint = true;
        ResetLabirinth.instance.SetCheckpoint();

        // Switch players computers
        computer1.SetActive(false);
        computer2.SetActive(true);

        // Switch players tvs
        tv1.SetActive(true);
        tv2.SetActive(false);

        // Switch players instructions
        string auxText = instructionPlayer1.text;
        instructionPlayer1.text = instructionPlayer2.text;
        instructionPlayer2.text = auxText;
    }

    // Labirinth success
    public void LabirinthSuccess()
    {
        // Stop robot
        ResetLabirinth.instance.FinishLevel();

        // Deactivate computers
        computer1.SetActive(false);
        computer2.SetActive(false);

        // Deactivate instructions
        instructions1.SetActive(false);
        instructions2.SetActive(false);

        // Deactivate roofs
        roof1.SetActive(false);
        roof2.SetActive(false);

        // Deactivate tvs
        tv1.SetActive(false);
        tv2.SetActive(false);

        // Activate projector
        projector.SetActive(true);
    }
}
