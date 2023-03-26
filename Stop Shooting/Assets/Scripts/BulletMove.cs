using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float direction;
    public float speed;
    public float horzInput;
    public float vertInput;
    public GameObject df;
    private bool isStationary;
    [SerializeField] private int damage = 3;
    private SpawnManager spM;
    [SerializeField] private Material specialMat;
    void Start()
    {
        df = GameObject.Find("Facing");
        isStationary = df.GetComponent<DirectionFacer>().IsFacingOverlap();
        if (!isStationary)
        {
            transform.LookAt(df.transform, Vector3.up);
        }
        else
        {
            transform.LookAt(transform.position + df.GetComponent<DirectionFacer>().GetLastDirection(), Vector3.up);    
        }
        transform.parent = null;
        spM = GameObject.Find("Game Manager").GetComponent<SpawnManager>();
        spM.AddShot();
        //Instantiate(bulletParticles,transform.position,transform.rotation);
    }
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boulder"))
        {
            if (CompareTag("Bullet"))
            {
                Destroy(gameObject);
            }
            else
            {
                other.GetComponent<Boulder>().BecomeOrphan();
            }
        }
    }
    public int GetDamage()
    {
        return damage;
    }
    public void SpecialBullet()
    {
        tag = "Special";
        GetComponent<Renderer>().material = specialMat;
        transform.localScale += new Vector3(2,2,2);
    }
}
