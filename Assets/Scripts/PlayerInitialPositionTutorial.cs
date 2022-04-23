using UnityEngine;

public class PlayerInitialPositionTutorial : MonoBehaviour {
    // Instruction
    public GameObject initialInstructions; // Initial instructions GameObject


    // Start is called before the first frame update
    void Start() {
        transform.LookAt(initialInstructions.transform); // Make this object look at initialInstructions
    }
}
