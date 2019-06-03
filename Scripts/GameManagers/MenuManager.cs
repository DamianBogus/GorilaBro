using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    string CANCEL_BUTTON = "Drop";
    
    [Header("UI Sounds")]
    public AudioClip[] uiSounds;

    [Header("Menu GameObjects")]
    public GameObject main;
    public Animator fadeScreen;
    public GameObject option;
    public Selectable[] button;

    [Header("UI Elements")]
    public Slider volumeSlider;
    public Text resolution;
    public Toggle fullscreenToggle;
    public bool fullscreen;
    public Toggle[] detail;

    SceneManagerScript sceneManager;
    //Other Variables
    AudioSource audioSource;


    public Color origButton;
    public Color highlightButton;
    bool backInput,fadeIn;
    float dPadY, dPadX;
    float alpha = 0.0f;

    void Start()
    {
        audioSource = FindObjectOfType<AudioSource>();
        volumeSlider.onValueChanged.AddListener(delegate { masterVolume(); });
        fullscreen = Screen.fullScreen;
        fullscreenToggle.isOn = fullscreen;
        resolution.text = Screen.width.ToString() + "x" + Screen.height.ToString();
        int i = QualitySettings.GetQualityLevel() - 2;
        detail[i].isOn = true;
    }
    void Update()
    {
        GetInput();
        changeDetailColour();
        if (backInput)
            openMain();
    }

    void GetInput()
    {
        backInput = Input.GetButtonDown(CANCEL_BUTTON);
    }
    
    public void menuFX()
    {
        audioSource.PlayOneShot(uiSounds[0]);
    }
    public void menuSelectFX()
    {
        audioSource.PlayOneShot(uiSounds[1]);
    }

    public void newGameFX()
    {
        audioSource.PlayOneShot(uiSounds[2]);
    }
    
    public void openMain()
    {
        if (main.activeSelf)
            return;

        fadeScreen.Play("FadingScreen");
        GameObject[] menus = { option, main };
        StartCoroutine("changeMenu", menus);
        button[0].Select();
    }
    public void openOptions()
    {
        fadeScreen.Play("FadingScreen");
        GameObject[] menus = { main, option };
        StartCoroutine("changeMenu", menus);
        button[1].Select();
    }

    IEnumerator changeMenu(GameObject[] menus)
    {
        yield return new WaitForSeconds(0.5f);

        menus[0].SetActive(false);
        menus[1].SetActive(true);
    }
    public void masterVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
    public void switchFullscreen()
    {
        fullscreen = !fullscreen;
    }

    void changeDetailColour()
    {
        for (int i = 0; i < detail.Length; i++)
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
        for (int i = 0; i < detail.Length; i++)
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
