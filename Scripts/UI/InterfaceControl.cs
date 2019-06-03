using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InterfaceControl : MonoBehaviour {

    CharacterControl gino;
    public Image faceIndicator;
    public Sprite[] faces;
    public Image healthBar;
    public Image energyBar;
    public GameObject combatPanel;
	void Start ()
    {
        gino = gameObject.GetComponent<CharacterControl>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.timeScale == 0)
            return;

       FaceIndicator();
       HealthIndicator();
       EnergyIndicator();
        CombatPanel();
    }
    void CombatPanel()
    {
        if (gino.combatSettings.inCombat && Time.timeScale != 0)
        {
            combatPanel.SetActive(true);
        }
        else
        {
            combatPanel.SetActive(false);
        }
    }
    void FaceIndicator()
    {
        bool inCombat = gino.combatSettings.inCombat;
        bool isPumped = gino.superInput;
        bool isHidden = gino.moveSettings.isHidden;

        int i = 0;

        if (isPumped)
        {
            i = 3;
        }
        else if (isHidden)
        {
            i = 2;
        }
        else if (inCombat)
        {
            i = 1;
        }

        if (faceIndicator.sprite != faces[i])
            faceIndicator.sprite = faces[i];
    }

    void HealthIndicator()
    {
        healthBar.fillAmount = gino.vitalsSettings.health / gino.vitalsSettings.maxHealth;
    }
    
    void EnergyIndicator()
    {
        energyBar.fillAmount = gino.vitalsSettings.energy / gino.vitalsSettings.maxEnergy;
    }
}
