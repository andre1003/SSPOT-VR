using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using NaughtyAttributes;
using SSpot.ComputerCode;
using SSpot.Objectives;
using SSpot.Robot;
using SSpot.Utilities;

public class RunCubes : MonoBehaviourPun
{
    #region Serialized Fields
    
    #region Screens
    [Header("Screens")]
    // Error screen GameObject
    public GameObject errorScreen;

    // Next level instructions screen GameObject
    public GameObject instructionsNextScene;
    #endregion

    #region Correct code
    [Header("Correct Code")]
    
    [SerializeField] private CubeCompiler compiler;

    [SerializeField] private LevelObjectiveSolver objective;
    
    #endregion

    #region Environment
    [Header("Environment")]
    public RobotData robot;
    
    // Programming slots that cube can be attached
    public List<GameObject> codingCell = new();
    
    // Programming slots for loops
    public List<LoopController> loop;

    // Instructions list
    [BoxGroup("CubeInfo")]
    [Label("CodingCell Instructions")]
    [ReadOnly]
    public List<Cube> mainInstructions = new();

    [BoxGroup("CubeInfo")]
    [Label("Tried Instructions")]
    [ReadOnly]
    public List<Cube> triedInstructions = new();

    // Programming slots
    public GameObject terminal;

    // Stars projector (appears at the end of level)
    public GameObject projector;

    // Projector particle system
    public GameObject projectorParticleSystem;

    #endregion
    
    #region Materials
    [Header("Materials")]
    // Material for indicate that the algorithm is correct
    public Material successTerminalMaterial;

    // Material for indicate that the algorithm is wrong
    public Material errorTerminalMaterial;
    #endregion
    
    #region Sounds
    [Header("Sounds")]
    // Success audio
    public AudioClip success;

    // Error audio
    public AudioClip error;
    #endregion
    
    #endregion

    #region Private Fields

    private AudioSource audioSource;

    private int _currentIndex;
    
    #endregion
    

    private void Awake()
    {
        // Setup ambient audio source
        audioSource = GetComponent<AudioSource>();

        if (objective)
        {
            objective.Init(HandleObjectiveResult);
        }
    }

    private void HandleObjectiveResult(ObjectiveResult result)
    {
        switch (result.Type)
        {
            case ObjectiveResult.ResultType.Error:
                Error(result.Message);
                break;
            case ObjectiveResult.ResultType.Success:
                Success();
                break;
        }
    }

    /// <summary>
    /// When player clicks on this object, it tries to run the algorithm.
    /// </summary>
    public void OnPointerClick()
    {
        // Try to run code
        if(PhotonNetwork.OfflineMode)
            TryRun();
        else
            photonView.RPC(nameof(TryRun), RpcTarget.AllBuffered);
    }

    private IEnumerator ExecuteCubeCoroutine(Cube cube)
    {
        yield return cube.ExecuteCoroutine(robot);
        yield return new WaitForSeconds(.5f);
        _currentIndex++;
        NextCommand();
    }
    
    /// <summary>
    /// Run next command from the algorithm.
    /// </summary>
    [PunRPC]
    private void NextCommand()
    {
        // If has commands left
        if(_currentIndex < mainInstructions.Count - 1)
        {
            StartCoroutine(ExecuteCubeCoroutine(mainInstructions[_currentIndex]));
            return;
        }

        // Set animationIndex to 1
        _currentIndex = 1;

        // Clear cubes
        mainInstructions.Clear();

        // Set final GameObjects
        if (instructionsNextScene) 
            instructionsNextScene.SetActive(true);
        if(projector) 
            projector.SetActive(true);

        // Destroy particle system after 5 seconds
        if(projectorParticleSystem)
            Destroy(projectorParticleSystem, 5.0f);

    }

    private Coroutine _errorDisableCoroutine;
    /// <summary>
    /// If player makes a mistake in algorithm, then the game will show the error.
    /// <para>This method plays error song, set material to error material and display error message.</para>
    /// </summary>
    /// <param name="errorMessage">The error message that will be displayed.</param>
    [PunRPC]
    private void Error(string errorMessage)
    {
        StopCoroutine(_errorDisableCoroutine);
        
        // Clear cubes list
        mainInstructions.Clear();

        // Play error song
        audioSource.clip = error;
        audioSource.Play();

        // Change terminal material to red (indicates error)
        if(terminal != null)
        {
            var mats = terminal.GetComponent<MeshRenderer>().materials;
            mats[1] = errorTerminalMaterial;
            terminal.GetComponent<MeshRenderer>().materials = mats;
        }

        // Display error message to player
        errorScreen.transform.GetComponentInChildren<Text>().text = errorMessage;
        errorScreen.SetActive(true);

        // Hide error screen after 5 seconds
        _errorDisableCoroutine = StartCoroutine(CoroutineUtilities.WaitThenDeactivate(5.0f, errorScreen));

    }

    /// <summary>
    /// If player have correctly built the whole algorithm, then the game will succeed.
    /// <para>This method plays success song, set material to success material and display the final GameObjects.</para>
    /// </summary>
    [PunRPC]
    private void Success()
    {
        // Play success song
        audioSource.clip = success;
        audioSource.Play();

        // Change terminal material to green (indicates success)
        if(terminal != null)
        {
            var mats = terminal.GetComponent<MeshRenderer>().materials;
            mats[1] = successTerminalMaterial;
            terminal.GetComponent<MeshRenderer>().materials = mats;
        }

        // Close error screen
        errorScreen.SetActive(false);
    }

    /// <summary>
    /// Check if code is runnable via RPC then execute the algorithm.
    /// </summary>
    [PunRPC]
    private void TryRun()
    {
        CompilationResult compilationResult = default;
        if (compilationResult.IsError)
        {
            Error(compilationResult.Error);
            return;
        }
        
        mainInstructions.AddRange(compilationResult.Result);
        triedInstructions.AddRange(compilationResult.Result);

        /*var objectiveResult = objective
            ? objective.EvaluateCubes(mainInstructions)
            : ObjectiveResult.None();
        
        HandleObjectiveResult(objectiveResult);
        if (objectiveResult.Type == ObjectiveResult.ResultType.Error)
            return;*/

        //Play audio and begin running commands
        robot.AudioSource.Play();
        NextCommand();
        
    }
}