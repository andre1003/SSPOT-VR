using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerCellsController : MonoBehaviour
{
    #region Singleton
    public static ComputerCellsController instance;

    void Awake()
    {
        instance = this;
    }
    #endregion

    public List<GameObject> mainCells;
    public List<GameObject> rightCells;
    public List<GameObject> loopPanels;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject GetMainCellAtIndex(int index)
    {
        return mainCells[index];
    }

    public GameObject GetLoopPanelAtIndex(int index)
    {
        return loopPanels[index];
    }

    public GameObject GetRightCellAtIndex(int index)
    {
        return rightCells[index];
    }
}
