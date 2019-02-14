using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingToolCoords : MonoBehaviour {
    // Stores a reference to the controller object data
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_TrackedObject otherControllerObj;
    // Accessing controller input via Controller reference for ease.
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    [Header("Hint settings")]
    public GameObject coordsHintPrefab;
    public float coordsHintElevation;

    [Header("Angle adjustment")]
    public float adjustFactor = 1;

    private GameObject coordsHint;
    private Quaternion curOrientation = Quaternion.identity;

    private void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        AttachOtherController();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        // Call procedures for drawing tool coordinates adjustment
        if (Controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (otherControllerObj == null) {
                AttachOtherController();
            }
            if (coordsHintPrefab != null && otherControllerObj != null) {
                coordsHint = Instantiate(coordsHintPrefab);
                coordsHint.transform.parent = otherControllerObj.transform;
                coordsHint.transform.position = otherControllerObj.transform.position + coordsHintElevation * otherControllerObj.transform.up;
            }
        }
        else if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
            float xRot = Quaternion.ToEulerAngles(trackedObj.transform.rotation).x;
            // Augment or disminish impact of left controller orientation on drawing angle adjustment
            float drawingAngle = xRot * adjustFactor;
            // Adjusting drawing angle above or below 90 degrees is just nonsense
            drawingAngle = drawingAngle > Mathf.PI / 2 ? Mathf.PI / 2 : drawingAngle;
            drawingAngle = drawingAngle < -Mathf.PI / 2 ? -Mathf.PI / 2 : drawingAngle;
            Vector3 hintEulerAngles = new Vector3(drawingAngle, .0f, .0f);
            Quaternion localXRotation = Quaternion.EulerAngles(hintEulerAngles);
            coordsHint.transform.rotation = otherControllerObj.transform.rotation * localXRotation;
            // Save new orientation for curves drawing
            curOrientation = localXRotation;
        }
        // Call procedures in order to remove hint display
        else if (Controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
            if (coordsHint != null) {
                Destroy(coordsHint);
            }
        }
    }

    // Get a reference to the other controller in order to access its position
    private void AttachOtherController() {
        GameObject otherController = GameObject.FindGameObjectWithTag("RightController");
        otherControllerObj = otherController.GetComponent<SteamVR_TrackedObject>();
    }

    /// <summary>
    /// Get current orientation to be used for normals
    /// </summary>
    /// <returns></returns>
    public Quaternion GetDrawingOrientation() {
        return curOrientation;
    }
}
