using UnityEngine;
using System.Collections;

public class GinoAnimControl : MonoBehaviour
{

    public UnityEngine.Animator animator;
    GinoSoundControl sound;
    GameObject player;
    public Rigidbody playerRBody;
    public Transform cameraTransform;
    public CharacterControl ginoController;
    public bool chargeCheck;
    //COMBAT
    int attackCount = 0;
    float attackTimer = 0;
    bool startTimer;
    void Start()
    {
        sound = GetComponent<GinoSoundControl>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        ginoController = player.GetComponent<CharacterControl>();
        playerRBody = player.GetComponent<Rigidbody>();
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    
    void Update()
    {
        if (Time.timeScale == 0)
            return;

        checkCurrentAnim();
        bool takingDamage = CheckDamage();
        if (!takingDamage)
        {
            MovementAnimations();
            CombatAnimations();
        }
    }
    bool CheckDamage()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("LeftHit") || animator.GetCurrentAnimatorStateInfo(0).IsName("RightHit"))
            return true;

            return false;
    }

    void checkCurrentAnim()
    {
        if(ginoController.vitalsSettings.health <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            animator.Play("Dead");
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Charge"))
        {
            chargeCheck = true;
        }
        else
        {
            chargeCheck = false;
        }
    }
    void MovementAnimations()
    {


        var locVel = transform.InverseTransformDirection(playerRBody.velocity);
        bool ground = ginoController.moveSettings.isGrounded;
        bool combat = ginoController.combatSettings.inCombat;
        bool wallCollision = ginoController.moveSettings.wallCollision;
        bool climbingWall = ginoController.moveSettings.climbingWall;
        float upSpeed = playerRBody.velocity.y;
        float groundDist = ginoController.height;
        float turnSpeed = Input.GetAxis("Horizontal");
        float forwardSpeed;
        float climbUp = locVel.y;
        float climbRight = locVel.x;
        bool breakingWall = ginoController.breakingWall;
        bool block = ginoController.blockInput;
        bool charge = false;
        bool crouching = ginoController.moveSettings.isCrouching;
        if (ginoController.runInput > 0 && ginoController.attackInput)
            charge = true;
        if (ginoController.attackInput && ginoController.runInput <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Slap"))
            combat = true;


        if (!combat)
        {
            forwardSpeed = locVel.z;
        }
        else
        {
            forwardSpeed = Input.GetAxis("Vertical");
        }

        if (Input.GetAxis("Vertical") < -0.1f && !combat)
        {
            turnSpeed = 0;
        }
        //Normal Locomotion
        animator.SetFloat("MoveSpeed", forwardSpeed);
        animator.SetFloat("MoveDirection", turnSpeed);
        animator.SetFloat("groundDist", groundDist);
        animator.SetFloat("climbUp", climbUp);
        animator.SetFloat("climbRight", climbRight);
        animator.SetBool("isGrounded", ground);
        animator.SetBool("wallCollision", wallCollision);
        animator.SetBool("climbingWall", climbingWall);
        animator.SetBool("isCrouching", crouching);
        //Combat Locomotion
        animator.SetBool("inCombat", combat);
        animator.SetFloat("UpSpeed", upSpeed);
        animator.SetFloat("CombatForwardSpeed", forwardSpeed);
        animator.SetFloat("CombatSideSpeed", turnSpeed);
        animator.SetBool("block", block);
        animator.SetBool("charge", charge);
    }
    void CombatAnimations()
    {


        bool attack = ginoController.attackInput;
        bool heavyAttack = ginoController.heavyInput;
        bool heavyAttack2 = ginoController.heavy2Input;

        //BASIC COMBOS
        if (attack)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Uppercut"))
                attackCount = 1;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slap"))
                attackCount = 2;
        }
        if (heavyAttack)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slap") && attackCount != 2)
                attackCount = 3;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ComboEnd"))
        {
            attackCount = 0;
        }

        //HEAVY ATTACK
        if (heavyAttack)
        {
            startTimer = true;
        }
        if (startTimer && heavyAttack2)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > 0.5f)
            {
                animator.SetBool("heavyAttack", true);
                startTimer = false;
            }
        }
        else
        {
            attackTimer = 0;
            animator.SetBool("heavyAttack", false);
        }


        animator.SetInteger("attackCount", attackCount);
        animator.SetBool("attack", attack);
        animator.SetFloat("attackTimer", attackTimer);
    }

    void Combat(float dmg)
    {
        if (ginoController.combatSettings.currentTarget == null)
        {
            sound.Attack(0);
            return;
        }
        float targetDist = Vector3.Distance(transform.position, ginoController.combatSettings.currentTarget.transform.position);


        if (targetDist < 4.0f)
        {
            if (!ginoController.combatSettings.currentTarget.transform.root.GetChild(0).GetComponent<enemyAnimControl>().isBlocking || ginoController.superInput)
            {
                float superMult = 1.0f;
                if (ginoController.superInput)
                {
                    superMult = 2.5f;
                }
                sound.Attack((int)dmg);
                ginoController.combatSettings.currentTarget.transform.root.GetComponent<EnemyStatControl>().vitalsSettings.health -= dmg * superMult * ginoController.vitalsSettings.strength;
                ginoController.combatSettings.currentTarget.transform.root.GetChild(0).GetComponent<enemyAnimControl>().HitAnimations((int)dmg);
            }
            else
            {
                sound.Attack(0);
            }
        }
        else
        {
            sound.Attack(0);
        }

    }

    void RangedCombat(float dmg)
    {
        sound.Attack((int) dmg * 2);

        for (int i=0; i < ginoController.combatSettings.enemyTargets.Count; i++)
        {
            if (ginoController.combatSettings.enemyTargets[i] == null)
                continue;

            float targetDist = Vector3.Distance(transform.position, ginoController.combatSettings.enemyTargets[i].transform.position);
            if(targetDist < 10.0f)
            {
                if (targetDist < 4)
                    targetDist = 1;

                ginoController.combatSettings.enemyTargets[i].transform.root.GetComponent<EnemyStatControl>().vitalsSettings.health -= (dmg * ginoController.vitalsSettings.strength)/ targetDist;
         
                ginoController.combatSettings.enemyTargets[i].transform.root.GetChild(0).GetComponent<enemyAnimControl>().HitAnimations((int)dmg);
            }
        }
    }
}
