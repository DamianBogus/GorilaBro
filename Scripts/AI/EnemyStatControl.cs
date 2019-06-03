using UnityEngine;
using System.Collections;

public class EnemyStatControl : MonoBehaviour
{

    [System.Serializable]
    public class VitalsSettings
    {
        //Health and Energy Area
        public float health = 100;
        public float maxHealth = 100;
        public bool dead = false;
    }

    GameObject healthDrop;
    public VitalsSettings vitalsSettings = new VitalsSettings();
    float timer;
    float currentHealth;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        checkHealth();
    }

    void checkHealth()
    {

        if (vitalsSettings.health <= 0.0f && !vitalsSettings.dead)
        {
            vitalsSettings.dead = true;
            gameObject.tag = "deadEnemy";
            healthDrop = Instantiate(Resources.Load("healthDrop")) as GameObject;
            healthDrop.transform.position = transform.position + Vector3.up;
        }

        if (vitalsSettings.dead)
        {
            timer += Time.deltaTime;
            if (timer > 3.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}

