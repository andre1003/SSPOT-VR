using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    // UI
    public Image reticle;       // Crosshair reticle


    // Scale
    private Vector3 newScale;   // Target scale


    void Awake()
    {
        if(reticle == null)
        {
            reticle = GameObject.Find("PointerImage").GetComponent<Image>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        newScale = reticle.transform.localScale;    // Set the target scale to reticle scale when the scene starts
    }

    // Update is called once per frame
    void Update()
    {
        // If reticle current scale is different from newScale, smoothly change the crosshair the scale
        if(reticle.transform.localScale != newScale)
        {
            reticle.transform.localScale = Vector3.Lerp(reticle.transform.localScale, newScale, 20f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Set crosshair target scale.
    /// </summary>
    /// <param name="scale">Target scale.</param>
    public void SetCrosshairScale(Vector3 scale)
    {
        newScale = scale;
        Debug.Log(scale);
    }
}
