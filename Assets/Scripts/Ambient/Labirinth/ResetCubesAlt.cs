using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace SSpot.Labirinth
{
    //TODO delete
    public class ResetCubesAlt : MonoBehaviourPun
    {
        // Terminal coding cells
        public List<GameObject> codingCell = new List<GameObject>();
        public List<GameObject> leftCells = new List<GameObject>();
        public List<GameObject> rightCells = new List<GameObject>();

        [SerializeField] private RunCubesAlt runCubes;

        // Audio source
        private AudioSource audioSource;


        // Start is called before the first frame update
        void Awake()
        {
            // Set audio source
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// When player clicks on this object, it reset all coding cells.
        /// </summary>
        public void OnPointerClick()
        {
            ResetCallback();
        }

        // Reset callback
        public void ResetCallback()
        {
            if(PhotonNetwork.OfflineMode)
                ExecuteReset();
            else
                photonView.RPC("ExecuteReset", RpcTarget.AllBuffered);
        }

        // Reset method
        [PunRPC]
        private void ExecuteReset()
        {
            // Call ResetBlocks method
            ResetBlocks();

            // If there is a cube in player's hand
            PlayerSetup.instance.DestroyCubeOnHand();
        }

        /// <summary>
        /// Reset all coding cells of terminal.
        /// </summary>
        private void ResetBlocks()
        {
            // Check every coding cell
            for(int i = 0; i < codingCell.Count; i++)
            {
                // If the coding cell has a child
                if(codingCell[i].transform.childCount > 0)
                {
                    // Clear this cell
                    Destroy(codingCell[i].transform.GetChild(0).gameObject);
                }
            }

            for(int i = 0; i < leftCells.Count; i++)
            {
                if(leftCells[i].transform.childCount > 0)
                {
                    Destroy(leftCells[i].transform.GetChild(0).gameObject);
                    leftCells[i].transform.parent.gameObject.SetActive(false);
                }
            }

            for(int i = 0; i < rightCells.Count; i++)
            {
                rightCells[i].SetActive(false);
            }

            // Stop robot movment and reset its position
            runCubes.robotMovement.StopMovement();
            //ResetLabyrinth.Instance?.ResetRobotPosition();

            // Play audio source
            audioSource.Play();

            // Reset cubes list from RunCubes script
            runCubes.ResetComputer();
        }
    }
}
