using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerCellsController : MonoBehaviour
{
    #region Singleton
    public static ComputerCellsController instance;

    void Awake()
    {
        if(instance == null)
            instance = this;
    }
    #endregion

    public List<GameObject> mainCells;
    public List<GameObject> leftCells;
    public List<GameObject> rightCells;

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

    public GameObject GetLeftCellAtIndex(int index)
    {
        return leftCells[index];
    }

    public GameObject GetRightCellAtIndex(int index)
    {
        return rightCells[index];
    }

    public List<string> GetAllLeftCommands()
    {
        // Create commands local list
        List<string> commands = new List<string>();

        // Loop all left cells
        foreach(GameObject cell in leftCells)
        {
            GameObject cube;

            try
            {
                cube = cell.transform.GetChild(0).transform.GetChild(0).gameObject;
                // If there is a repeat cube, add its name to local list
                if(cube.name.Contains("Repeat"))
                {
                    int length = cube.name.Length;
                    commands.Add(cube.name.Remove(length - 16));
                }
                else
                {
                    commands.Add("None");
                }
            }
            catch
            {
                commands.Add("None");
            }


        }

        // Return commands
        return commands;
    }
}
