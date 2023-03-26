using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject player;
    public GameObject facing;
    private UIManager ui;
    public float startDelay = 2.0f;
    public float interval = 3.0f;
    public float newSpawnMulitplier = 1;
    public int spawners;
    public float timeTilNewSpawner;
    private static float xRange;
    private static float zRange;
    [SerializeField] private float safeOffset;
    private bool running;
    [SerializeField] private GameObject boulderPrefab;
    private CameraController camScript;
    private int count;
    private int score;
    private int timer;
    [SerializeField] private int growthEventValue = 1000;
    [SerializeField] private float boulderHitMultipler = 1.5f;
    [SerializeField] private float boulderTurnedMultipler = 2f;
    private float volume;
    //[SerializeField] private float timeValueMultiplier = 2f;
    //The following variables are fun facts at the end of the game
    private int bouldersTurned;
    private int shotsFired;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject[] powerUps;
    private float dropDelay;
    [SerializeField] private float powerUpTime;
    private AudioSource musicSource;
    [SerializeField] private int timeRange;
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        volume = 1;
        xRange = 30.0f;
        zRange = 16.0f;
        running = false;
        player.SetActive(false);
        facing.SetActive(false);
        ui = GetComponent<UIManager>();
        InvokeRepeating("SpawnBoulder", 1, 0.25f);
        camScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        dropDelay = 5;
        powerUpTime = Random.Range(25,timeRange);
        musicSource = GetComponent<AudioSource>();
    }
    public void StartGame()
    {
        CancelInvoke("SpawnBoulder");
        GameObject[] beginningBoulders = GameObject.FindGameObjectsWithTag("Boulder");
        foreach (GameObject b in beginningBoulders)
        {
            Destroy(b);
        }
        count = 0;
        score = 0;
        timer = 0;
        spawners =  0;
        player.SetActive(true);
        facing.SetActive(true);
        running = true;
        StartCoroutine(NewSpawnTimer());
        //Debug.Log("Max Z range: "+SpawnManager.GetzRange());
        //Debug.Log("Max X range: " + SpawnManager.GetxRange());
        InvokeRepeating("TimerIncrement", 0, 1);
    }
    
    void SpawnBoulder()
    {
            Vector3 pos;

            //choose a side of the screen to spawn on
            int randOrentation = Random.Range(0, 2);
            //Range has a max that is exclusive, so options here are actually 0-1
            //Depending on if 0 or 1, choose a vertical or horizontal side of screen
            int randSide = Random.Range(0, 2);
            //chooses left/right or up/down
            count++;
            //horz
            if (randOrentation == 0)
            {
                //left
                if (randSide == 0)
                {
                    pos.x = -SpawnManager.GetxRange() - safeOffset;
                }
                //right
                else
                {
                    pos.x = SpawnManager.GetxRange()+safeOffset;
                }
                pos.z = Random.Range(-SpawnManager.GetzRange(), SpawnManager.GetzRange());
            }
            //vert
            else
            {
                //down
                if (randSide == 0)
                {
                    pos.z = -SpawnManager.GetzRange() - safeOffset;
                }
                //up
                else
                {
                    pos.z = SpawnManager.GetzRange() + safeOffset;
                }
                pos.x = Random.Range(-SpawnManager.GetxRange(), SpawnManager.GetxRange());
            }
            pos.y = 0;
            Instantiate(boulderPrefab, pos, boulderPrefab.transform.rotation);
    }
    
    private IEnumerator SpawnPowerUp()
    {
        float safeZoneOffset = 2;
        //This is to make sure there isnt a power up spawning in the corner, where its barely visable and dangerous to get
        float xPos = Random.Range(-SpawnManager.GetxRange() + safeZoneOffset, SpawnManager.GetxRange() - safeZoneOffset);
        float zPos = Random.Range(-SpawnManager.GetzRange() + safeZoneOffset, SpawnManager.GetzRange() - safeZoneOffset);

        Vector3 spawnPos = new Vector3(xPos, 0, zPos);
        GameObject target = Instantiate(targetPrefab,spawnPos,targetPrefab.transform.rotation);
        yield return new WaitForSeconds(dropDelay);
        Instantiate(powerUps[Random.Range(0,powerUps.Length-1)],target.transform.position, target.transform.rotation);
        Destroy(target);
    }
    //this method is used to have other scripts spawn a power up in a specific postion
    private IEnumerator SpawnPowerUpAt(Vector3 spawnPos)
    {
        Debug.Log("External Access Power Up");
        GameObject target = Instantiate(targetPrefab, spawnPos, targetPrefab.transform.rotation);
        yield return new WaitForSeconds(dropDelay);
        Instantiate(powerUps[Random.Range(0, powerUps.Length - 1)], target.transform.position, target.transform.rotation);
        Destroy(target);
    }
    //The ExternalPowerUpCall is to fix a coroutine bug where if the original script that called th function is deactivated, the coroutine is canceled
    public void ExternalPowerUpCall(Vector3 pos)
    {
        StartCoroutine(SpawnPowerUpAt(pos));
    }
    private void TimerIncrement()
    {
        timer++;
        ui.UpdateTimer(timer);
        if (timer >= powerUpTime)
        {
            StartCoroutine(SpawnPowerUp());
            powerUpTime = Random.Range(powerUpTime,powerUpTime+60);
        }
    }
    public static float GetxRange()
    {
        return xRange;
    }
    public static float GetzRange()
    {
        return zRange;
    }
    public bool GetGameState()
    {
        return running;
    }
    public void GameOver()
    {
        running = false;
        CancelInvoke("TimerIncrement");
        ui.ShowEndScreen(count, shotsFired, bouldersTurned, score, timer);
    }
    public void GrowthEvent()
    {
        camScript.GrowthEvent();
        AddScore(growthEventValue);
    }
    public void BouldersTurned(int boulderNum, float size)
    {
        bouldersTurned += boulderNum;
        AddScore(size * boulderTurnedMultipler);
    }
    public void AddShot()
    {
        shotsFired++;
    }
    private void AddScore(int amount)
    {
        score += amount;
        ui.UpdateScore(score);
    }
    private void AddScore(float amount)
    {
        score += (int)amount;
        ui.UpdateScore(score);
    }
    private IEnumerator NewSpawnTimer()
    {
        InvokeRepeating("SpawnBoulder",startDelay,interval);
        spawners++;
        //Debug.Log("Spawners: "+spawners);
        interval *= newSpawnMulitplier;
        startDelay = 0;
        yield return new WaitForSeconds(timeTilNewSpawner);
        //to prevent too much strain on the system, we will restrict the amount of spawners
        if (spawners < 40)
        {
            StartCoroutine(NewSpawnTimer());
        }
    }
    public int GetSpawnerCount()
    {
        return spawners;
    }
    public void PauseGame()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            ui.PauseScreen(true);
        }
        else
        {
            Time.timeScale = 1;
            ui.PauseScreen(false);
        }
    }
    public void SetVolume(float v)
    {
        volume = v;
        musicSource.volume = v;
    }
    public float GetVolume()
    {
        return volume;
    }
}
