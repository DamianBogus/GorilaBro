using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class EnergyBar : MonoBehaviour {

    public GameObject player;
    float stamina;
    public Image StaminaBar;

	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        checkStamina();
	}

    void checkStamina()
    {
     //   stamina = player.GetComponent<CharacterControl>().vitalsSettings.stamina;

       // StaminaBar.fillAmount = stamina / 100f;

    }
}
