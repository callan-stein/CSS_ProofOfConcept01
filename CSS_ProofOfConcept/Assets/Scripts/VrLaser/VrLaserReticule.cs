using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrLaserReticule : MonoBehaviour
{
    [HideInInspector]
    public InputManager.VrControllerId Id;

    public Material ValidMaterial;
    public Material InvalidMaterial;

    public AnimationCurve RotationAnim;
    public AnimationCurve ScaleAnim;

    public float LoopTime = 1.0f;

    public float RotationSpeed = 180.0f;

    public float ScaleSpeed = 1.0f;

    private float currLoopTime = 0.0f;

    private Vector3 defaultScale;

    private Vector3 normal;

    private bool currValid = false;

    public float ValidVibrationsPerSecond = 2.0f;

    private float currVibrateTime = 0.0f;
    
    void Start()
    {
        defaultScale = transform.localScale;
        normal = transform.right;
    }

	// Update is called once per frame
	void Update ()
	{
	    float rotationNum = RotationAnim.Evaluate(currLoopTime / LoopTime);

        transform.RotateAround(transform.position, normal, RotationSpeed * rotationNum);

	    float scaleNum = ScaleAnim.Evaluate(currLoopTime / LoopTime);
        transform.localScale = defaultScale * (scaleNum * ScaleSpeed);

	    currLoopTime += Time.deltaTime;

	    if (currLoopTime >= LoopTime)
	    {
	        currLoopTime = 0.0f;
	    }

	    if (currValid)
	    {
	        currVibrateTime += Time.deltaTime;
	        if (currVibrateTime >= 1.0f / ValidVibrationsPerSecond)
	        {
	            InputManager.Instance.Vibrate(Id, 1.0f);
	            currVibrateTime = 0.0f;
	        }
        }
	}

    public void SetupReticule(bool valid, Vector3 normalIn)
    {
        normal = normalIn;

        if (currValid != valid)
        {
            this.GetComponent<Renderer>().material = valid ? ValidMaterial : InvalidMaterial;
            currValid = valid;

            //This checks to see if the laser is turning valid from being invalid last frame. If so, we vibrate the controller slightly
            if (currValid)
            {
                InputManager.Instance.Vibrate(Id, 1.0f);
            }
            else
            {
                currVibrateTime = 0.0f;
            }
        }
    }

    public void ResetReticule()
    {
        currValid = false;
        currLoopTime = 0.0f;
        currVibrateTime = 0.0f;
    }
}
