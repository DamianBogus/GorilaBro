using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{

    string PAUSE_BUTTON = "Pause";
    string CANCEL_BUTTON = "Drop";
    string DPAD_AXIS = "DpadX";

    [Header("Music Files")]
    public AudioClip[] music;

    [Header("UI Sounds")]
    public AudioClip[] uiSounds;

    [Header("Menu GameObjects")]
    public GameObject pauseMenu;
    public GameObject playerHUD;
    public GameObject levelWin;
    public GameObject[] menus;
    public Selectable[] button;

    [Header("Graphics Elements")]
    public Slider volumeSlider;
    public Text resolution;
    public Toggle fullscreenToggle;
    public bool fullscreen;
    public Toggle[] detail;

    [Header("Skill Elements")]
    public Text Coins;
    public Text Cost;
    public Button health;
    public Button strength;
    public Button energy;
    public Image strengthBar;
    public Image healthBar;
    public Image energyBar;
    public bool str;
    public bool eng;
    public bool hp;
    public bool increaseAtt;
    
    [Header("Level Complete Elements")]
    public Text coinCount;
    public Text enemyText;
    public Text gameTime;
    public Text levelGrade;

    public GameObject[] enemies;
    CharacterControl player;
    //Other Variables
    AudioSource audioSource;
    [HideInInspector]
    public bool levelCompleted;

    bool inMenu, pauseInput, backInput;
    float dPadY, dPadX, levelTime, total;
    float attributeCost = 20;
 
    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        volumeSlider.onValueChanged.AddListener(delegate { masterVolume(); });
        
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControl>();

    }
	void Update ()
    {
        if (levelCompleted)
            return;

        GetInput();
        checkMenu();
        levelTimer();
        changeDetail();
        AttributeAdjust();
        UpdateCost();
        UpdateMusic();
        if (backInput)
            backMenu();
    }

    void levelTimer()
    {
        if (Time.timeScale == 0)
            return;

        levelTime += Time.deltaTime;
    }

    void levelComplete()
    {
       // Time.timeScale = 0;
        float coins = player.vitalsSettings.coins;

        int enemyCount = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
                enemyCount++;
        }

        enemyCount = enemies.Length - enemyCount;
        float finalScore = (coins * 10.0f - enemyCount);
        string grade = "A";

        if (finalScore == 300.0f)
            grade = "A*";

        if (finalScore == 200.0f)
            grade = "A";

        if (finalScore == 100.0f)
            grade = "B";

        if (finalScore == 50.0f)
            grade = "C";

        if (finalScore < 50.0f)
            grade = "F";


        levelGrade.text = grade;
        coinCount.text = "x" + coins.ToString();
        enemyText.text = "x" + enemyCount.ToString();
        float minutes = Mathf.Round(levelTime / 60);
        float seconds = Mathf.Round(levelTime % 60);

        string minuteString = "";
        string secondString = "";

        if (minutes < 9)
        {
            minuteString = "0" + minutes.ToString();
        }
        else
        {
            minuteString = minutes.ToString();
        }

        if (seconds < 9)
        {
            secondString = "0" + seconds.ToString();
        }
        else
        {
            secondString = seconds.ToString();
        }

        gameTime.text = minuteString + ":" + secondString;




        levelWin.SetActive(true);
        audioSource.clip = music[1];
        audioSource.pitch = 1.2f;
        audioSource.volume = 0.8f;
        audioSource.Play();

        StartCoroutine("BackToMenu");
    }
    void GetInput()
    {
        pauseInput = Input.GetButtonDown(PAUSE_BUTTON);
        backInput = Input.GetButtonDown(CANCEL_BUTTON);
        dPadX = Input.GetAxis(DPAD_AXIS);
    }
    void checkMenu()
    {
        if (pauseInput)
            inMenu = !inMenu;

        if (pauseInput && inMenu)
        {
            openMenu();
        }
        if (pauseInput && !inMenu)
        {
            closeMenu();
        }
    }
    //MUSIC FUNCTIONS
    void UpdateMusic()
    {
        if (pauseMenu.activeSelf)
        {
            audioSource.clip = music[2];
            audioSource.pitch = 1.0f;
            audioSource.volume = 1.0f;
        }
        else if (player.superInput && audioSource.clip != music[1])
        {
            audioSource.clip = music[1];
            audioSource.pitch = 1.2f;
            audioSource.volume = 0.8f;
        }
        else if (!player.superInput && audioSource.clip != music[0])
        {
            audioSource.clip = music[0];
            audioSource.pitch = 1.0f;
            audioSource.volume = 1.0f;
        }

        if (!audioSource.isPlaying)
            audioSource.Play();
    }
    //MENU FUNCTIONS
    void backMenu()
    {
        if (!pauseMenu.activeSelf)
            return;

        int menu = 0;

        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].activeSelf == true)
            {
                menu = i;
                break;
            }
        }

        if(menu == 0)
        {
            closeMenu();
        }
        else
        {
            menus[menu].SetActive(false);
            audioSource.PlayOneShot(uiSounds[1]);
            menus[0].SetActive(true);
            button[0].Select();
        }

    }
    void openMenu()
    {
        Time.timeScale = 0;
        playerHUD.SetActive(false);
        pauseMenu.SetActive(true);
        menus[0].SetActive(true);
        button[0].Select();
        audioSource.PlayOneShot(uiSounds[0]);
    }
    public void closeMenu()
    {
        inMenu = false;
        Time.timeScale = 1;
        audioSource.PlayOneShot(uiSounds[1]);
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
        }
        pauseMenu.SetActive(false);
        playerHUD.SetActive(true);
    }

    public void menuOptions()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
        }
        audioSource.PlayOneShot(uiSounds[1]);
        menus[1].SetActive(true);
        button[1].Select();

        fullscreen = Screen.fullScreen;
        fullscreenToggle.isOn = fullscreen;
        resolution.text = Screen.width.ToString() + "x" + Screen.height.ToString();
        int qual = QualitySettings.GetQualityLevel() - 2;
        detail[qual].isOn = true;
    }

    public void menuSkills()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
        }
        audioSource.PlayOneShot(uiSounds[1]);
        menus[2].SetActive(true);
        button[2].Select();

        Coins.text = "Coins x" + player.vitalsSettings.coins;
        strengthBar.fillAmount = player.vitalsSettings.strengthLevel / 8;
        healthBar.fillAmount = player.vitalsSettings.healthLevel / 8;
        energyBar.fillAmount = player.vitalsSettings.energyLevel / 8;
    }
    //SKILL FUNCTIONS
    public void strengthSelect()
    {
        str = !str;
    }
    public void energySelect()
    {
        eng = !eng;
    }
    public void healthSelect()
    {
        hp = !hp;
    }
    void AttributeAdjust()
    {

        if (dPadX == 0.0)
            increaseAtt = true;

        Coins.text = "Coins x" + player.vitalsSettings.coins;
        if (!menus[2].activeSelf || !increaseAtt)
            return;

        if (dPadX != 0.0)
            increaseAtt = false;

        if (eng)
        {
            if (dPadX < 0 && energyBar.fillAmount * 8 == player.vitalsSettings.energyLevel)
            {
            }
            else
            {
                energyBar.fillAmount += 0.125f * dPadX;
            }

        }
        if (str)
        {
            if (dPadX < 0 && strengthBar.fillAmount * 8 == player.vitalsSettings.strengthLevel)
            {
            }
            else
            {
                strengthBar.fillAmount += 0.125f * dPadX;
            }
        }
        if (hp)
        {
            if (dPadX < 0 && healthBar.fillAmount * 8 == player.vitalsSettings.healthLevel)
            {
            }
            else
            {
                healthBar.fillAmount += 0.125f * dPadX;
            }
        }
    }
    void UpdateCost()
    {
        if (!menus[2].activeSelf)
            return;

        total = (energyBar.fillAmount * 8) - player.vitalsSettings.energyLevel;
        total += (healthBar.fillAmount * 8) - player.vitalsSettings.healthLevel;
        total += (strengthBar.fillAmount * 8) - player.vitalsSettings.strengthLevel;

        total *= attributeCost;

        Cost.text = "Cost x" + total;
        if(total > player.vitalsSettings.coins)
        {
            Cost.color = Color.red;
        }
        else
        {
            Cost.color = Color.white;
        }
    }

    public void BuyAttributes()
    {
        if (total == 0 || total > player.vitalsSettings.coins)
            return;

        player.vitalsSettings.coins -= (int) total;
        player.vitalsSettings.strengthLevel = strengthBar.fillAmount * 8;
        player.vitalsSettings.healthLevel = healthBar.fillAmount * 8;
        player.vitalsSettings.energyLevel = energyBar.fillAmount * 8;

        player.UpdateAttributes();
    }
    //SOUND FUNCTIONS
    public void menuFX()
    {
        audioSource.PlayOneShot(uiSounds[0]);
    }
    public void menuSelectFX()
    {
        audioSource.PlayOneShot(uiSounds[2]);
    }

    //GRAPHIC/SOUND FUNCTIONS
    public void masterVolume()
    {
        menuFX();
        AudioListener.volume = volumeSlider.value;
    }
    public void switchFullscreen()
    {
        fullscreen = !fullscreen;
    }
    
    void changeDetail()
    {
        for(int i=0; i < detail.Length; i++)
        {
            ColorBlock colors = detail[i].colors;
            if (detail[i].isOn)
            {
                colors.normalColor = Color.red;
            }
            else
            {
                colors.normalColor = Color.white;
            }

            detail[i].colors = colors;
        }
    }
    public void applyGraphics()
    {
        string[] res = resolution.text.Split('x');
        int width = int.Parse(res[0]);
        int height = int.Parse(res[1]);
        int detailLevel = 3;
        for(int i=0; i < detail.Length; i++)
        {
            if (detail[i].isOn)
            {
                detailLevel = i + 2;
                break;
            }
        }

        Screen.SetResolution(width, height, fullscreen, 60);
        QualitySettings.SetQualityLevel(detailLevel);
    }

    
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            levelCompleted = true;
            levelComplete();
        }
    }

    IEnumerator BackToMenu()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        Initiate.Fade("Loading Scene", "Main Menu", Color.black, 0.5f);

    }
}
