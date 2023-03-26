using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform target;
    private static List<GameObject> allTargets = new List<GameObject>();
    private static List<GameObject> followers = new List<GameObject>();
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 1.75f, 0);
    private Boulder targetScript;
    private GameObject shield;
    private float shieldY;
    private float shieldZ;
    public bool shieldActive;
    private void Start()
    {
        shield = transform.GetChild(1).gameObject;
        shieldY = shield.transform.localScale.y;
        shieldZ = shield.transform.localScale.z;
        shieldActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
        }else if (targetScript.isHouse || target.parent)
        {
            //If there is a target, see if it's a house or has a parent (we dont need health bars for homes and we dont need multiple health bars on a boulder)
            Destroy(gameObject);
        }
        else
        {
            transform.position = target.position + healthBarOffset;
        }
    }
    public void SetTarget(GameObject obj)
    {
        target = obj.transform;
        targetScript = target.GetComponent<Boulder>();
        allTargets.Add(obj);
        followers.Add(gameObject);
    }
    public static GameObject IsTarget(GameObject obj)
    {
        foreach (GameObject t in allTargets)
        {
            if (t==obj)
            {
                //Already has health bar
                return followers[allTargets.IndexOf(t)];  
            }
        }
        //Does not have health bar
        return null;
    }
}
