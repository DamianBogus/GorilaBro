using UnityEngine;
using System.Collections;

public static class Initiate {
    //Create Fader object and assing the fade scripts and assign all the variables
    public static void Fade (string loadingScene, string levelScene, Color col,float damp){
		GameObject init = new GameObject ();
		init.name = "Fader";
		init.AddComponent<Fader> ();
		Fader scr = init.GetComponent<Fader>();
		scr.fadeDamp = damp;
		scr.fadeScene = loadingScene;
        scr.levelScene = levelScene;
		scr.fadeColor = col;
		scr.start = true;
	}
    public static void FadeToLevel(string scene, Color col, float damp)
    {
        GameObject init = new GameObject();
        init.name = "Fader";
        init.AddComponent<FaderToLevel>();
        FaderToLevel scr = init.GetComponent<FaderToLevel>();
        scr.fadeDamp = damp;
        scr.fadeScene = scene;
        scr.fadeColor = col;
        scr.start = true;
    }
}
