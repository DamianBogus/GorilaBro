using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDetect : MonoBehaviour
{
 
    //ENEMY COMPONENTS
    public GameObject indicator;
    public EnemyControl enemyControl;

    //ALLY COMPONENTS
    public List<GameObject> allies;
    public EnemyDetect[] alliesNav;

    //OTHER COMPONENTS
    Transform cam;
    GameObject playerTarget;
    GameObject player;
    CharacterControl playerControl;
    LayerMask mask = (1 << 8) | (1 << 11);

    //PLAYER DETECTION VARIABLES
    [HideInInspector]
    public Vector3 lastKnownPos;
   // [HideInInspector]
    public bool playerSpotted;
    public bool playerDetected;
    public bool playerFound;

    bool inLineOfSight;
    bool sightObstructed;
    float detectionTimer = 0;
    float detectionDuration = 0;
    float detectionCooldown = 5.0f;
    float detectionDistance = 20.0f;
    Color indicatorColor;
    public GameObject[] alliesArray;

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        enemyControl = transform.root.gameObject.GetComponent<EnemyControl>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerControl = player.GetComponent<CharacterControl>();
        indicator = transform.GetChild(0).gameObject;
        playerTarget = GameObject.FindGameObjectWithTag("CameraTarget");
        indicatorColor = indicator.GetComponent<Renderer>().material.color;
        mask = ~mask;
        GetAllies();
    }

    void GetAllies()
    {
        alliesArray = (GameObject.FindGameObjectsWithTag("enemyDetector"));

        for (int i=0; i < alliesArray.Length; i++)
        {
            if (alliesArray[i].GetComponent<Rigidbody>())
                allies.Add(alliesArray[i]);
        }

        allies.Remove(gameObject);
        alliesNav = new EnemyDetect[allies.Count];
        for(int i=0; i < allies.Count; i++)
        {
            alliesNav[i] = allies[i].GetComponent<EnemyDetect>();
        }
    }

    void Update()
    {
        DetectPlayer();
        TrackPlayer();
        AllyDetectPlayer();
        playerindicatorPoint();
    }

    void playerindicatorPoint()
    {
        if (!indicator)
            return;
        
        indicator.transform.LookAt(cam.position);

    }
    void DetectPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
       
        if (dist > detectionDistance)
            return;

        CheckLineOfSight();

        if (playerFound)
          return;


        if (inLineOfSight && !sightObstructed)
        {
            detectionTimer += Time.deltaTime;
            playerSpotted = true;
            indicator.SetActive(true);
        }
        else
        {
            detectionTimer -= Time.deltaTime;
            if (detectionTimer < 0)
            {
                detectionTimer = 0;
                indicator.SetActive(false);
                playerSpotted = false;
            }
        }

        if (dist > 6)
        {
            detectionDuration = dist / 6.0f;
        }
        else
        {
            detectionDuration = 1.0f;
        }
        indicatorColor.g = (1 - (detectionTimer / detectionDuration));

        if (detectionTimer > detectionDuration)
        {
            detectionTimer = 0;
            playerFound = true;
            indicatorColor = Color.red;
        }


        if (indicator && indicator.GetComponent<Renderer>().material.color != indicatorColor)
            indicator.GetComponent<Renderer>().material.color = indicatorColor;
    }
    
    void CheckLineOfSight()
    {
            Vector3 detectionZone = (player.transform.position - transform.position).normalized;
            Vector3 playerPos = playerTarget.transform.position;
            RaycastHit hit;

        if (Physics.Linecast(transform.position, playerPos, out hit, mask) || playerControl.moveSettings.isHidden)
        {
            sightObstructed = true;
        }
        else if(!playerControl.moveSettings.isHidden)
        {
            sightObstructed = false;
        }


        if (Vector3.Dot(detectionZone, transform.forward) >= 0.2f)
        {
            inLineOfSight = true;
        }
        else
        {
            inLineOfSight = false;
        }
    }

    void TrackPlayer()
    {
        if (!playerFound)
            return;

        if ((inLineOfSight && !sightObstructed) || player.GetComponent<CharacterControl>().combatSettings.inCombat)
        {
            lastKnownPos = player.transform.position;
            playerDetected = true;
            detectionTimer = 0;
            indicatorColor = Color.red;
            if (indicator && indicator.GetComponent<Renderer>().material.color != indicatorColor)
                indicator.GetComponent<Renderer>().material.color = indicatorColor;
        }
        else 
        {
            //Player Was Just lost
            playerDetected = false;
            indicatorColor = Color.yellow;

            if (indicator && indicator.GetComponent<Renderer>().material.color != indicatorColor)
                indicator.GetComponent<Renderer>().material.color = indicatorColor;
        }

        if (!playerDetected && enemyControl.remainingDistance < 1)
        {
            detectionTimer += Time.deltaTime;
            if(detectionTimer > detectionCooldown)
            {
                detectionTimer = 0;
                playerFound = false;
                indicator.SetActive(false);
                return;
            }
        }

    }
    void AllyDetectPlayer()
    {
        if (playerFound)
            return;

        for (int i = 0; i < allies.Count - 1; i++)
        {
            if (allies[i] == null)
                continue;

            RaycastHit hit;
            float distance = Vector3.Distance(transform.position, allies[i].transform.position);
            if(distance < detectionDistance && !Physics.Linecast(transform.position, allies[i].transform.position, out hit, mask))
            {
                playerFound = alliesNav[i].playerFound;
                lastKnownPos = alliesNav[i].lastKnownPos;
                if (playerFound)
                {
                    indicator.SetActive(true);
                    return;
                }
            }
        }

    }
}


           