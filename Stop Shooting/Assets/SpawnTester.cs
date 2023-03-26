using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTester : MonoBehaviour
{

    public GameObject box;
    private Vector3 spawnPos = new Vector3(0, 0, 0);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) {
            Instantiate(box, spawnPos, box.transform.rotation);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            Destroy(box);
        }
    }
}
