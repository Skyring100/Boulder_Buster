using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionFacer : MonoBehaviour
{
    public GameObject player;
    public Vector3 offsetVector;
    public float offsetNum = 10;
    private bool isMatching;
    private float checkingOffset = 0.5f;
    private SpawnManager spM;
    private Vector3 lastDirection;
    private void Start()
    {
        isMatching = true;
        spM = GameObject.Find("Game Manager").GetComponent<SpawnManager>();
    }
    private void Update()
    {
        if (spM.GetGameState()) {
            Vector3 playerPos = player.transform.position;
            float horzInput = Input.GetAxis("Horizontal");
            float vertInput = Input.GetAxis("Vertical");
            if (horzInput != 0)
            {
                offsetVector.x = offsetNum * horzInput;
            }
            if (vertInput != 0)
            {
                offsetVector.z = offsetNum * vertInput;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void LateUpdate()
    {
        if (spM.GetGameState()) {
            transform.position = player.transform.position + offsetVector;
            isMatching = IsInRange(transform.position, player.transform.position);
            if (!isMatching)
            {
                lastDirection = offsetVector;
            }
            else
            {
                transform.position = player.transform.position;
            }
        }
    }
    public bool IsFacingOverlap()
    {
        return isMatching;
    }

    //When moving vectors, they aren't acurate enough to just see if they are identical
    private bool IsInRange(Vector3 facingPos, Vector3 playerPos)
    {
        //Checking if facing postion is around the same as player's
        if(facingPos.x < playerPos.x + checkingOffset && facingPos.x > playerPos.x - checkingOffset)
        {
            if (facingPos.z < playerPos.z + checkingOffset && facingPos.z > playerPos.z - checkingOffset)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public Vector3 GetLastDirection()
    {
        lastDirection.x = Mathf.Round(lastDirection.x);
        lastDirection.z = Mathf.Round(lastDirection.z);
        return lastDirection;
    }
}
