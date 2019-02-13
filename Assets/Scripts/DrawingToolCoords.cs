using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingToolCoords : MonoBehaviour {
    // Stores a reference to the controller object data
    private SteamVR_TrackedObject trackedObj;
    // Accessing controller input via Controller reference for ease.
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    public GameObject coordsHintPrefab;

    private GameObject coordsHint;
    private GameObject rightController;

    private void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        rightController = GameObject.FindGameObjectWithTag("RightController");
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
                SteamVR_TrackedObject rightControllerObj = rightController.GetComponent<SteamVR_TrackedObject>();
                coordsHint.transform.parent = rightControllerObj.transform;
                coordsHint.transform.position = rightControllerObj.transform.position;
            }
        }
        // Call procedures in order to apply changes & remove hint display
        else if (Controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
            if (coordsHint != null) {
                Destroy(coordsHint);
            }
        }
    }
}
