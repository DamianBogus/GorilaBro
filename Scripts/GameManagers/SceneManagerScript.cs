using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneManagerScript : MonoBehaviour
{
    public Image loadingBar;
    public bool startLevel;

    string main = "Main Menu";
    string loading = "Loading Scene";
    string level1 = "Level1";

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    public void LoadNewGame()
    {
        Initiate.Fade(loading, level1, Color.black, 0.5f);
    }

    IEnumerator InitializeLoadingScreen(string scene)
    {
        string currentLevel = SceneManager.GetActiveScene().name;

        while (currentLevel != loading)
            yield return null;

        GameObject progressBar = Instantiate(Resources.Load("Menu/LoadingScreen")) as GameObject;
        
        AsyncOperation loadNextLevel = SceneManager.LoadSceneAsync(scene);
        loadNextLevel.allowSceneActivation = false;

        while (loadNextLevel.progress < 0.9f)
        {
            if(loadNextLevel.progress > 0.8f)
                Initiate.FadeToLevel(scene, Color.black, 0.5f);

            yield return null;
        }

        loadNextLevel.allowSceneActivation = true;
    }
}
