using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class LoopController : MonoBehaviour
{
    public int iterations;
    public Text iterationsText;
    public Text rangeText;

    public int maxIterarions = 10;

    [BoxGroup("Visuals")]
    public GameObject Plane;
    [BoxGroup("Visuals")]
    public float planeSize = 0.9f;
    [BoxGroup("Visuals")]
    public float curRange = 1;
    [BoxGroup("Visuals")]
    public GameObject IncreaseAmountButton;

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

    public void IncreaseRange() 
    {
        if (curRange < ComputerCellsController.instance.mainCells.Count)
        {
            Vector3 positionVector = Plane.transform.localPosition;
            positionVector = new (positionVector.x, positionVector.y - (planeSize / 2), positionVector.z);
            Vector3 scaleVector = Plane.transform.localScale;
            scaleVector = new (scaleVector.x, scaleVector.y, scaleVector.z * (1 + (1 / curRange)));
            Plane.transform.localPosition = positionVector;
            Plane.transform.localScale = scaleVector;
            curRange++;
            if (curRange == ComputerCellsController.instance.mainCells.Count)
            {
                IncreaseAmountButton.SetActive(false);
            }
            else if(curRange == 2)
            {
                rangeText.text = "A";
            }
        }
    }
    public void DecreaseRange() 
    {
        if (curRange > 1)
        {
            Vector3 positionVector = Plane.transform.localPosition;
            positionVector = new(positionVector.x, positionVector.y + (planeSize / 2), positionVector.z);
            Vector3 scaleVector = Plane.transform.localScale;
            scaleVector = new(scaleVector.x, scaleVector.y, scaleVector.z * (1 - (1 / curRange)));
            Plane.transform.localPosition = positionVector;
            Plane.transform.localScale = scaleVector;
            curRange--;
            if (curRange == 1)
            {
                rangeText.text = "X";
            }
            else if (curRange == ComputerCellsController.instance.mainCells.Count - 1)
            {
                IncreaseAmountButton.SetActive(true);
            }
        }
        else if (curRange == 1) gameObject.SetActive(false);
    }

    public void DestroyLooper() 
    { 
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        iterationsText.text = iterations.ToString();
    }

}
