using UnityEngine;

public class WalkingRobot : MonoBehaviour {
    // Robot movement
    public bool forward;            // Forward movement
    public bool left;               // Left movement
    public bool right;              // Right movement


    // Robot animation
    private Animator robotAnimator; // Robot animator


    // Start is called before the first frame update
    private void Start() {
        // Set robot animator
        robotAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void FixedUpdate() {
        // Set animator's params
        robotAnimator.SetBool("Forward", forward);  // Forward param
        robotAnimator.SetBool("Left", left);        // Left param
        robotAnimator.SetBool("Right", right);      // Right param
    }
}
