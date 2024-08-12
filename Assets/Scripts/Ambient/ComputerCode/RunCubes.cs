using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RunCubes : MonoBehaviourPun
{
    #region Attributes
    #region Screens
    [Header("Screens")]
    // Error screen GameObject
    public GameObject errorScreen;

    // Next level instructions screen GameObject
    public GameObject instructionsNextScene;
    #endregion

    #region Robot movement
    [Header("Robot Movement")]
    // Robot GameObject
    public GameObject robot;

    // Robot animation
    public Animation robotAnimation;

    // Index of animation
    public int animationIndex;

    // Boolean varible for wait for idle
    public bool waitingForIdle;
    #endregion

    #region Correct code
    [Header("Correct Code")]
    // Novalidate
    public bool novalidate = false;

    // Desired instructions list
    public List<string> instructions;

    // Desierd loop commands list
    public List<string> loop;
    #endregion

    #region Environment
    [Header("Environment")]
    // Programming slots that cube can be attached
    public List<GameObject> codingCell = new List<GameObject>();

    // Programming slots
    public GameObject terminal;

    // Stars projector (appears at the end of level)
    public GameObject projector;

    // Projector particle system
    public GameObject projectorParticleSystem;

    // Elevator button
    public GameObject elevatorButton;

    // Elevator controller script
    public GoingUpAndDownController scriptGoingUpAndDown;
    #endregion

    #region Materials
    [Header("Materials")]
    // Material for indicate that the algorithm is correct
    public Material successTerminalMaterial;

    // Material for indicate that the algorithm is wrong
    public Material errorTerminalMaterial;
    #endregion

    #region Audio-visual resource
    [Header("Sounds")]
    // Success audio
    public AudioClip success;

    // Error audio
    public AudioClip error;
    #endregion

    #region Player
    [Header("Player Hand")]
    // Player hand
    public GameObject playerHand;
    #endregion

    #region Robot variables
    // Audio source
    private AudioSource audioSource;

    // Material vector
    private Material[] mats;

    // Robot animator
    private Animator robotAnimator;

    // Robot audio source
    private AudioSource robotSource;
    #endregion

    #region Computer commands
    [Header("Computer Commands Map")]
    // Loop commands list
    public static List<string> loopCommands = new List<string>();

    // Instructions list
    public static List<string> mainInstructions = new List<string>();
    #endregion

    #region Iteration
    // Remaining iterations
    private int iteration = 0;

    // Iteration start index
    private int iterationStart = -1;

    // Iteration end index
    private int iterationEnd = -1;
    #endregion
    #endregion


    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        // Setup robot audio-visual attributes
        robotAnimator = robot.GetComponent<Animator>();
        robotAnimation = robot.GetComponent<Animation>();
        robotSource = robot.GetComponent<AudioSource>();

        // Setup robot movement attributes
        animationIndex = 1;
        waitingForIdle = false;

        // Setup iteration attributes
        iterationStart = -1;
        iterationEnd = -1;

        // Setup ambient audio source
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // If is waiting for robot's idle
        if(waitingForIdle)
        {
            AnimatorStateInfo robotState = robotAnimator.GetCurrentAnimatorStateInfo(0);
            if(// If robot walk ended
                    robotState.IsName("Walk") &&
                    robotState.normalizedTime >= robotState.length &&
                    robotAnimator.GetBool("Forward")
                )
            {
                // Stop robot
                robotAnimator.SetBool("Forward", false);
                waitingForIdle = false;

                // Go to next command after 0.5 seconds (enought time to refresh robot animation)
                StartCoroutine(WaitToNextCommand(0.5f));
            }


            if(// If robot is turning left
                    robotState.IsName("TurnLeft") &&
                    robotState.normalizedTime >= robotState.length &&
                    robotAnimator.GetBool("Left")
                )
            {

                // Stop robot
                robotAnimator.SetBool("Left", false);
                waitingForIdle = false;

                // Go to next command after 0.5 seconds (enought time to refresh robot animation)
                StartCoroutine(WaitToNextCommand(0.5f));
            }


            if(// If robot is turning right
                    robotState.IsName("TurnRight") &&
                    robotState.normalizedTime >= robotState.length &&
                    robotAnimator.GetBool("Right")
                )
            {

                // Stop robot
                robotAnimator.SetBool("Right", false);
                waitingForIdle = false;

                // Go to next command after 0.5 seconds (enought time to refresh robot animation)
                StartCoroutine(WaitToNextCommand(0.5f));
            }
        }
    }

    /// <summary>
    /// Wait a given seconds to call NextCommand().
    /// </summary>
    /// <param name="seconds">Seconds to wait.</param>
    private IEnumerator WaitToNextCommand(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // Call NextCommand method
        RunNextCommand();
    }

    /// <summary>
    /// When player clicks on this object, it tries to run the algorithm.
    /// </summary>
    public void OnPointerClick()
    {
        // Try to run code
        if(PhotonNetwork.OfflineMode)
            CheckIsRunnable();
        else
            photonView.RPC("CheckIsRunnable", RpcTarget.AllBuffered);
    }

    /// <summary>
    /// Run next command from the algorithm.
    /// </summary>
    [PunRPC]
    public void NextCommand()
    {
        // If have any coammand left
        if(animationIndex < (mainInstructions.Count - 1))
        {
            robotAnimator.SetBool(mainInstructions[animationIndex], true); // Set animation according to command
            waitingForIdle = true;                              // Wait for robot idle
            animationIndex++;                                   // Increase animationIndex
        }
        // If doesn't have any command left
        else
        {
            // Set animationIndex to 1
            animationIndex = 1;

            // Clear cubes
            mainInstructions.Clear();

            // Set final GameObjects
            instructionsNextScene?.SetActive(true);
            projector?.SetActive(true);

            // Destroy particle system after 5 seconds
            StartCoroutine(WaitToDestroy(5.0f, projectorParticleSystem));

            // Call challenge check
            MiniChallenge.instance.CheckMiniChallenge();
        }
    }

    /// <summary>
    /// If player makes a mistake in algorithm, then the game will show the error.
    /// 
    /// <para>This method plays error song, set material to error material and display error message.</para>
    /// </summary>
    /// <param name="errorMessage">The error message that will be displayed.</param>
    [PunRPC]
    private void Error(string errorMessage)
    {
        // Clear cubes list
        mainInstructions.Clear();

        // Play error song
        audioSource.clip = error;
        audioSource.Play();

        // Change terminal material to red (indicates error)
        if(terminal != null)
        {
            mats = terminal.GetComponent<MeshRenderer>().materials;
            mats[1] = errorTerminalMaterial;
            terminal.GetComponent<MeshRenderer>().materials = mats;
        }

        // Display error message to player
        errorScreen.transform.GetComponentInChildren<Text>().text = errorMessage;
        errorScreen.SetActive(true);

        // Hide error screen after 5 seconds
        StartCoroutine(WaitToDeactivate(5.0f, errorScreen));

        // Increase mini challenge errors
        MiniChallenge.instance.IncreaseError();
    }

    /// <summary>
    /// If player have correctly built the whole algorithm, then the game will successed.
    /// 
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
            mats = terminal.GetComponent<MeshRenderer>().materials;
            mats[1] = successTerminalMaterial;
            terminal.GetComponent<MeshRenderer>().materials = mats;
        }

        RunCode();

        // Close error screen
        errorScreen.SetActive(false);

        // Stop mini challenge timer
        MiniChallenge.instance.StopTimer();
    }

    [PunRPC]
    private void RunCode()
    {
        // Play robot audio source and next command
        robotSource.Play();
        RunNextCommand();
    }

    /// <summary>
    /// Wait a given seconds to destroy a given GameObject.
    /// </summary>
    /// <param name="seconds">Seconds to wait.</param>
    /// <param name="objectToDestroy">GameObject to destroy.</param>
    private IEnumerator WaitToDestroy(float seconds, GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(seconds);

        Destroy(objectToDestroy);
    }

    /// <summary>
    /// Wait a given seconds to deactivate a given GameObject.
    /// </summary>
    /// <param name="seconds">Seconds to wait.</param>
    /// <param name="objectToDestroy">GameObject to deactivate.</param>
    private IEnumerator WaitToDeactivate(float seconds, GameObject objectToDeactivate)
    {
        yield return new WaitForSeconds(seconds);

        objectToDeactivate.SetActive(false);
    }

    /// <summary>
    /// Check if all loop commands are correct.
    /// </summary>
    /// <returns>TRUE if all loop commands are correct. FALSE if not.</returns>
    [PunRPC]
    private bool CheckLoop()
    {
        // Get all loop commands
        loopCommands = ComputerCellsController.instance.GetAllLeftCommands();

        // If loop command number are different than expected, give an error and exit
        if(loopCommands.Count != loop.Count)
        {
            Error("As repetições estão erradas!");
            return false;
        }

        // Loop loop commands
        for(int i = 0; i < loopCommands.Count; i++)
        {
            // If loop command is different than expected, give an error and exit
            if(loopCommands[i] != loop[i])
            {
                Error("A repetição na linha " + i + " está incorreta!");
                Debug.Log("LoopCommands: " + loopCommands[i] + "\nEsperado: " + loop[i]);
                return false;
            }
        }

        // Loop commands are correct
        return true;
    }

    /// <summary>
    /// Check if all instructions are correct.
    /// </summary>
    /// <returns>TRUE if all instructions are correct. FALSE if not.</returns>
    [PunRPC]
    private bool CheckInstructions()
    {
        // Local variables
        string cube;    // Cube name
        int length = 0; // Name length

        // Compiler
        for(int i = 0; i < codingCell.Count; i++)
        {
            #region Valid Coding Cell
            // Verify if all the slots are sequentially filled. There can be no empty slots
            if(codingCell[i].transform.childCount > 0)
            {
                // Get child GameObject's name length
                length = codingCell[i].transform.GetChild(0).gameObject.name.Length;

                // Remove not desirable name
                cube = codingCell[i].transform.GetChild(0).gameObject.name.Remove(length - 16);

                // Add cube to cubes list
                mainInstructions.Add(cube);

            }
            #endregion Valid Coding Cell

            #region Errors
            // If coding cell doesn't have a child
            else
            {
                // Call Error method
                Error("Deu ERRO! Você deve preencher todas as placas de programação!");

                // Stop for loop
                return false;
            }
            // If code cell is not correct
            if(mainInstructions[i] != instructions[i])
            {
                // Check if the first cube is not "Begin"
                if(i == 0 && mainInstructions[i] != "Begin")
                {
                    // Call Error method
                    Error("Deu ERRO! Verifique se o algoritmo foi iniciado corretamente!");

                    // Stop for loop
                    return false;
                }

                // Check if the last cube is not "End"
                if(i == (codingCell.Count - 1) && mainInstructions[i] != "End")
                {
                    // Call Error method
                    Error("Deu ERRO! Verifique se o algoritmo foi finalizado corretamente!");

                    // Stop for loop
                    return false;
                }

                // Debug code information
                Debug.Log("Code: " + instructions[i] + ", Cubes: " + mainInstructions[i] + ", Index: " + i);

                // Check if "Begin" and "End" cubes are in the middle of the algorithm
                if((i != 0) && (i != codingCell.Count - 1) && (mainInstructions[i] == "Begin" || mainInstructions[i] == "End"))
                {
                    // Call Error method
                    Error("Deu ERRO! Início e Fim devem ser usados no lugar certo!");

                    // Stop for loop
                    return false;
                }

                // Something else is wrong
                Error("Deu ERRO!\nCódigo incorreto! Tente novamente!");
                return false;
            }
            #endregion Errors
        }

        // Instructions are correct
        return true;
    }

    [PunRPC]
    private bool BasicCodeCheck()
    {
        for(int i = 0; i < codingCell.Count; i++)
        {
            // Verify if all the slots are sequentially filled. There can be no empty slots
            if(codingCell[i].transform.childCount > 0)
            {
                // Get child GameObject's name length
                int length = codingCell[i].transform.GetChild(0).gameObject.name.Length;

                // Remove not desirable name
                string cube = codingCell[i].transform.GetChild(0).gameObject.name.Remove(length - 16);

                // Add cube to cubes list
                mainInstructions.Add(cube);

            }

            // If coding cell doesn't have a child
            else
            {
                Error("Deu ERRO! Você deve preencher todas as placas de programação!");
                return false;
            }

            // Check if the first cube is not "Begin"
            if(i == 0 && mainInstructions[i] != "Begin")
            {
                Error("Deu ERRO! Verifique se o algoritmo foi iniciado corretamente!");
                return false;
            }

            // Check if the last cube is not "End"
            if(i == (codingCell.Count - 1) && mainInstructions[i] != "End")
            {
                Error("Deu ERRO! Verifique se o algoritmo foi finalizado corretamente!");
                return false;
            }

            // Check if "Begin" and "End" cubes are in the middle of the algorithm
            if((i != 0) && (i != codingCell.Count - 1) && (mainInstructions[i] == "Begin" || mainInstructions[i] == "End"))
            {
                Error("Deu ERRO! Início e Fim devem ser usados no lugar certo!");
                return false;
            }
        }
        loopCommands = ComputerCellsController.instance.GetAllLeftCommands();
        return true;
    }

    /// <summary>
    /// Check if code is runnable via RPC.
    /// </summary>
    [PunRPC]
    private void CheckIsRunnable()
    {
        if(novalidate)
        {
            if(BasicCodeCheck())
                RunCode();
            return;
        }

        // Check loops and instructions
        bool checkLoop = CheckLoop();
        bool checkInstructions = CheckInstructions();

        // If both loops and instructions are correct, run animations
        if(checkLoop && checkInstructions)
        {
            Success();
        }
    }

    /// <summary>
    /// Find the end of repeat loop.
    /// </summary>
    /// <param name="startIndex">Repeat instruction start index.</param>
    /// <returns>If there is a end repeat, return it's index. Else, return startIndex + 1.</returns>
    [PunRPC]
    int FindRepeatEnd(int startIndex)
    {
        // Loop loop commands, starting at startIndex
        for(int i = startIndex; i < loopCommands.Count; i++)
        {
            // If finds the end loop command, return its index
            if(loopCommands[i] == "EndRepeat")
            {
                return i + 1;
            }
            else if(i != startIndex && loopCommands[i] == "Repeat")
                break;
        }

        // Return startIndex if there is no end loop command
        return startIndex + 1;
    }

    /// <summary>
    /// Run the next instruction displayed on terminal.
    /// </summary>
    [PunRPC]
    public void RunNextCommand()
    {
        // If there is an iterarion start index and the animation index is equal to iteration end
        if(iterationStart != -1 && animationIndex == iterationEnd)
        {
            // Decrease iteration
            iteration--;

            // If there are remaining iterations
            if(iteration > 0)
            {
                // Set animation index to iteration start
                animationIndex = iterationStart;
            }

            // If there are no remaining iterations, clear iteration variables
            else
            {
                iteration = 0;
                iterationStart = -1;
                iterationEnd = -1;
                RunNextCommand();
                return;
            }
        }

        // Else, if the animation index is on loop commands range
        else if(animationIndex < (loopCommands.Count))
        {
            // If the loop comand is repeat begin
            if(loopCommands[animationIndex] == "Repeat")
            {
                // Setup iterations variables
                iteration = ComputerCellsController.instance.GetRightCellAtIndex(animationIndex).GetComponent<LoopController>().iterations;
                Debug.Log(iteration);
                iterationStart = animationIndex;
                iterationEnd = FindRepeatEnd(iterationStart);
            }
        }

        // Run next command
        NextCommand();
    }
    #endregion
}