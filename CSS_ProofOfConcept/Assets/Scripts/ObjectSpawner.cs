using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject Object;

    public int ObjectsSpawnedPerBurst = 1;

    public bool SpawnOnLoad;

    [HideInInspector]
    public bool SpawnOverTime;
    [HideInInspector]
    public float SecondsBetweenSpawns = 0.0f;
    [HideInInspector]
    public int NumberOfBursts = 5;
    [HideInInspector]
    public bool SpawnForever = false;

    private float currBurstTimer = 0.0f;
    private int currNumBursts = 0;

    // Use this for initialization
    void Start () {
        if (SpawnOnLoad)
        {
            SpawnObject(ObjectsSpawnedPerBurst);

            if (!SpawnOverTime)
            {
                Destroy(gameObject);
            }

            currNumBursts++;
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
	    currBurstTimer += Time.deltaTime;
	    if (currBurstTimer >= SecondsBetweenSpawns)
	    {
	        SpawnObject(ObjectsSpawnedPerBurst);

	        currNumBursts++;

	        if (currNumBursts >= NumberOfBursts && !SpawnForever)
	        {
	            Destroy(gameObject);
	        }

	        currBurstTimer -= SecondsBetweenSpawns;
	    }
	}

    private void SpawnObject(int numberOfObjects)
    {
        if (!Object)
        {
            return;
        }

        for (int i = 0; i < numberOfObjects; ++i)
        {
            Instantiate(Object, transform.position, transform.rotation, transform.parent);
        }
    }
}
