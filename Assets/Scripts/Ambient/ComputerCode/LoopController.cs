using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopController : MonoBehaviourPun
{
    public int iterations;
    public Text amountText;

    public int maxIterarions = 10;

    void Awake()
    {
        UpdateUI();
    }

    public void IncreaseIterations()
    {
        photonView.RPC("IncreaseRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void IncreaseRPC()
    {
        iterations++;

        if(iterations > maxIterarions)
        {
            iterations = maxIterarions;
        }

        UpdateUI();
    }

    public void DecreaseIterations()
    {
        photonView.RPC("DecreaseRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void DecreaseRPC()
    {
        iterations--;

        if(iterations < 1)
        {
            iterations = 1;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        amountText.text = iterations.ToString();
    }
}
