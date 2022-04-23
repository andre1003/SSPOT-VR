using UnityEngine;
using UnityEngine.EventSystems;

public class AttachingCube : MonoBehaviour {
    // Player
    public GameObject playerHands;      // Player hand

    // Coding cell
    public GameObject cubeHolder;       // Coding cell cube holder
    
    // Audio
    public AudioClip selectingCube;     // Cube select audio
    public AudioClip releasingCube;     // Cube release audio


    // Selected cube
    private GameObject selectedCube;    // Selected cube GameObject

    // Audio source
    private AudioSource audioSource;    // Audio source


    // Update is called once per frame
    void Update() {
        // Set audio source
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Attach a cube to a coding cell.
    /// 
    /// <para>If there is a cube in player hand and the coding cell cube holder is free, then attach it to the clicked cube holder.</para>
    /// <para>If there is a cube in the coding cell but not in the player hand, remove the cube from coding cell.</para>
    /// </summary>
    public void Atacching() {
        // Check if player hand has a child and cube holder has no child
        if (playerHands.transform.childCount == 1 && cubeHolder.transform.childCount == 0) {
            // Set selected cube
            selectedCube = playerHands.transform.GetChild(0).gameObject;

            // Attach the selected cube to cubeHolder
            selectedCube.transform.SetParent(cubeHolder.transform);

            // Disable BoxCollider and EvenTrigger from selected cube
            selectedCube.GetComponent<BoxCollider>().enabled = false;
            selectedCube.GetComponent<EventTrigger>().enabled = false; // THIS MIGHT BE REMOVED AFTER UPGRADE

            // Set selected cube transform
            selectedCube.transform.localPosition = new Vector3(0f, 0f, 0f);     // Position
            selectedCube.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);   // Rotation
            selectedCube.transform.localScale = new Vector3(1f, 1f, 1f);        // Scale

            // Set selected cube to null
            selectedCube = null;

            // Play select cube sound
            audioSource.clip = selectingCube;
            audioSource.Play();

        } 
        // If cube holder has a child and player hand has no child 
        else if (cubeHolder.transform.childCount == 1 && playerHands.transform.childCount == 0) {
            // Destroy cube from cube holder
            Destroy(cubeHolder.transform.GetChild(0).gameObject);

            // Play release sound
            audioSource.clip = releasingCube;
            audioSource.Play();
        }
    }
}
