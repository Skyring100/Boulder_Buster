using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public GameObject player;
    private GameObject df;
    // Update is called once per frame
    private void Start()
    {
        df = GameObject.Find("Facing");
    }
    void LateUpdate()
    {
        transform.position = player.transform.position;
        Vector3 lookDirection = (df.transform.forward - transform.position).normalized;
        transform.rotation = Quaternion.Euler(lookDirection);
    }
}
