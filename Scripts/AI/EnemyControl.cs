using UnityEngine;
using System.Collections;

public class EnemyControl : MonoBehaviour
{
    public Transform patrolRoute;

    public EnemyDetect trackingInfo;
    UnityEngine.AI.NavMeshAgent navAgent;
    Transform player;
    enemyAnimControl animControl;

    public int i = 0;
    float idleTime = 3.0f;
    float Timer;
    bool playerHeard;
    public Transform[] patrolPoints;
    public float remainingDistance;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        trackingInfo = transform.Find("CyberSoldier/Armature/Hips/Spine/Chest/Neck/Head/detectionObject").gameObject.GetComponent<EnemyDetect>();
        animControl = transform.GetChild(0).GetComponent<enemyAnimControl>();
        navAgent = transform.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (patrolRoute != null)
            GetPatrolPoints();
    }
    void GetPatrolPoints()
    {
        patrolPoints = new Transform[patrolRoute.childCount];
        for (int i = 0; i < patrolRoute.childCount; i++)
        {
            patrolPoints[i] = patrolRoute.transform.GetChild(i).GetComponent<Transform>();
        }
        navAgent.SetDestination(patrolPoints[0].position);
    }
    void Update()
    {
        Patrol();
        Combat();
        SetNavVariables();
    }

    void Patrol()
    {
        if (playerHeard && !trackingInfo.playerDetected)
        {
            navAgent.SetDestination(player.transform.position);
            return;
        }
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (trackingInfo.playerFound)
        {
            Timer = 0;
            return;
        }


        if (remainingDistance < 1f)
        {
            Timer += Time.deltaTime;
            if (Timer > idleTime)
            {
                Timer = 0;
                i++;
                if (i > patrolPoints.Length - 1)
                    i = 0;
            }
        }
        navAgent.SetDestination(patrolPoints[i].position);
    }
    void Combat()
    {
        if (!trackingInfo.playerFound)
            return;

        navAgent.SetDestination(trackingInfo.lastKnownPos);
        
    }
    void SetNavVariables()
    {
        remainingDistance = navAgent.remainingDistance;


        if (trackingInfo.playerDetected)
        {

            if (remainingDistance > 2)
            {
                navAgent.Resume();
                navAgent.speed = 4.5f;
            }
            else
            {
                navAgent.Stop();
                navAgent.updateRotation = true;
                transform.LookAt(player);
                navAgent.speed = 2.0f;
            }
        }
        else
        {
            navAgent.stoppingDistance = 0;
        }
    }

    IEnumerator SpottedTimer()
    {
        yield return new WaitForSeconds(5.0f);

        playerHeard = false;
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player")
            return;
        playerHeard = true;
        StartCoroutine("SpottedTimer");
    }
}
