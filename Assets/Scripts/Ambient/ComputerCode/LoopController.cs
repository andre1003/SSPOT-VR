using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopController : MonoBehaviour
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
        iterations++;

        if(iterations > maxIterarions)
        {
            iterations = maxIterarions;
        }

        UpdateUI();
    }

    public void DecreaseIterations()
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
