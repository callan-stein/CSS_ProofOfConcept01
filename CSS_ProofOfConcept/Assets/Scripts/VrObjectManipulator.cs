using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class VrObjectManipulator : MonoBehaviour
{
    public float BreakForce = 16000;
    public float BreakTorque = 16000;

    public float CatchTime = 0.1f;
    private float currCatchTime = 0.0f;
    private bool catching = false;

    [HideInInspector]
    public bool HoldingObject = false;

    [HideInInspector]
    public InputManager.VrControllerId Id;

    [HideInInspector]
    public VrLaserPointer LaserPointer;

    [HideInInspector]
    public Selectable CurrentFocusSelectable;
    [HideInInspector]
    public GameObject CurrentFocusGameObject;

    [HideInInspector]
    public InputManager InputManager;

    //Vr controller tracking
    private SteamVR_TrackedObject trackedObj;

    public SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        LaserPointer = GetComponent<VrLaserPointer>();
        LaserPointer.Id = Id;
    }
    
	// Update is called once per frame
	void Update ()
    {
        if (!InputManager.CanInteract())
        {
            return;
        }

        //Teleporting & drawing laser
	    if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
	    {
	        LaserPointer.TouchpadPress();
	    }

	    if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
	    {
	        LaserPointer.TouchpadPressUp();
	    }

        //Holding objects
        if (Controller.GetHairTriggerDown())
        {
            if (CurrentFocusGameObject)
            {
                InputManager.ResolveItemHoldConflicts(Id, CurrentFocusGameObject);
                HoldObject();
            }
            else
            {
                catching = true;
            }
        }

        if (catching)
        {
            currCatchTime += Time.deltaTime;

            if (currCatchTime > CatchTime)
            {
                currCatchTime = 0.0f;
                catching = false;
            }
        }

        if (Controller.GetHairTriggerUp())
        {
            if (HoldingObject)
            {
                ReleaseObject();
            }
            else
            {
                ResetCatching();
            }
        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            InputManager.ResetScene();
        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log(Id + ": " + (CurrentFocusGameObject ? CurrentFocusGameObject.name : "(Nothing)") + ", Holding = " + HoldingObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        HoverObject(other);

        if (catching && CurrentFocusGameObject)
        {
            InputManager.ResolveItemHoldConflicts(Id, CurrentFocusGameObject);
            HoldObject();
        }
    }

    void OnTriggerStay(Collider other)
    {
        HoverObject(other);
    }

    void OnTriggerExit(Collider other)
    {
        //if the object leaving us is the one we have highlighted, unless we're grabbing it we want to unhighlight it
        if (other.gameObject == CurrentFocusGameObject && !HoldingObject)
        {
            ForgetObject();
        }
    }

    private void HoverObject(Collider obj)
    {
        if (!CurrentFocusSelectable && obj.tag == "Selectable")
        {
            CurrentFocusGameObject = obj.gameObject;
            CurrentFocusSelectable = obj.GetComponent<Selectable>();

            CurrentFocusSelectable.SetControllerState(Id, Selectable.VrControllerUseState.Hover);
            InputManager.Instance.Vibrate(Id, 1.0f);
        }
    }

    private void HoldObject()
    {
        CurrentFocusSelectable.SetControllerState(Id, Selectable.VrControllerUseState.Hold);
        FixedJoint joint = AddFixedJoint();
        joint.connectedBody = CurrentFocusGameObject.GetComponent<Rigidbody>();
        HoldingObject = true;

        InputManager.Instance.VibrateForDuration(Id, 0.1f, 0.6f);

        ResetCatching();
    }

    public void ReleaseObject()
    {
        if (!CurrentFocusGameObject)
        {
            return;
        }

        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            CurrentFocusGameObject.GetComponent<Rigidbody>().velocity = Controller.velocity;
            CurrentFocusGameObject.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }

        HoldingObject = false;
        ForgetObject();
        ResetCatching();

        InputManager.Instance.VibrateForDuration(Id, 0.05f, 0.3f);
    }

    public void ForgetObject()
    {
        CurrentFocusSelectable.SetControllerState(Id, Selectable.VrControllerUseState.Neutral);
        CurrentFocusSelectable = null;
        CurrentFocusGameObject = null;
    }

    private void ResetCatching()
    {
        currCatchTime = 0.0f;
        catching = false;
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
        newJoint.breakForce = BreakForce;
        newJoint.breakTorque = BreakTorque;
        return newJoint;
    }

    private void OnJointBreak(float breakForce)
    {
        ReleaseObject();
    }

    public Vector3 CalculateHeldObjectDistance()
    {
        if (!HoldingObject)
        {
            return Vector3.zero;
        }
        return transform.position - CurrentFocusGameObject.transform.position;
    }

    public void BringHeldObjectThroughTeleport(Vector3 delta)
    {
        if (!HoldingObject)
        {
            return;
        }

        CurrentFocusGameObject.transform.position = transform.position + delta;
    }
}
