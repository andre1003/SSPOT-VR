using UnityEngine;

public class CloningCube : MonoBehaviour
{
    // Player
    public GameObject playerHands;      // Player hands GameObject

    // Audio source
    public AudioSource audioSource;     // Audio source

    // Selected cube
    private GameObject selectedCube;    // Selected cube GameObject

    /// <summary>
    /// When player clicks on this object, it clones the selected cube to player's hand
    /// </summary>
    public void OnPointerClick()
    {
        // If the hand is not empty
        if(playerHands.transform.childCount != 0)
        {
            // Clear the hand
            Destroy(playerHands.transform.GetChild(0).gameObject);
            audioSource.Play();
        }

        // Attach the selected cube to the player's hand
        AttachCubeToHand();
    }

    private void AttachCubeToHand()
    {
        // Instanciate the selected object
        selectedCube = Instantiate(gameObject);

        // Attach to player's hand
        selectedCube.transform.SetParent(playerHands.transform);

        // Set cube transform at player's hand
        selectedCube.transform.localPosition = new Vector3(0f, -0.5f, 0.75f);   // Position
        selectedCube.transform.rotation = Quaternion.Euler(72f, 0f, 0f);        // Rotation
        selectedCube.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);      // Scale
    }
}
