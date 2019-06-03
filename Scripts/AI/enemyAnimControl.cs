using UnityEngine;
using System.Collections;

public class enemyAnimControl : MonoBehaviour {

    [Header("Combat Audio Clips")]
    public AudioClip[] woosh;
    public AudioClip[] attacks;

    GinoAnimControl player;
    CharacterControl playerVitals;
    AudioSource audioSource;
    Animator anim;
    GameObject detectObject;
    GameObject head;
    EnemyDetect detectScript;
    EnemyStatControl controlScript;

    public UnityEngine.AI.NavMeshAgent navAgent;
    bool dead;
    bool attacking;
    bool takingDamage;
    public bool isBlocking;
    float attackTimer;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("playerModel").GetComponent<GinoAnimControl>();
        playerVitals = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControl>();
        audioSource = transform.GetComponent<AudioSource>();
        navAgent = transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        detectObject = transform.Find("Armature/Hips/Spine/Chest/Neck/Head/detectionObject").gameObject;
        head = transform.Find("Armature/Hips/Spine/Chest/Neck/Head").gameObject;
        detectScript = detectObject.GetComponent<EnemyDetect>();
        controlScript = transform.parent.GetComponent<EnemyStatControl>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(!dead)
        UpdateAnimation();
        CheckAttack();
        if(!player.animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        Combat();
    }

    void UpdateAnimation()
    {
        bool inCombat = detectScript.playerDetected;
        bool isSearching = detectScript.playerSpotted;
        bool isDead = controlScript.vitalsSettings.dead;

        if (isDead)
            anim.Play("Dead");
        
        
        anim.SetBool("inCombat", inCombat);
        anim.SetBool("isSearching", isSearching);

        Vector3 localVel = transform.InverseTransformDirection(navAgent.velocity);

        anim.SetFloat("rightSpeed", localVel.x);
        anim.SetFloat("forwardSpeed", localVel.z);



    }

    public void HitAnimations(int dmg)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
            return;

        switch (dmg)
        {
            case 15: case 50: case 28: anim.Play("LeftHit");
                                       break;

            case 30: case 35: case 10:
                                       anim.Play("RightHit");
                                       break;

            case 60: default:          anim.Play("RightHeavyHit");  
                                       break;

        }
        attackTimer = 0;
    }

    void CheckAttack()
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Cross") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Jab"))
        {
            attacking = false;
        }
        else
        {
            attacking = true;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LeftHit") || anim.GetCurrentAnimatorStateInfo(0).IsName("RightHit") || anim.GetCurrentAnimatorStateInfo(0).IsName("RightHeavyHit"))
        {
            takingDamage = true;
        }
        else
        {
            takingDamage = false;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Block"))
        {
            isBlocking = true;
        }
        else
        {
            isBlocking = false;
        }

    }

    public void PunchAnimations(int punch)
    {
        if(punch == 0)
        {
            anim.Play("Jab");
        }
        else
        {
            anim.Play("Cross");
        }
    }

    void AttackDamage(float dmg)
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if(distance > 2f || player.animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
        {
            int rand = Random.Range(0, 2);
            audioSource.PlayOneShot(woosh[rand]);
        }
        else
        {
            switch((int)dmg)
            {
                case 20: audioSource.PlayOneShot(attacks[0]);
                         player.animator.Play("LeftHit");
                         break;
                case 30: audioSource.PlayOneShot(attacks[1]);
                         player.animator.Play("RightHit");
                         break;
            }

            playerVitals.vitalsSettings.health -= dmg;
        }



    }
    void Combat()
    {

        if (detectScript.playerDetected && !takingDamage)
        {
            attackTimer += Time.deltaTime;
            if (navAgent.remainingDistance < 2.0f && attackTimer > 2.0f)
            {
                bool allyAttacking = false;

                for(int i=0; i < detectScript.alliesNav.Length; i++)
                {
                    if (detectScript.alliesNav[i] == null)
                        continue;

                    allyAttacking = detectScript.alliesNav[i].transform.root.GetChild(0).GetComponent<enemyAnimControl>().attacking;

                    if (allyAttacking)
                        return;
                }
                

                attackTimer = 0;
                bool attack = (Random.value > 0.25f);

                if (attack)
                {
                    float i = Random.value;

                    if (i >= 0.5)
                    {
                        PunchAnimations(1);
                    }
                    else
                    {
                        PunchAnimations(0);
                    }

                }
            }

            if (!attacking && attackTimer > 5.0f)
            {
                bool block = (Random.value > 0.9f);

                if (block && !isBlocking)
                {
                    anim.Play("Block");

                    attackTimer = 0;
                }
            }

        }
        else if (navAgent.remainingDistance > 1)
        {
            //GOING TO LAST POSITION
        }
        else if (navAgent.remainingDistance < 1)
        {
            //LOOKING IN SEARCH ZONE
        }



    }
}
