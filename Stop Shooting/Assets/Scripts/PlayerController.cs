using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float horzInput;
    private float vertInput;
    public float speed = 20.0f;
    public float xRange;
    public float zRange;

    public GameObject bullet;
    [SerializeField] private float bulletCooldown;
    private const float trueBulletCountdown = 0.25f;
    private bool bulletIsReady = true;
    private SpawnManager spM;
    private InputManager inM;
    private UIManager uiM;
    [SerializeField] private GameObject forceField;
    private ForceField ffScript;
    //[SerializeField] private AudioClip shootSFX;
    private KeyCode shootKey;
    private KeyCode pauseKey;
    private KeyCode specialBullet;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode upKey;
    private KeyCode downKey;
    private bool isDefaultKeys;
    AudioSource audioSource;
    public static bool inHome;
    private Renderer render;
    [SerializeField] private Material normalMat;
    [SerializeField] private Material powerUpMat;
    [SerializeField] private int overHeatCounter;
    [SerializeField] private int overHeatLimit;
    [SerializeField] private bool isDecrementing;
    //power up
    [SerializeField] private float noDelayPowerUpTime = 6;
    private int noDelayPowerUpCount;
    private bool hasChargedBullet;
    private bool readyToCharge;
    [SerializeField] private float chargeCooldown;

    private void Start()
    {
        transform.position = new Vector3(0,1,0);
        xRange = SpawnManager.GetxRange();
        zRange = SpawnManager.GetzRange();
        spM = GameObject.Find("Game Manager").GetComponent<SpawnManager>();
        inM = GameObject.Find("Game Manager").GetComponent<InputManager>();
        uiM = GameObject.Find("Game Manager").GetComponent<UIManager>();
        ffScript = forceField.GetComponent<ForceField>();
        bulletCooldown = 0.25f;
        shootKey = inM.GetKeyBind("shoot");
        pauseKey = inM.GetKeyBind("pause");
        specialBullet = inM.GetKeyBind("special");
        //movement keys
        leftKey = inM.GetKeyBind("left");
        rightKey = inM.GetKeyBind("right");
        upKey = inM.GetKeyBind("up");
        downKey = inM.GetKeyBind("down");
        isDefaultKeys = inM.GetDefault();
        audioSource = GetComponent<AudioSource>();
        noDelayPowerUpCount = 0;
        inHome = false;
        render = GetComponent<Renderer>();
        render.material = normalMat;
        overHeatCounter = 0;
        overHeatLimit = 7;
        isDecrementing = false;
        uiM.SetMaxCooldown(overHeatLimit);
        hasChargedBullet = false;
        readyToCharge = true;
    }
    void Update()
    {
        if (spM.GetGameState()) {
            if (Input.GetKeyDown(pauseKey))
            {
                spM.PauseGame();
            }
            //Screen wrapping
            if (transform.position.x > xRange)
            {
                transform.position = new Vector3(-xRange, 0, transform.position.z);
            }
            else if (transform.position.x < -xRange)
            {
                transform.position = new Vector3(xRange, 0, transform.position.z);
            }
            if (transform.position.z > zRange)
            {
                transform.position = new Vector3(transform.position.x, 0, -zRange);
            }
            else if (transform.position.z < -zRange)
            {
                transform.position = new Vector3(transform.position.x, 0, zRange);
            }
            if (isDefaultKeys)
            {
                horzInput = Input.GetAxis("Horizontal");
                vertInput = Input.GetAxis("Vertical");
            }
            else
            {
                horzInput = GetInput(Input.GetKey(rightKey), Input.GetKey(leftKey));
                vertInput = GetInput(Input.GetKey(upKey), Input.GetKey(downKey));
            }
            transform.Translate(new Vector3(horzInput * speed * Time.deltaTime, 0, vertInput * speed * Time.deltaTime));

            //Vector3 direction = new Vector3(horzInput , 0, vertInput ).normalized;
            //transform.Translate(direction * speed * Time.deltaTime);
            if (Input.GetKeyDown(specialBullet))
            {
                if (readyToCharge)
                {
                    hasChargedBullet = true;
                    uiM.SpecialActive();
                }
                else
                {
                    Debug.Log("NOT READY");
                }
            }
            //shooting
            if (Input.GetKey(shootKey))
            {
                //There was a bug where player can spawn a bullet in pause screen. I fixied it by checking if game is paused
                if (bulletIsReady && Time.timeScale == 1)
                {
                    if (!CheckOverHeat())
                    {
                        //if there is no power up active, increment overheat
                        if (bulletCooldown == trueBulletCountdown)
                        {
                            overHeatCounter++;
                        }
                        GameObject shot = Instantiate(bullet, transform.position, bullet.transform.rotation);
                        if (hasChargedBullet)
                        {
                            shot.GetComponent<BulletMove>().SpecialBullet();
                            hasChargedBullet = false;
                            StartCoroutine(SpecialBulletCooldown());
                        }
                        PlayShootSound();
                        StartCoroutine(BulletCooldown());
                    }
                }
            }
            if (overHeatCounter > 0 && !isDecrementing)
            {
                StartCoroutine(ReduceOverHeat());
            }
        }
    }
    private IEnumerator OverHeatEvent()
    {
        StopCoroutine(BulletCooldown());
        bulletIsReady = false;
        uiM.OverHeat();
        yield return new WaitForSeconds(4);
        bulletIsReady = true;
        uiM.NormalCooldown();
    }
    private IEnumerator ReduceOverHeat()
    {
        isDecrementing = true;
        yield return new WaitForSeconds(0.5f);
        overHeatCounter--;
        uiM.SubtractCooldown();
        isDecrementing = false;
    }
    private bool CheckOverHeat()
    {
        //if the unlimited power up is active
        if (bulletCooldown != trueBulletCountdown)
        {
            return false;
        }
        //if overheated
        else if (overHeatCounter >= overHeatLimit)
        {
            StartCoroutine(OverHeatEvent());
            return true;
        }
        else
        {
            uiM.AddCooldown();
            return false;
        }
    }
    private IEnumerator SpecialBulletCooldown()
    {
        uiM.SpecialNotReady();
        readyToCharge = false;
        yield return new WaitForSeconds(chargeCooldown);
        readyToCharge = true;
        uiM.SpecialReady();
    }
    private float GetInput(bool posKey, bool negKey)
    {
        float input;
        if (posKey && negKey)
        {
            input = 0;
        }else if (posKey)
        {
            input = 1;
        }else if (negKey)
        {
            input = -1;
        }
        else
        {
            input = 0;
        }
        return input;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boulder"))
        {
            if (!forceField.activeSelf)
            {
                spM.GameOver();
                Destroy(gameObject);
            }
        }else if (other.CompareTag("Home"))
        {
            forceField.SetActive(true);
            ffScript.Begin();
            inHome = true;
        }
        else if (other.CompareTag("Power Up"))
        {
            NoDelayPowerUp();
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Boulder") && !forceField.activeSelf)
        {
            spM.GameOver();
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Home"))
        {
            ffScript.TimeRunningOut();
            inHome = false;
        }
    }
    //in order to stop cases where forcefield goes away inside home. This happens due to accurate triggers
    //(There will be a trigger exit, but if player just barely steps out and back in, there is no trigger enter)
    
    public void TimeRunOut()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }
    public IEnumerator BulletCooldown()
    {
        bulletIsReady = false;
        yield return new WaitForSeconds(bulletCooldown);
        bulletIsReady = true;
    }
    private void PlayShootSound()
    {
        audioSource.volume = spM.GetVolume();
        audioSource.Play();
    }
    /*
    private void PlayShootSound()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = shootSFX;
        audioSource.volume = spM.GetVolume();
        audioSource.Play();
    }
    private void AudioSourceCleanUp()
    {
        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        foreach (AudioSource s in sources)
        {
            if (!s.isPlaying)
            {
                Destroy(s);
            }
        }
    }
    */
    public void NoDelayPowerUp()
    {
        bulletCooldown = 0;
        noDelayPowerUpCount++;
        bulletIsReady = true;
        StartCoroutine(NoDelayPowerUpWait());
    }
    private IEnumerator NoDelayPowerUpWait()
    {
        render.material = powerUpMat;
        yield return new WaitForSeconds(noDelayPowerUpTime);
        noDelayPowerUpCount--;
        if (noDelayPowerUpCount == 0)
        {
            bulletCooldown = trueBulletCountdown;
            render.material = normalMat;

        }
    }
    //Note: I use a counter to keep track how many times a power up has been activated. We will only return player to normal once last coroutine has ended
}
