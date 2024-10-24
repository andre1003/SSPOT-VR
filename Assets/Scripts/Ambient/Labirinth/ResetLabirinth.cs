using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLabirinth : MonoBehaviour
{
    #region Singleton
    public static ResetLabirinth instance;

    void Awake()
    {
        if(instance != null)
            Destroy(this);
        instance = this;
    }
    #endregion


    // Robot reference
    [SerializeField] private GameObject robot;
    [SerializeField] private Animator robotAnimator;

    // Reset points
    [SerializeField] private Transform firstPoint;
    [SerializeField] private Transform secondPoint;

    // Checkpoint manager
    [SerializeField] private bool reloadCheckpoint = false;

    // Resets
    [SerializeField] private ResetCubesAlt firstPlayerReset;
    [SerializeField] private ResetCubesAlt secondPlayerReset;
    
    // Runners
    [SerializeField] private RunCubesAlt firstPlayerRun;
    [SerializeField] private RunCubesAlt secondPlayerRun;


    // Reset robot position and rotation
    public void ResetRobotPosition()
    {
        // Set position and rotation values
        Vector3 position = firstPoint.position;
        Quaternion rotation = firstPoint.rotation;

        // Set to checkpoint, if needed
        if(reloadCheckpoint)
        {
            position = secondPoint.position;
            rotation = secondPoint.rotation;
        }

        // Fix robot Y position
        position.y = robot.transform.position.y;

        // Set robot position and rotation
        robot.transform.position = position;
        robot.transform.rotation = rotation;
    }

    // Reset robot if it hits a wall
    public void WrongWay()
    {
        if(reloadCheckpoint)
            secondPlayerReset.ResetCallback();
        else
            firstPlayerReset.ResetCallback();
    }

    // Set checkpoint
    public void SetCheckpoint()
    {
        firstPlayerRun.StopExecution();
        reloadCheckpoint = true;
        StartCoroutine(WaitToStopRobot());
    }

    IEnumerator WaitToStopRobot()
    {
        yield return new WaitForSeconds(0.75f);
        robotAnimator?.SetBool("Forward", false);
        robotAnimator?.SetBool("Right", false);
        robotAnimator?.SetBool("Left", false);
    }

    public void FinishLevel()
    {
        secondPlayerRun.ResetComputer();
        robotAnimator?.SetBool("Forward", false);
        robotAnimator?.SetBool("Right", false);
        robotAnimator?.SetBool("Left", false);
    }
}
