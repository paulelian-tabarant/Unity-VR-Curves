using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveWidth : MonoBehaviour {
    // Stores a reference to the controller object data
    private SteamVR_TrackedObject trackedObj;
    // Accessing controller input via Controller reference for ease.
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    public float minWidth = 0.01f;
    public float maxWidth = 0.1f;
    public float elevation = 0.02f;
    public GameObject widthHintPrefab;

    private float width;
    private GameObject widthHint;

    private void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start () {
        // Set a curve width default value
        width = (minWidth + maxWidth) / 2;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 touchpadAxis = Controller.GetAxis();
        if (touchpadAxis != Vector2.zero) {
            if (widthHint == null) {
                widthHint = Instantiate(widthHintPrefab);
            }
            Vector3 hintScale = widthHint.transform.localScale;
            // Get the horizontal position of the finger on touchpad
            float x = touchpadAxis[0];
            width = GetWidthValueFromX(x);

            // Reproduce the currently selected width on the hint
            // Scale is defined from middle to extremity, reason for 0.5 factor
            hintScale.y = width / 2;
            widthHint.transform.localScale = hintScale;
            widthHint.transform.parent = trackedObj.transform;
            widthHint.transform.position = trackedObj.transform.position + elevation * trackedObj.transform.up;
            widthHint.transform.rotation = trackedObj.transform.rotation 
                * Quaternion.AngleAxis(90.0f, Vector3.forward)
                * Quaternion.AngleAxis(90.0f, Vector3.right);
        }
        else if (widthHint != null) {
            Destroy(widthHint);
        }
    }

    private float GetWidthValueFromX(float x) {
        // Convert from [-1;1] to [0;1] scale
        float normVal = (x + 1) / 2;
        // Linear interpolation of [0;1] between [minWidth;maxWidth]
        float newWidth = (1 - normVal) * minWidth + normVal * maxWidth;
        return newWidth;
    }

    /// <summary>
    /// Get the curve width current value, set by the user
    /// </summary>
    /// <returns></returns>
    public float GetCurValue() {
        return width;
    }
}
