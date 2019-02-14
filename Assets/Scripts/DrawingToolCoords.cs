using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingToolCoords : MonoBehaviour {
    // Stores a reference to the controller object data
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_TrackedObject otherControllerObj;
    private PathDrawer pathDrawer;
    // Accessing controller input via Controller reference for ease.
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    [Header("Hint settings")]
    public GameObject coordsHintPrefab;
    public float coordsHintElevation;

    [Header("Coordinates adjustment facilities")]
    public float angleFactor = 1.0f;
    public float posFactor = .1f;

    private GameObject coordsHint;
    private Quaternion curOrientation = Quaternion.identity;
    private Quaternion drawingOrientation = Quaternion.identity;
    private Vector3 triggerDownPosition, triggerDownAxis;
    private float curPosition = .0f;
    private float drawingPosition = .0f;
    private float controllerLength = .15f;

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
                triggerDownPosition = trackedObj.transform.position;
                triggerDownAxis = trackedObj.transform.forward;
            }
        }
        else if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
            // Rotation adjustment
            Quaternion localXRotation = GetLocalXRotation(trackedObj.transform.rotation);
            coordsHint.transform.rotation = otherControllerObj.transform.rotation * localXRotation;
            // Position adjustment : positioning drawingPosition 0 at the middle of the controller length
            // then interpolated between bottom (-1) & top (+1)
            float hintPosition = GetNormalizedPosition(trackedObj.transform.position);
            coordsHint.transform.position = otherControllerObj.transform.position
                + coordsHintElevation * otherControllerObj.transform.up
                + (hintPosition - 1) * (controllerLength / 2) * otherControllerObj.transform.forward;
            // Save new coordinates for curves drawing
            curOrientation = localXRotation;
            curPosition = hintPosition;
        }
        // Call procedures in order to remove hint display & save drawing position settings
        else if (Controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
            drawingPosition = curPosition;
            drawingOrientation = curOrientation;
            if (coordsHint != null) {
                Destroy(coordsHint);
            }
            // Position axes hint according to the new drawing coordinates just set before
            pathDrawer.UpdateAxesHint();
        }
    }

    private Quaternion GetLocalXRotation(Quaternion rotation) {
        float xRot = Quaternion.ToEulerAngles(rotation).x;
        // Augment or disminish impact of left controller orientation on drawing angle adjustment
        float drawingAngle = xRot * angleFactor;
        // Adjusting drawing angle above or below 90 degrees is just nonsense
        drawingAngle = drawingAngle > Mathf.PI / 2 ? Mathf.PI / 2 : drawingAngle;
        drawingAngle = drawingAngle < -Mathf.PI / 2 ? -Mathf.PI / 2 : drawingAngle;
        Vector3 hintEulerAngles = new Vector3(drawingAngle, .0f, .0f);
        return Quaternion.EulerAngles(hintEulerAngles);
    }

    private float GetNormalizedPosition(Vector3 position) {
        Vector3 disVector = trackedObj.transform.position - triggerDownPosition;
        float disProjection = Vector3.Dot(disVector, triggerDownAxis);
        disProjection /= posFactor;
        disProjection = disProjection < -1 ? -1 : disProjection;
        disProjection = disProjection > 1 ? 1 : disProjection;
        return disProjection;
    }

    // Get a reference to the other controller in order to access its position
    private void AttachOtherController() {
        GameObject otherController = GameObject.FindGameObjectWithTag("RightController");
        if (otherController != null) {
            otherControllerObj = otherController.GetComponent<SteamVR_TrackedObject>();
            pathDrawer = otherController.GetComponent<PathDrawer>();
        }
    }

    /// <summary>
    /// Get current orientation to be used for normals
    /// </summary>
    /// <returns>
    /// The rotation to apply to the controller orientation in order to get the 
    /// actual drawing orientation
    /// </returns>
    public Quaternion GetDrawingOrientation() {
        return drawingOrientation;
    }

    /// <summary>
    /// Return the amount of displacement which should be done from the controller origin
    /// to take into account drawing position adjustments
    /// </summary>
    /// <returns>
    /// floating point number k such as the controller drawing offset on position should be set as
    /// k * controller forward vector
    /// </returns>
    public float GetDrawingPositionOffset() {
        return (drawingPosition - 1) * (controllerLength / 2);
    }
}
