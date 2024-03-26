using UnityEngine;

public class GoingUp : MonoBehaviour
{
    // Movement
    public float finalY;    // Final Y value
    public float speed;     // Speed     


    // Update is called once per frame
    void Update()
    {
        // If current Y value is lower or equal the final Y value, keep going up
        if(transform.position.y <= finalY)
            transform.Translate(0f, speed * Time.deltaTime, 0f, Space.World);
        // Else, stop movement and disable this
        else
            this.enabled = false;
    }
}
