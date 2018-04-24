using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    public GameObject cameraRig;
    public float acceleration;
    private float speed;

    private SteamVR_TrackedObject trackedObj;
    // The GameObject that the trigger is currently colliding with
    private GameObject collidingObject;
    // Reference to the GameObject the player is currently holding
    private GameObject objectInHand; 

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

    void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

    // Checks if the user is holding an object, otherwise change the collidingObject
    private void SetCollidingObject(Collider col)
	{
		if (collidingObject || !col.GetComponent<Rigidbody>())
		{
			return;
		}
		collidingObject = col.gameObject;
	}

    // When the trigger collides with an object, calls the method SetCollidingObject(Collider col)
    public void OnTriggerEnter(Collider other)
	{
		SetCollidingObject(other);
	}
    
    // Keeps the good object when it is held
	public void OnTriggerStay(Collider other)
	{
		SetCollidingObject(other);
	}

    // Resets the collidingObject when it is not in contact anymore
    public void OnTriggerExit(Collider other)
	{
		if (!collidingObject)
		{
			return;
		}

		collidingObject = null;
	}

    // Sets the objectInHand, resets the collidingObject and fixes the object to the controller
	private void GrabObject()
	{
		objectInHand = collidingObject;
		collidingObject = null;
		var joint = AddFixedJoint();
		joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
	}
    
    // Fixes the object with enough force so it doesn't break
	private FixedJoint AddFixedJoint()
	{
		FixedJoint fx = gameObject.AddComponent<FixedJoint>();
		fx.breakForce = 20000;
		fx.breakTorque = 20000;
		return fx;
	}

    // Deletes the joint and resets the objectInHand
	private void ReleaseObject()
	{
		if (GetComponent<FixedJoint>())
		{
			GetComponent<FixedJoint>().connectedBody = null;
			Destroy(GetComponent<FixedJoint>());
			objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
			objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
		}
		objectInHand = null;
	}
	
	void Update () {

        // Calls the grab function when the grip buttons are pressed
		if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
		{
			if (collidingObject)
			{
				GrabObject();
			}
        }

        // Calls the release function when the grip buttons are released
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
		{
			if (objectInHand)
			{
				ReleaseObject();
			}
		}

        // Pushes the cameraRig according to the position of the finger on the touchpad. The speed can be changed.
        if (Controller.GetAxis() != Vector2.zero)
        {
            Debug.Log(gameObject.name + Controller.GetAxis());
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
            {
                speed = acceleration * 5;
            } else
            {
                speed = acceleration;
            }
            Vector3 newPosition = cameraRig.transform.position;
            newPosition += Controller.GetAxis().y * transform.forward * speed * Time.deltaTime;
            newPosition += Controller.GetAxis().x * transform.right * speed * Time.deltaTime;
            cameraRig.transform.position = newPosition;
        }
    }
}
