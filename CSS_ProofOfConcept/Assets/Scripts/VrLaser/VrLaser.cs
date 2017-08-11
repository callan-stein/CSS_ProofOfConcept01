using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrLaser : MonoBehaviour
{
    public Material ValidMaterial;
    public Material InvalidMaterial;

    private bool currValid = true;

    public GameObject Beam;
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetupLaser(bool valid)
    {
        if (currValid != valid)
        {
            Beam.GetComponent<Renderer>().material = valid ? ValidMaterial : InvalidMaterial;
            currValid = valid;
        }
    }
}
