using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Data.Common;
// TODO: comentar a classe e revisar algumas fun��es
public class LoopController : MonoBehaviour

{
    [BoxGroup("LoopSettings")]
    public int iterations;

    [BoxGroup("LoopSettings")]
    public int maxIterarions = 10;

    [BoxGroup("LoopSettings")]
    public int maxRange = 6;

    [BoxGroup("LoopSettings")]
    public bool globalMaxRange = false;

    [BoxGroup("LoopSettings")]
    [ShowIf("globalMaxRange")]
    [MinValue(1)]
    public int globalRange = 1;

    [BoxGroup("Visuals")]
    public Text iterationsText;
    [BoxGroup("Visuals")]
    public Text rangeText;
    [BoxGroup("Visuals")]
    public GameObject Plane;
    [BoxGroup("Visuals")]
    public float planeSize = 0.9f;
    [BoxGroup("Visuals")]
    public float curRange = 1;
    [BoxGroup("Visuals")]
    public GameObject IncreaseAmountButton;

    private void OnEnable()
    {
        UpdateAllPanels();
    }

    private void OnDisable()
    {
        UpdateAllPanels();
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

    public void IncreaseRange() 
    {
        if (curRange < maxRange)
        {
            Vector3 tempVector = Plane.transform.localPosition;
            Plane.transform.localPosition = new (tempVector.x, tempVector.y - (planeSize / 2), tempVector.z);
            
            tempVector = Plane.transform.localScale;
            Plane.transform.localScale = new (tempVector.x, tempVector.y, tempVector.z * (1 + (1 / curRange)));
            
            curRange++;
            UpdateUI();

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
            UpdateUI();
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
        IncreaseAmountButton.SetActive(curRange != maxRange);
        rangeText.text = curRange == 1 ? "X" : "A";
    }

    private void UpdateRange()
    {
        // updating maxRange
        List<GameObject> Panels = ComputerCellsController.instance.loopPanels;
        int panelCount = Panels.Count;

        
        for (int i = 0; i < panelCount; i++)
        {
            if (Panels[i].GetComponent<LoopController>() == this)
            {
                maxRange = panelCount - i;
                if(i == panelCount - 1) { maxRange = 1; return; }

                i++;
                for (int rangeCount = 1; i < panelCount; rangeCount++, i++)
                {
                    if (Panels[i].activeInHierarchy) 
                    { 
                        maxRange = rangeCount;
                    }
                }
            }
        }
        while (curRange > maxRange) DecreaseRange();
        if (globalMaxRange && maxRange > globalRange) maxRange = globalRange;
    }

    public void UpdateAllPanels()
    {
        List<GameObject> Panels = ComputerCellsController.instance.loopPanels;
        int panelCount = Panels.Count;

        for (int i = 0; i < panelCount; i++)
        {
            if (Panels[i].activeInHierarchy) 
            {
                Panels[i].GetComponent<LoopController>().UpdateRange();
                Panels[i].GetComponent<LoopController>().UpdateUI();
            } 
        } 
    }

}
