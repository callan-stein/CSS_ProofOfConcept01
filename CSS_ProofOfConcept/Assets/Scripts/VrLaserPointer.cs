using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrLaserPointer : MonoBehaviour {

    [HideInInspector]
    public InputManager.VrControllerId Id;

    public GameObject LaserPrefab;
    private GameObject laser;

    public GameObject ReticulePrefab;
    private GameObject reticule;
    private VrLaserReticule reticuleLogic;

    public float MaxTeleportDistance = 30.0f;
    public float ReticuleOffset = 0.05f;

    public LayerMask TeleportMask;

    private Vector3 lastLaserHitPoint;
    private bool lastLandingPointValid;

    private void Start()
    {
        laser = Instantiate(LaserPrefab);
        laser.SetActive(false);

        reticule = Instantiate(ReticulePrefab);
        reticuleLogic = reticule.GetComponent<VrLaserReticule>();
        reticuleLogic.Id = Id;
        reticule.SetActive(false);
    }

    public void TouchpadPress()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxTeleportDistance, TeleportMask))
        {
            lastLaserHitPoint = hit.point;

            //Landing points 
            lastLandingPointValid = hit.transform.tag == "CanTeleport";

            //Enable reticule & move it to hit point
            DrawReticule(lastLaserHitPoint, hit.normal, lastLandingPointValid);

            //Draw laser from controller to hit point
            DrawLaser(Vector3.Lerp(transform.position, lastLaserHitPoint, .5f), lastLaserHitPoint, lastLandingPointValid);
        }
        else
        {
            lastLandingPointValid = false;

            //Draw laser at max length with "Invalid" properties
            //Vector3 laserEnd = Vector3.MoveTowards(transform.position, transform.forward, MaxTeleportDistance);
            Vector3 laserEnd = transform.position + (transform.forward * MaxTeleportDistance);

            DrawLaser(Vector3.Lerp(transform.position, laserEnd, 0.5f), laserEnd, false);

            //We didn't hit anything, so we shouldn't draw the reticule
            reticule.SetActive(false);
        }
    }

    public void TouchpadPressUp()
    {
        //If we can currently teleport, do so
        //Else, disable the current laser & reticule

        if (lastLandingPointValid)
        {
            lastLandingPointValid = false;
            InputManager.Instance.PointerTeleport(Id, lastLaserHitPoint);
        }

        laser.SetActive(false);
        reticule.SetActive(false);
    }
    
    private void DrawLaser(Vector3 midPoint, Vector3 endPoint, bool valid)
    {
        laser.SetActive(true);

        laser.transform.position = midPoint;
        laser.transform.LookAt(endPoint);
        //Stretch the laser from the controller to the desired end point
        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, Vector3.Distance(transform.position, midPoint));

        laser.GetComponent<VrLaser>().SetupLaser(valid);
    }

    private void DrawReticule(Vector3 position, Vector3 surfaceNormal, bool valid)
    {
        reticule.SetActive(true);

        reticule.transform.rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal); ;

        reticule.transform.position = position + (surfaceNormal * ReticuleOffset);

        reticuleLogic.SetupReticule(valid, surfaceNormal);
    }

    public void DisableEffects()
    {
        reticule.SetActive(false);
        laser.SetActive(false);
    }
}
