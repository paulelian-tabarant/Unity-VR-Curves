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

    public GameObject coordsHintPrefab;
    public float coordsHintElevation;

    private GameObject coordsHint;
    private Quaternion curOrientation = Quaternion.identity;

    private void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        GameObject otherController = GameObject.FindGameObjectWithTag("RightController");
        otherControllerObj = otherController.GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Call procedures for drawing tool coordinates adjustment
        if (Controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (coordsHintPrefab != null) {
                coordsHint = Instantiate(coordsHintPrefab);
                coordsHint.transform.parent = otherControllerObj.transform;
                coordsHint.transform.position = otherControllerObj.transform.position + coordsHintElevation * otherControllerObj.transform.up;
            }
        }
        else if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
            float xRot = Quaternion.ToEulerAngles(trackedObj.transform.rotation).x;
            Vector3 hintEulerAngles = new Vector3(xRot, .0f, .0f);
            Quaternion localXRotation = Quaternion.EulerAngles(hintEulerAngles);
            coordsHint.transform.rotation = otherControllerObj.transform.rotation * localXRotation;
        }
        // Call procedures in order to apply changes & remove hint display
        else if (Controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
            // Save new orientation for curves drawing
            curOrientation = coordsHint.transform.rotation;
            if (coordsHint != null) {
                Destroy(coordsHint);
            }
        }
    }

    public Quaternion GetDrawingOrientation() {
        return curOrientation;
    }
}
