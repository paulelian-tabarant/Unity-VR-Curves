using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    // ref. to the object colliding with the controller
    private GameObject collidingObject;
    // ref. to the object currently being grabbed
    private GameObject objectInHand;

    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col) {
        if (collidingObject || !col.GetComponent<Rigidbody>())
            return;
        collidingObject = col.gameObject;
    }

    // Callback methods to handle rigid body's interactions with others
    public void OnTriggerEnter(Collider other) {
        SetCollidingObject(other);
    }
    private void OnTriggerStay(Collider other) {
        SetCollidingObject(other);
    }
    private void OnTriggerExit(Collider other) {
        if (!collidingObject)
            return;
        collidingObject = null;
    }


    // Handles objects grab/release in the scene
    private void GrabObject() {
        objectInHand = collidingObject;
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }
    private FixedJoint AddFixedJoint() {

        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
    private void ReleaseObject() {
        if (GetComponent<FixedJoint>()) {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        objectInHand = null;
    }

    // Update is called once per frame
    void Update () {
		if (Controller.GetHairTriggerDown()) {
            if (collidingObject) {
                GrabObject();
            }
        }

        if (Controller.GetHairTriggerUp()) {
            if (objectInHand) {
                ReleaseObject();
            }
        }
	}
}
