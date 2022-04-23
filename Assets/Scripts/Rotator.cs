using UnityEngine;

public class Rotator : MonoBehaviour {
    // Rotation
    public Vector3 rotation; // Rotation vector

    // Update is called once per frame
    void Update() {
        transform.Rotate(rotation * Time.deltaTime); // Constantly rotate this object in rotation vector
    }
}
