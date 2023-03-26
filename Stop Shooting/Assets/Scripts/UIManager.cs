using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private SpawnManager spM;
    private InputManager inM;
    public GameObject startScreen;
    public GameObject endScreen;
    public GameObject gameplayUI;
    public TextMeshProUGUI boulderCountText;
    public TextMeshProUGUI shotsFiredText;
    public TextMeshProUGUI boulderTurnedText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalTimerText;
    public TextMeshProUGUI liveScoreText;
    public TextMeshProUGUI timerText;
    private bool isDelay = false;
    [SerializeField] private float delay = 0.25f;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject settingsScreen;
    public TextMeshProUGUI shootKeyText;
    public TextMeshProUGUI pauseKeyText;
    public TextMeshProUGUI specialKeyText;
    public TextMeshProUGUI leftKeyText;
    public TextMeshProUGUI rightKeyText;
    public TextMeshProUGUI upKeyText;
    public TextMeshProUGUI downKeyText;
    public Slider volumeSlider;
    public Slider cooldownSlider;
    public GameObject normalCooldownImage;
    public GameObject overHeatImage;
    public GameObject warningImage;
    public GameObject specialReadyImage;
    public GameObject specialNotReadyImage;
    public GameObject specialActiveImage;
    public TextMeshProUGUI howToPlayText;
    // Start is called before the first frame update
    void Start()
    {
        startScreen.SetActive(true);
        endScreen.SetActive(false);
        gameplayUI.SetActive(false);
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
        volumeSlider.gameObject.SetActive(false);
        spM = GetComponent<SpawnManager>();
        inM = GetComponent<InputManager>();
        NormalCooldown();
        SpecialReady();
        specialActiveImage.SetActive(false);
        howToPlayText.gameObject.SetActive(false);
    }
    public void StartButtonClick()
    {
        spM.StartGame();
        startScreen.SetActive(false);
        gameplayUI.SetActive(true);
    }
    public void RestartButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Battlefield");
    }
    public void SettingsButtonClick()
    {
        startScreen.SetActive(false);
        settingsScreen.SetActive(true);
        ShowVolume();
        RefreshKeyBinds();
    }
    public void ShowTutorial()
    {
        startScreen.SetActive(false);
        howToPlayText.gameObject.SetActive(true);
    }
    public void VolumeSliderChange()
    {
        spM.SetVolume(volumeSlider.value);
        AudioSource testAudio = volumeSlider.GetComponent<AudioSource>();
        testAudio.volume = spM.GetVolume();
        testAudio.Play();
    }
    public void ChangeShootKey()
    {
        inM.ChangeKey("shoot");
    }
    public void ChangePauseKey()
    {
        inM.ChangeKey("pause");
    }
    public void ChangeSpecialKey()
    {
        inM.ChangeKey("special");
    }
    public void ChangeLeftKey()
    {
        inM.ChangeKey("left");
    }
    public void ChangeRightKey()
    {
        inM.ChangeKey("right");
    }
    public void ChangeUpKey()
    {
        inM.ChangeKey("up");
    }
    public void ChangeDownKey()
    {
        inM.ChangeKey("down");
    }
    public void RefreshKeyBinds()
    {
        shootKeyText.text = "Shoot Key: "+inM.GetKeyBind("shoot").ToString();
        pauseKeyText.text = "Pause Key: " + inM.GetKeyBind("pause").ToString();
        specialKeyText.text = "Special Bullet Key: " +inM.GetKeyBind("special").ToString();
        leftKeyText.text = "Left Key: " + inM.GetKeyBind("left").ToString();
        rightKeyText.text = "Right Key: " + inM.GetKeyBind("right").ToString();
        upKeyText.text = "Up Key: " + inM.GetKeyBind("up").ToString();
        downKeyText.text = "Down Key: " + inM.GetKeyBind("down").ToString();
    }
    public void BackToMenu()
    {
        startScreen.SetActive(true);
        settingsScreen.SetActive(false);
        howToPlayText.gameObject.SetActive(false);
        volumeSlider.gameObject.SetActive(false);
    }
    public void UpdateScore(int score)
    {
        liveScoreText.text = "Score: " + score;
    }
    public void UpdateTimer(int time)
    {
        timerText.text = "Time: "+time;
    }
    public void ShowEndScreen(int count, int shots, int turned, int score, int time)
    {
        endScreen.SetActive(true);
        gameplayUI.SetActive(false);
        boulderCountText.text = "Boulders spawned: "+ count;
        shotsFiredText.text = "Shots fired: " + shots;
        boulderTurnedText.text = "Boulders transformed: " + turned;
        finalTimerText.text = "Time: " + time;
        finalScoreText.text = "Score: " + score;
        /*
        CountVar(count,boulderCountText, "Boulders spawned");
        CountVar(hit, boulderHitText, "Boulders bounced");
        CountVar(turned, boulderTurnedText, "Boulders transformed");
        CountVar(score, finalScoreText, "Score");
        */
        //I wanted the numbers to quickly count up from 0 but unity crashed everytime i tried to do so. Look at CountVar and CountVarDelay for more info
    }
    private void CountVar(int value, TextMeshProUGUI textMesh, string text)
    {
        int increment = (int)value / 10;
        int i = 0;
        while (i <= value)
        {
            if (!isDelay)
            {
                //Debug.Log(i);
                textMesh.text = text + ": " + i;
                i += increment;
                //Unity can't handle incements by 1, so we do it by a larger number to stop crashing
                //StartCoroutine(CountVarDelay());
                //A COROUTINE INSIDE OF A WHILE LOOP CRASHES UNITY
                if (i > value)
                {
                    i = value;
                    //Debug.Log(i);
                    textMesh.text = text + ": " + i;
                    break;
                }
                //due to a Unity limitation listed above, check if the value is over incremeted. We don't want to show a higher number than what is actually there
            }
        }
    }
    public IEnumerator CountVarDelay()
    {
        isDelay = true;
        yield return new WaitForSeconds(delay);
        isDelay = false;
    }
    public void PauseScreen(bool value)
    {
        pauseScreen.SetActive(value);
        ShowVolume(value);
    }
    public void ShowVolume()
    {
        //There is a unity bug where the "slider change" is triggered every time it is enabled. I have volume that plays when changed, but it dont want it to play when enabled
        AudioSource audio = volumeSlider.GetComponent<AudioSource>();
        audio.mute = true;
        volumeSlider.gameObject.SetActive(true);
        audio.mute = false;
    }
    public void ShowVolume(bool value)
    {
        volumeSlider.gameObject.SetActive(value);
    }
    public void AddCooldown()
    {
        cooldownSlider.value++;
        if (cooldownSlider.value == cooldownSlider.maxValue-1 || cooldownSlider.value == cooldownSlider.maxValue)
        {
            warningImage.SetActive(true);
        }
    }
    public void SubtractCooldown()
    {
        cooldownSlider.value--;
        if (cooldownSlider.value < cooldownSlider.maxValue-1 && normalCooldownImage.activeSelf)
        {
            warningImage.SetActive(false);
        }
    }
    public void NormalCooldown()
    {
        warningImage.SetActive(false);
        normalCooldownImage.SetActive(true);
        overHeatImage.SetActive(false);
    }
    public void OverHeat()
    {
        normalCooldownImage.SetActive(false);
        overHeatImage.SetActive(true);
    }
    public void SetMaxCooldown(float amount)
    {
        cooldownSlider.maxValue = amount;
    }
    public void SpecialReady()
    {
        specialReadyImage.SetActive(true);
        specialNotReadyImage.SetActive(false);
    }
    public void SpecialNotReady()
    {
        specialReadyImage.SetActive(false);
        specialNotReadyImage.SetActive(true);
        CancelInvoke();
        specialActiveImage.SetActive(false);
    }
    public void SpecialActive()
    {
        InvokeRepeating("LoopSpecial",0,2);
    }
    private void LoopSpecial()
    {
        StartCoroutine(FlashSpecial());
    }
    private IEnumerator FlashSpecial()
    {
        specialActiveImage.SetActive(true);
        yield return new WaitForSeconds(1);
        specialActiveImage.SetActive(false);
    }
}
