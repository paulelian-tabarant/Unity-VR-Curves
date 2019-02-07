using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveControllerInputTest : MonoBehaviour {

    // stores a reference to the controller object data
    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller 
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update () {
        // gets the position of the finger on touchpad
        if (Controller.GetAxis() != Vector2.zero) {
            Debug.Log(gameObject.name + Controller.GetAxis());
        }

        // back trigger inputs
        if (Controller.GetHairTriggerDown()) {
            Debug.Log(gameObject.name + " Trigger Press");
        }
        if (Controller.GetHairTriggerUp()) {
            Debug.Log(gameObject.name + " Trigger Release");
        }

        // side grip inputs
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
            Debug.Log(gameObject.name + " Grip Press");
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {
            Debug.Log(gameObject.name + " Grip Release");
        }
	}
}
