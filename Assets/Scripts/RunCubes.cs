using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunCubes : MonoBehaviour {
    // Screens
    public GameObject errorScreen;                                  // Error screen GameObject
    public GameObject instructionsNextScene;                        // Next level instructions screen GameObject

    // Robot movement
    public GameObject robot;                                        // Robot GameObject
    public Animation robotAnimation;                                // Robot animation
    public int animationIndex;                                      // Index of animation
    public bool waitingForIdle;                                     // Boolean varible for wait for idle

    // Correct code
    public List<string> code;

    // Enviroment
    public List<GameObject> codingCell = new List<GameObject>();    // Programming slots that cube can be attached
    public static List<string> cubes = new List<string>();          // Cubes list
    public GameObject terminal;                                     // Programming slots
    public GameObject projector;                                    // Stars projector (appears at the end of level)
    public GameObject projectorParticleSystem;                      // Projector particle system
    public GameObject elevatorButton;                               // Elevator button
    public GoingUpAndDownController scriptGoingUpAndDown;           // Elevator controller script

    // Materials
    public Material successTerminalMaterial;                        // Material for indicate that the algorithm is correct
    public Material errorTerminalMaterial;                          // Material for indicate that the algorithm is wrong

    // Audio-visual resource
    public AudioClip success;                                       // Success audio
    public AudioClip error;                                         // Error audio

    // Player
    public GameObject playerHand;                                   // Player hand


    // Robot variables
    private AudioSource audioSource;                                // Audio source
    private Material[] mats;                                        // Material vector
    private Animator robotAnimator;                                 // Robot animator
    private AudioSource robotSource;                                // Robot audio source

    // Counters
    //private int errors = 0;

    // Start is called before the first frame update
    void Start() {
        // Set robot audio-visual attributes
        robotAnimator = robot.GetComponent<Animator>();
        robotAnimation = robot.GetComponent<Animation>();
        robotSource = robot.GetComponent<AudioSource>();

        animationIndex = 1;
        waitingForIdle = false;

        // Set ambient audio source
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void LateUpdate() {
        // If is waiting for robot's idle
        if (waitingForIdle) {
            
            if (// If robot walk ended
                    robotAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk") &&
                    robotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= robotAnimator.GetCurrentAnimatorStateInfo(0).length &&
                    robotAnimator.GetBool("Forward")
                ) {
                // Stop robot
                robotAnimator.SetBool("Forward", false);
                waitingForIdle = false;

                // Go to next command after 0.5 seconds (enought time to refresh robot animation)
                StartCoroutine(WaitToNextCommand(0.5f));
            }


            if (// If robot is turning left
                    robotAnimator.GetCurrentAnimatorStateInfo(0).IsName("TurnLeft") &&
                    robotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= robotAnimator.GetCurrentAnimatorStateInfo(0).length &&
                    robotAnimator.GetBool("Left")
                ) {

                // Stop robot
                robotAnimator.SetBool("Left", false);
                waitingForIdle = false;

                // Go to next command after 0.5 seconds (enought time to refresh robot animation)
                StartCoroutine(WaitToNextCommand(0.5f));
            }


            if (// If robot is turning right
                    robotAnimator.GetCurrentAnimatorStateInfo(0).IsName("TurnRight") &&
                    robotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= robotAnimator.GetCurrentAnimatorStateInfo(0).length &&
                    robotAnimator.GetBool("Right")
                ) {

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
    private IEnumerator WaitToNextCommand(float seconds) {
        yield return new WaitForSeconds(seconds);

        // Call NextCommand method
        NextCommand();
    }

    /// <summary>
    /// When player clicks on this object, it tries to run the algorithm.
    /// </summary>
    public void OnPointerClick() {
        // Try to run code
        Run();

        // If there is a cube in player's hand
        if (playerHand.transform.childCount != 0) {
            // Clear the hand
            Destroy(playerHand.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// Run next command from the algorithm.
    /// </summary>
    public void NextCommand() {
        // If have any coammand left
        if (animationIndex < (cubes.Count - 1)) {
            Debug.Log(cubes[animationIndex]);
            robotAnimator.SetBool(cubes[animationIndex], true); // Set animation according to command
            waitingForIdle = true;                              // Wait for robot idle
            animationIndex++;                                   // Increase animationIndex
        }
        // If doesn't have any command left
        else {
            // Set animationIndex to 1
            animationIndex = 1;

            // Clear cubes
            cubes.Clear();

            // Set final GameObjects
            instructionsNextScene.SetActive(true);
            projector.SetActive(true);

            // Destroy particle system after 5 seconds
            StartCoroutine(WaitToDestroy(5.0f, projectorParticleSystem));
        }
    }

    /// <summary>
    /// Run the algorithm constructed by the player.
    /// 
    /// <para>Access every codingCell and verify if it has child. If it does, store the child's GameObject name at cubes list in instruction format.
    /// Then displays it to player.</para>
    /// </summary>
    public void Run() {
        // Local variables
        string cube;    // Cube name
        int length = 0; // Name length

        // Compiler
        for (int i = 0; i < codingCell.Count; i++) {
            #region Valid Coding Cell
            // Verify if all the slots are sequentially filled. There can be no empty slots
            if (codingCell[i].transform.childCount > 0) {
                length = codingCell[i].transform.GetChild(0).gameObject.name.Length;            // Get child GameObject's name length
                cube = codingCell[i].transform.GetChild(0).gameObject.name.Remove(length - 16); // Remove not desirable name
                cubes.Add(cube);                                                                // Add cube to cubes list

            }
            #endregion Valid Coding Cell

            #region Errors
            // If coding cell doesn't have a child
            else {
                // Call Error method
                Error("Deu ERRO! Você deve preencher todas as placas de programação!");

                // Stop for loop
                break;
            }
            // If code cell is not correct
            if(cubes[i] != code[i]) {
                // Check if the first cube is not "Begin"
                if(i == 0 && cubes[i] != "Begin") {
                    // Call Error method
                    Error("Deu ERRO! Verifique se o algoritmo foi iniciado corretamente!");

                    // Stop for loop
                    break;
                }

                // Check if the last cube is not "End"
                if(i == (codingCell.Count - 1) && cubes[i] != "End") {
                    // Call Error method
                    Error("Deu ERRO! Verifique se o algoritmo foi finalizado corretamente!");

                    // Stop for loop
                    break;
                }


                Debug.Log("Code: " + code[i] + ", Cubes: " + cubes[i] + ", Index: " + i);
                // Check if "Begin" and "End" cubes are in the middle of the algorithm
                if((i != 0) && (i != codingCell.Count - 1) && (cubes[i] == "Begin" || cubes[i] == "End")) {
                    // Call Error method
                    Error("Deu ERRO! Início e Fim devem ser usados no lugar certo!");

                    // Stop for loop
                    break;
                }

                Error("Deu ERRO!\nCódigo incorreto! Tente novamente!");
                break;
            }
            #endregion Errors

            #region Success
            // Check if i is the last index
            if (i == (codingCell.Count - 1)) {
                // Call Success method
                Success();
            }
            #endregion Success
        }
    }

    /// <summary>
    /// If player makes a mistake in algorithm, then the game will show the error.
    /// 
    /// <para>This method plays error song, set material to error material and display error message.</para>
    /// </summary>
    /// <param name="errorMessage">The error message that will be displayed.</param>
    private void Error(string errorMessage) {
        // Clear cubes list
        cubes.Clear();

        // Play error song
        audioSource.clip = error;
        audioSource.Play();

        // Change terminal material to red (indicates error)
        mats = terminal.GetComponent<MeshRenderer>().materials;
        mats[1] = errorTerminalMaterial;
        terminal.GetComponent<MeshRenderer>().materials = mats;

        // Display error message to player
        errorScreen.transform.GetComponentInChildren<Text>().text = errorMessage;
        errorScreen.SetActive(true);

        // Hide error screen after 5 seconds
        StartCoroutine(WaitToDeactivate(5.0f, errorScreen));
    }

    /// <summary>
    /// If player have correctly built the whole algorithm, then the game will successed.
    /// 
    /// <para>This method plays success song, set material to success material and display the final GameObjects.</para>
    /// </summary>
    private void Success() {
        // Play success song
        audioSource.clip = success;
        audioSource.Play();

        // Change terminal material to green (indicates success)
        mats = terminal.GetComponent<MeshRenderer>().materials;
        mats[1] = successTerminalMaterial;
        terminal.GetComponent<MeshRenderer>().materials = mats;

        // Play robot audio source and next command
        robotSource.Play();
        NextCommand();
        
        // Close error screen
        errorScreen.SetActive(false);
    }

    /// <summary>
    /// Wait a given seconds to destroy a given GameObject.
    /// </summary>
    /// <param name="seconds">Seconds to wait.</param>
    /// <param name="objectToDestroy">GameObject to destroy.</param>
    private IEnumerator WaitToDestroy(float seconds, GameObject objectToDestroy) {
        yield return new WaitForSeconds(seconds);

        Destroy(objectToDestroy);
    }

    /// <summary>
    /// Wait a given seconds to deactivate a given GameObject.
    /// </summary>
    /// <param name="seconds">Seconds to wait.</param>
    /// <param name="objectToDestroy">GameObject to deactivate.</param>
    private IEnumerator WaitToDeactivate(float seconds, GameObject objectToDeactivate) {
        yield return new WaitForSeconds(seconds);

        objectToDeactivate.SetActive(false);
    }
}
