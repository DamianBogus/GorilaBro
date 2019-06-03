using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour {

    public GameObject coinIndicator;
    public Text coinCounter;
    CharacterControl gino;
    int prevCoins;
    float Timer;

	void Start ()
    {
        gino = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControl>();
        coinCounter = coinIndicator.transform.GetChild(0).GetComponent<Text>();
        prevCoins = gino.vitalsSettings.coins;
	}
	
	// Update is called once per frame
	void Update ()
    {
        CoinCount();
	}

    void CoinCount()
    {
        Timer += Time.deltaTime;

        if (prevCoins != gino.vitalsSettings.coins)
        {
            Timer = 0;
            prevCoins = gino.vitalsSettings.coins;
            coinIndicator.SetActive(true);
            coinCounter.text = "x" + prevCoins;
        }

        if(Timer > 2.0f)
        {
           coinIndicator.SetActive(false);  
        }
    }

}
