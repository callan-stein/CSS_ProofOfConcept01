using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public enum VrControllerId
    {
        Left,
        Right
    }

    public GameObject LeftControllerObject;
    public GameObject RightControllerObject;

    public Transform CameraRigTransform;
    public Transform HeadTransform;

    private VrObjectManipulator _leftObjectManipulator;
    private VrObjectManipulator _rightObjectManipulator;

    private FadeManager _fadeManager;

    private IEnumerator vibrateCoroutine;

    public static InputManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _leftObjectManipulator = LeftControllerObject.GetComponent<VrObjectManipulator>();
        _leftObjectManipulator.InputManager = this;
        _leftObjectManipulator.Id = VrControllerId.Left;

        _rightObjectManipulator = RightControllerObject.GetComponent<VrObjectManipulator>();
        _rightObjectManipulator.InputManager = this;
        _rightObjectManipulator.Id = VrControllerId.Right;

        _fadeManager = GetComponent<FadeManager>();
    }

    private void Start()
    {
        _fadeManager.FadeInOnLoad();
    }

    public VrObjectManipulator GetVrController(VrControllerId id)
    {
        return id == VrControllerId.Left ? _leftObjectManipulator : _rightObjectManipulator;
    }

    public void ResolveItemHoldConflicts(VrControllerId id, GameObject item)
    {
        //Get the controller that ISN'T the one requesting the drop (IE: The opposide of "id")
        VrObjectManipulator otherController = GetVrController(id == VrControllerId.Left ? VrControllerId.Right : VrControllerId.Left);

        //If the other controller is holding the item that requesting controller wants to grab...
        if (otherController.HoldingObject && otherController.CurrentFocusGameObject == item)
        {
            otherController.ReleaseObject();
        }
    }

    public void Vibrate(VrControllerId id, float magnitude)
    {
        GetVrController(id).Controller.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, magnitude));
    }

    public void VibrateForDuration(VrControllerId id, float duration, float magnitude)
    {
        vibrateCoroutine = VibrateCoroutine(id, duration, magnitude);
        StartCoroutine(vibrateCoroutine);
    }

    IEnumerator VibrateCoroutine(VrControllerId id, float duration, float magnitude)
    {
        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            GetVrController(id).Controller.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, magnitude));
            yield return null;
        }
    }

    public void PointerTeleport(VrControllerId id, Vector3 laserHitPoint)
    {
        //This retains the position of the user's head relative to the play area
        Vector3 difference = CameraRigTransform.position - HeadTransform.position;
        difference.y = 0;

        //Find how far each controller was from its respective held objects
        Vector3 leftHeldDelta = _leftObjectManipulator.CalculateHeldObjectDistance();
        Vector3 rightHeldDelta = _rightObjectManipulator.CalculateHeldObjectDistance();

        //Move the entire SteamVR rig
        CameraRigTransform.position = laserHitPoint + difference;
        
        //Restore the left-behind held objects to their previous positions, relative to the controllers
        _leftObjectManipulator.BringHeldObjectThroughTeleport(leftHeldDelta);
        _rightObjectManipulator.BringHeldObjectThroughTeleport(rightHeldDelta);

        VibrateForDuration(id, 0.05f, 0.5f);
        _fadeManager.Flicker();
    }

    public void ResetScene()
    {
        _leftObjectManipulator.LaserPointer.DisableEffects();
        _rightObjectManipulator.LaserPointer.DisableEffects();

        _leftObjectManipulator.ReleaseObject();
        _rightObjectManipulator.ReleaseObject();

        _fadeManager.ResetScene();
    }

    public bool CanInteract()
    {
        return !_fadeManager.Loading;
    }
}
