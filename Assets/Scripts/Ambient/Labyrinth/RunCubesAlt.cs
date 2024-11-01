using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace SSPot.Ambient.Labyrinth
{
    public class RunCubesAlt : MonoBehaviourPun
    {
        [Header("Screens")]
        // Error screen GameObject
        public GameObject errorScreen;
        
        [Header("Robot Movement")]
        // Robot GameObject
        public GameObject robot;

        // Robot animation
        public Animation robotAnimation;

        // Precise movement
        public RobotPreciseMovement robotMovement;

        // Index of animation
        public int animationIndex;


        [Header("Environment")]
        // Programming slots that cube can be attached
        public List<GameObject> codingCell = new List<GameObject>();


        [Header("Sounds")]
        // Error audio
        public AudioClip error;


        [Header("Player Hand")]
        // Player hand
        public GameObject playerHand;


        [Header("Computer Commands Map")]
        // Loop commands list
        public List<string> loopCommands = new List<string>();

        // Instructions list
        public List<string> mainInstructions = new List<string>();


        // Audio source
        private AudioSource audioSource;

        // Robot animator
        private Animator robotAnimator;

        // Robot audio source
        private AudioSource robotSource;

        // Remaining iterations
        private int iteration = 0;

        // Iteration start index
        private int iterationStart = -1;

        // Iteration end index
        private int iterationEnd = -1;

        // Run command coroutine
        private Coroutine runCommandCoroutine;

        //public ComputerCellsController cellsController;

        // Start is called before the first frame update
        void Start()
        {
            /*if(cellsController == null)
                cellsController = ComputerCellsController.instance;*/

            // Setup robot audio-visual attributes
            robotAnimator = robot.GetComponent<Animator>();
            robotAnimation = robot.GetComponent<Animation>();
            robotSource = robot.GetComponent<AudioSource>();

            // Setup robot movement attributes
            animationIndex = 1;

            // Setup iteration attributes
            iterationStart = -1;
            iterationEnd = -1;

            // Setup ambient audio source
            audioSource = GetComponent<AudioSource>();
        }

        // Wait a given seconds to call NextCommand().
        private IEnumerator WaitToNextCommand(float seconds, float nextCommandDelay = 0.5f)
        {
            yield return new WaitForSeconds(seconds);

            robotAnimator.SetBool(mainInstructions[animationIndex], false);
            animationIndex++;

            yield return new WaitForSeconds(nextCommandDelay);

            // Call NextCommand method
            RunNextCommand();
        }

        // When player clicks on this object, it tries to run the algorithm.
        public void OnPointerClick()
        {
            // Try to run code
            if(PhotonNetwork.OfflineMode)
                CheckIsRunnable();
            else
                photonView.RPC("CheckIsRunnable", RpcTarget.AllBuffered);
        }

        // Run next command from the algorithm.
        [PunRPC]
        public void NextCommand()
        {
            // If have any coammand left
            if(animationIndex < (mainInstructions.Count - 1))
            {
                robotMovement.Move(mainInstructions[animationIndex]);
                robotAnimator.SetBool(mainInstructions[animationIndex], true);
                runCommandCoroutine = StartCoroutine(WaitToNextCommand(robotMovement.MovementTime));
            }
            // If doesn't have any command left
            else
            {
                // Set animationIndex to 1
                animationIndex = 1;

                // Clear cubes
                mainInstructions.Clear();

                // Reset robot animations
                robotAnimator.SetBool("Forward", false);
                robotAnimator.SetBool("Right", false);
                robotAnimator.SetBool("Left", false);
            }
        }

        // If player makes a mistake in algorithm, then the game will show the error.
        [PunRPC]
        private void Error(string errorMessage)
        {
            // Clear cubes list
            mainInstructions.Clear();

            // Play error song
            audioSource.clip = error;
            audioSource.Play();

            // Display error message to player
            errorScreen.transform.GetComponentInChildren<Text>().text = errorMessage;
            errorScreen.SetActive(true);

            // Hide error screen after 5 seconds
            StartCoroutine(WaitToDeactivate(5.0f, errorScreen));
        }

        // Run terminal code
        [PunRPC]
        private void RunCode()
        {
            // Play robot audio source and next command
            robotSource.Play();
            RunNextCommand();
        }

        // Wait a given seconds to destroy a given GameObject.
        private IEnumerator WaitToDestroy(float seconds, GameObject objectToDestroy)
        {
            yield return new WaitForSeconds(seconds);

            Destroy(objectToDestroy);
        }

        // Wait a given seconds to deactivate a given GameObject.
        private IEnumerator WaitToDeactivate(float seconds, GameObject objectToDeactivate)
        {
            yield return new WaitForSeconds(seconds);

            objectToDeactivate.SetActive(false);
        }

        // Check if code sintax is correct (don't validate the code itself)
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
            return true;
        }

        // Check if code is runnable via RPC.
        [PunRPC]
        private void CheckIsRunnable()
        {
            if(BasicCodeCheck())
                RunCode();
        }

        // Find the end of repeat loop.
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

        // Run the next instruction displayed on terminal.
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
                    //iteration = cellsController.GetRightCellAtIndex(animationIndex).GetComponent<LoopController>().iterations;
                    iterationStart = animationIndex;
                    iterationEnd = FindRepeatEnd(iterationStart);
                }
            }

            // Run next command
            NextCommand();
        }

        // Reset computer
        public void ResetComputer()
        {
            // Stop code execution
            StopExecution();

            // Reset robot animation stats
            robotAnimator.SetBool("Forward", false);
            robotAnimator.SetBool("Right", false);
            robotAnimator.SetBool("Left", false);
        }

        public void StopExecution()
        {
            // Reset animation and iteration controllers
            animationIndex = 1;
            iterationStart = -1;
            iterationEnd = -1;

            // Clear main and loop instructions
            mainInstructions.Clear();
            loopCommands.Clear();

            // Stop run command routine
            if(runCommandCoroutine != null) StopCoroutine(runCommandCoroutine);
        }
    }
}
