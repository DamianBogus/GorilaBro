using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FaderToLevel : MonoBehaviour {
    [HideInInspector]
	public bool start = false;
    [HideInInspector]
    public float fadeDamp = 0.0f;
    [HideInInspector]
    public string fadeScene;
    [HideInInspector]
    public float alpha = 0.0f;
    [HideInInspector]
    public Color fadeColor;
    [HideInInspector]
    public bool isFadeIn = false;

    

    //Create a texture , Color it, Paint It , then Fade Away
    void OnGUI () {
        //Fallback check
        if (!start)
			return;
        //Assign the color with variable alpha
		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        //Temp Texture
		Texture2D myTex;
		myTex = new Texture2D (1, 1);
		myTex.SetPixel (0, 0, fadeColor);
		myTex.Apply ();
        //Print Texture
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), myTex);
        //Fade in and out control
        if (isFadeIn)
			alpha = Mathf.Lerp (alpha, -0.1f, fadeDamp * Time.deltaTime);
		else
			alpha = Mathf.Lerp (alpha, 1.1f, fadeDamp * Time.deltaTime);
        
        
        //Load scene
        if (alpha >= 1 && !isFadeIn)
        {
            isFadeIn = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (alpha <= 0 && isFadeIn)
            {
                Destroy(gameObject);
            }
        }
	}
    
}
