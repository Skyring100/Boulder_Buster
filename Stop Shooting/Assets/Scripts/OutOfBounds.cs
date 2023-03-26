using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public float xRange;
    public float zRange;

    private void Start()
    {
        xRange = SpawnManager.GetxRange();
        zRange = SpawnManager.GetzRange();
    }
    void Update()
    {
        if (transform.position.x > xRange || transform.position.x < -xRange)
        {
            Destroy(gameObject);
        }
        else if (transform.position.z > zRange || transform.position.z < -zRange)
        {
            Destroy(gameObject);
        }
    }
}
