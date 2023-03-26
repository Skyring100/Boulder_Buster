using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 startingPos = new Vector3(0,30,0);
    Vector3 startingRotation = new Vector3(90,0,0);
    private bool cameraTime = false;
    [SerializeField] private float cooldownTime = 3.0f;
    [SerializeField] private float shakeMagnitude = 20;
    void Start()
    {
        transform.position = startingPos;
        transform.rotation = Quaternion.Euler(startingRotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTime)
        {
            transform.Translate(new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), 0, Random.Range(-shakeMagnitude, shakeMagnitude)) * Time.deltaTime);
        }
    }
    public void GrowthEvent()
    {
        CrazyCam();
    }
    private void CrazyCam()
    {
        StartCoroutine(CrazyCamCountdown());
    }
    IEnumerator CrazyCamCountdown()
    {
        cameraTime = true;
        yield return new WaitForSeconds(cooldownTime);
        cameraTime = false;
        transform.position = startingPos;
    }
}
