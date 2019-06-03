using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class CharacterControl : MonoBehaviour
{


    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.1f;
        public string FORWARD_AXIS = "Vertical";
        public string TURN_AXIS = "Horizontal";
        public string SPRINT_AXIS = "Sprint";
        public string DPADX_AXIS = "DpadX";
        public string JUMP_BUTTON = "Jump";
        public string GRAB_BUTTON = "Grab";
        public string HEAVY_BUTTON = "Heavy";
        public string DROP_BUTTON = "Drop";
        public string BLOCK_BUTTON = "Block";
        public string SELECT_BUTTON = "Select";
        public string ANALOGL_BUTTON = "AnalogL";
        public string ANALOGR_BUTTON = "AnalogR";
    }

    [System.Serializable]
    public class CombatSettings
    {
        public bool inCombat;
        public float strafeSpeed = 2f;
        public List<GameObject> enemyTargets;
        public GameObject currentTarget;
    }


    [System.Serializable]
    public class MoveSettings
    {
        [Header("Walk Settings")]
        public float walkSpeed = 2f;
        public float runSpeed = 6f;
        public float turnSpeed = 2f;
        public float turnrunSpeed = 4f;
        public float gravity = 40f;
        public bool isGrounded;
        public float walkAcceleration = 3f;

        [Header("Crouch Settings")]
        public float crouchSpeed = 1f;
        public bool canStand = true;
        public bool isCrouching = false;
        public bool isHidden = false;

        [Header("Jump Settings")]
        public float jumpHeight = 10f;
        public float airSpeed = 8f;
        public float airTurnSpeed = 8f;
        public bool jump;

        [Header("Climb Settings")]
        public bool wallCollision;
        public bool climbingWall;
        public float climbSpeed = 2f;

        [Header("Other Variables")]
        public Rigidbody rBody;
        public Transform CameraMain;
        public Animator anim;
        public SphereCollider sphere;
    }

    [System.Serializable]
    public class VitalsSettings
    {
        //Health and Energy Area
        public float healthLevel = 1;
        public float health = 150;
        public float maxHealth = 250;

        public float energyLevel = 1;
        public float energy = 25;
        public float maxEnergy = 100;

        public float strengthLevel = 1;
        public float strength = 1;

        public GameObject energyEffect;
        public GameObject headphones;
        public int coins = 0;
    }

    public InputSettings inputSettings = new InputSettings();
    public CombatSettings combatSettings = new CombatSettings();
    public MoveSettings moveSettings = new MoveSettings();
    public VitalsSettings vitalsSettings = new VitalsSettings();

    //Game Controller
    GameControl gameControl;
    
    //Controller INPUTS
    [HideInInspector]
    public float forwardInput, turnInput, runInput, dPadXInput;
    [HideInInspector]
    public bool superInput, heavyInput, heavy2Input, selectInput, grabInput, dropInput, jumpInput, jumpInitiation, blockInput, attackInput;
    
    
    //Jump Timers
    private float jumpTimer;
    private bool jumpTimerStart;
    private float wallJumpTimer;
    private Vector3 jumpRef;

    //Booleans to Stop Movement
    private bool freezeAirMovement;
    private bool freezeAirRotation;


    //Climbing Objects and Checks
    [HideInInspector]
    public bool breakingWall;
    [HideInInspector]
    public GameObject climbWall, climbLedge;
    [HideInInspector]
    public bool climbLeft, climbRight, climbTop, climbDown;

    
    
    //Combat Variables and Checks
    private bool check;
    private int enemyTarget = 0;
    [HideInInspector]
    public List<EnemyDetect> enemyDetectScripts;
    [HideInInspector]
    public int attackCount;
    private float attackTimer;
    private bool attackCooldown;
    private bool spotted = false;

    //Variables for animation
    [HideInInspector]
    public float height;
    [HideInInspector]
    public GameObject characterModel;
    [HideInInspector]
    public Vector3 characterDirection;
    private Vector3 direction;
    private CapsuleCollider capsule;
    private LayerMask mask = 1 << 8;


    //Hint Variables
    private GameObject iconObject;
    private string iconTag;
    public Sprite[] icons;

    
    void Start()
    {

        GetEnemies();
        if (GetComponent<Rigidbody>())
        {
            moveSettings.rBody = GetComponent<Rigidbody>();
            moveSettings.CameraMain = Camera.main.transform;
            capsule = GetComponent<CapsuleCollider>();
            moveSettings.sphere = GetComponent<SphereCollider>();
            gameControl = GameObject.FindObjectOfType<GameControl>();
            mask = ~mask;
        }
        else
        {
            Debug.LogError("Rigidbody or Camera Missing");
        }
        forwardInput = turnInput = runInput = 0;
    }

    void GetEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemyDetector");
        
        for (int i = 0; i < enemies.Length; i++)
        {
            if(!enemyDetectScripts.Contains(enemies[i].GetComponent<EnemyDetect>()) && enemies[i].GetComponent<Rigidbody>())
                  enemyDetectScripts.Add(enemies[i].GetComponent<EnemyDetect>());
        }
    }   
    void Update()
    {
        if (gameControl.levelCompleted)
            return;

       GetInput();
       TrackCombat();
       CheckHealth();
       SizeSphereCapsule();
       climbCheck();
       DrawHints();
    }
    void FixedUpdate()
    {
        if (Time.timeScale == 0 || gameControl.levelCompleted)
            return;

        isGrounded();
        CheckCrouch();
        if (moveSettings.climbingWall)
        {
            Climb();
            ClimbJump();
        }
        else if (!combatSettings.inCombat && !moveSettings.wallCollision)
        {
            Walk();
        }
        else if(combatSettings.inCombat)
        {
            moveSettings.isCrouching = false;
            SwitchTarget();
            StrafeWalk();
        }
        Jump();
    }
    
    void GetInput()
    {
        forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS);
        turnInput = Input.GetAxis(inputSettings.TURN_AXIS);
        runInput = Input.GetAxis(inputSettings.SPRINT_AXIS);
        grabInput = Input.GetButtonDown(inputSettings.GRAB_BUTTON);
        attackInput = Input.GetButtonDown(inputSettings.GRAB_BUTTON);
        dropInput = Input.GetButton(inputSettings.DROP_BUTTON);
        jumpInput = Input.GetButton(inputSettings.JUMP_BUTTON);
        jumpInitiation = Input.GetButtonDown(inputSettings.JUMP_BUTTON);
        blockInput = Input.GetButton(inputSettings.BLOCK_BUTTON);
        dPadXInput = Input.GetAxisRaw(inputSettings.DPADX_AXIS);
        selectInput = Input.GetButtonDown(inputSettings.SELECT_BUTTON);
        heavyInput = Input.GetButtonDown(inputSettings.HEAVY_BUTTON);
        heavy2Input = Input.GetButton(inputSettings.HEAVY_BUTTON);

        if (Input.GetButtonDown(inputSettings.ANALOGL_BUTTON))
        {
            if (vitalsSettings.energy > 0)
            {
                superInput = !superInput;
            }
            else
            {
                superInput = false;
            }
        }
        if (Input.GetButtonDown(inputSettings.JUMP_BUTTON) && moveSettings.isGrounded)
        {
            jumpTimerStart = true;
        }
        if (Input.GetButton(inputSettings.JUMP_BUTTON) && jumpTimerStart)
        {
            jumpTimer += Time.deltaTime;
        }
        if(Input.GetButtonUp(inputSettings.JUMP_BUTTON) && moveSettings.isGrounded)
        {
            moveSettings.jump = true;
        }

    }
    void DrawHints()
    {
        Vector3 origin = transform.position + Vector3.up + transform.forward;
        RaycastHit hit;
        Debug.DrawRay(origin, transform.forward * 10, Color.red);
        if (Physics.Raycast(origin, transform.forward, out hit))
        {
            if (hit.collider.tag != iconTag)
            {
                Destroy(iconObject);
                iconObject = null;
            }

            if (hit.collider.tag == "Wall" && iconObject == null)
            {
                iconObject = Instantiate(Resources.Load("ButtonIcons/ButtonHint")) as GameObject;
                iconTag = "Wall";
                iconObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = icons[0];
                iconObject.transform.GetChild(1).transform.gameObject.SetActive(false);
                iconObject.transform.GetChild(2).transform.gameObject.SetActive(false);
            }

            if (hit.collider.tag == "breakWall" && iconObject == null)
            {
                iconObject = Instantiate(Resources.Load("ButtonIcons/ButtonHint")) as GameObject;
                iconTag = "breakWall";
                iconObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = icons[7];
                iconObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = icons[2];
            }

            if (hit.collider.tag == "ClimbWall" && iconObject == null && !moveSettings.climbingWall)
            {
                iconObject = Instantiate(Resources.Load("ButtonIcons/ButtonHint")) as GameObject;
                iconTag = "ClimbWall";
                iconObject.transform.GetChild(1).transform.gameObject.SetActive(false);
                iconObject.transform.GetChild(2).transform.gameObject.SetActive(false);
                iconObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = icons[2];
            }

            if (hit.collider.tag == "hidingSpot" && iconObject == null && !moveSettings.climbingWall)
            {
                iconObject = Instantiate(Resources.Load("ButtonIcons/ButtonHint")) as GameObject;
                iconTag = "hidingSpot";
                iconObject.transform.GetChild(1).transform.gameObject.SetActive(false);
                iconObject.transform.GetChild(2).transform.gameObject.SetActive(false);
                iconObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = icons[6];
            }
            if (iconObject != null)
            {
                if (hit.distance > 4)
                {
                    Destroy(iconObject);
                    iconObject = null;
                }
                else
                {
                    iconObject.transform.position = hit.point - (moveSettings.CameraMain.forward * 1.0f) + Vector3.up;
                }
            }
        }
        if (moveSettings.climbingWall)
        {
            Destroy(iconObject);
            iconObject = null;
        }
    }
    void climbCheck()
    {
        if (climbWall == null)
        {
            moveSettings.climbingWall = false;
            transform.parent = null;
            return;
        }
        transform.parent = climbWall.transform;

        Transform marker = climbWall.transform.Find("Marker").transform;
        float direction = Vector3.Dot(transform.forward, marker.forward);
        float distance = transform.localPosition.z;
        bool dir = false;

        if ((direction < -0.74f && moveSettings.isGrounded) || (direction <= 0 && !moveSettings.isGrounded))
            dir = true;

        if (dir && distance < 1.8f && grabInput && !moveSettings.climbingWall)
        {
            
            transform.rotation = Quaternion.LookRotation(-climbWall.transform.forward);
            Vector3 wallPos = climbWall.transform.position;
          
            Vector3 localPos = transform.localPosition;
            localPos.z = 0.2f;
            transform.localPosition = localPos;

            moveSettings.climbingWall = true;
            moveSettings.rBody.velocity = Vector3.zero;
        }
    }
    void Climb()
    {
        if (climbWall == null)
            return;

        bool Move = true;
        Vector2 inputDirection = new Vector2(0, 0);
        inputDirection.y = forwardInput;
        inputDirection.x = turnInput;
        inputDirection.Normalize();

        float xPos = transform.localPosition.x;
        float yPos = transform.localPosition.y + 1;
        float xScale = (climbWall.transform.Find("ClimbingWall").transform.localScale.x - 1.5f) / 2;
        float yScale = (climbWall.transform.Find("ClimbingWall").transform.localScale.y - 2.5f) / 2;
        
        if ((turnInput < 0 && xPos > xScale) || (turnInput > 0 && xPos < -xScale))
             Move = false;

       // if ((forwardInput > 0 && yPos > yScale) || (forwardInput < 0 && yPos < -yScale))
         //   Move = false;

        if (inputDirection != Vector2.zero && Move)
        {
                direction = transform.up * inputDirection.y * moveSettings.climbSpeed;
                direction += transform.right * inputDirection.x * moveSettings.climbSpeed;
                moveSettings.rBody.velocity = Vector3.Lerp(moveSettings.rBody.velocity, direction, Time.deltaTime * moveSettings.walkAcceleration);
        }
        else if (forwardInput == 0 && turnInput == 0 || !Move)
        {
            moveSettings.rBody.velocity = Vector3.zero;
        }
        
        characterDirection.y = 0;
    }

    void ClimbJump()
    {
        if (dropInput)
        {
            moveSettings.climbingWall = false;
        }
        if(forwardInput == 1 && climbTop)
        {
            climbTop = false;
            StartCoroutine("ClimbLedge", climbLedge.transform);
        }
        Vector3 jumpDir;
        if (jumpInitiation)
        {
            moveSettings.climbingWall = false;
            transform.parent = null;
           
            if (turnInput < -0.5f && climbLeft)
            {
                jumpDir = climbWall.transform.right * 5.0f;
                moveSettings.rBody.AddForce(jumpDir, ForceMode.Impulse);
                moveSettings.rBody.AddForce(Vector3.up * 15.0f, ForceMode.Impulse);
            }
            else if (turnInput > 0.5f && climbRight)
            {
                jumpDir = -climbWall.transform.right * 5.0f;
                moveSettings.rBody.AddForce(jumpDir, ForceMode.Impulse);
                moveSettings.rBody.AddForce(Vector3.up * 15.0f, ForceMode.Impulse);
            }
            else
            {
                moveSettings.rBody.AddForce(-transform.forward * 6f, ForceMode.Impulse);
            }
            freezeAirMovement = true;
            freezeAirRotation = true;
            StartCoroutine("FreezeAirMovement");
            StartCoroutine("FreezeAirRotation");
        }
    }

    void CheckCrouch()
    {
        if (moveSettings.isCrouching)
        {
            capsule.height = 1.5f;
            capsule.center = new Vector3(0, 0.75f, 0);

            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.up, out hit, mask))
            {
                if(hit.distance <= 4)
                {
                    moveSettings.canStand = false;
                }
            }
            else
            {
                moveSettings.canStand = true;
            }
        }
        else
        {
            moveSettings.canStand = true;
            capsule.height = 2;
            capsule.center = new Vector3(0,1,0);
        }

    }
    void Walk()
    {
        Vector2 inputDirection = new Vector2(0, 0);
        inputDirection.y = forwardInput;
        inputDirection.x = turnInput;
        inputDirection.Normalize();

        if (superInput)
        {
            moveSettings.runSpeed = 15f;
        }
        else
        {
            moveSettings.runSpeed = 7.0f;
        }
        if (moveSettings.isGrounded && !moveSettings.anim.GetCurrentAnimatorStateInfo(0).IsName("Land"))
        {
            if (moveSettings.canStand)
            {
                if (runInput > 0.2f)
                {
                    moveSettings.isCrouching = false;
                    direction = moveSettings.CameraMain.transform.forward * inputDirection.y * moveSettings.runSpeed * runInput;
                    direction += moveSettings.CameraMain.transform.right * inputDirection.x * moveSettings.turnrunSpeed * runInput;
                }
                else if (Mathf.Abs(runInput) < 0.2f)
                {
                    moveSettings.isCrouching = false;
                    direction = moveSettings.CameraMain.transform.forward * inputDirection.y * moveSettings.walkSpeed;
                    direction += moveSettings.CameraMain.transform.right * inputDirection.x * moveSettings.turnSpeed;
                }
                else if (runInput < -0.2f)
                {
                    moveSettings.isCrouching = true;
                    direction = moveSettings.CameraMain.transform.forward * inputDirection.y * moveSettings.crouchSpeed;
                    direction += moveSettings.CameraMain.transform.right * inputDirection.x * moveSettings.crouchSpeed;
                }
            }
            else
            {
                moveSettings.isCrouching = true;
                direction = moveSettings.CameraMain.transform.forward * inputDirection.y * moveSettings.crouchSpeed;
                direction += moveSettings.CameraMain.transform.right * inputDirection.x * moveSettings.crouchSpeed;
            }

            direction.y = moveSettings.rBody.velocity.y;
            moveSettings.rBody.velocity = Vector3.Lerp(moveSettings.rBody.velocity, direction, Time.deltaTime * moveSettings.walkAcceleration);
        }
        else if (!moveSettings.isGrounded && !freezeAirMovement)
        {
            direction = moveSettings.CameraMain.transform.forward * inputDirection.y * moveSettings.airSpeed;
            direction += moveSettings.CameraMain.transform.right * inputDirection.x * moveSettings.airTurnSpeed;

            direction.y = moveSettings.rBody.velocity.y;
            moveSettings.rBody.velocity = Vector3.Lerp(moveSettings.rBody.velocity, direction, Time.deltaTime * moveSettings.walkAcceleration);
        }

        if ((forwardInput == 0 && turnInput == 0 && moveSettings.isGrounded) || moveSettings.anim.GetCurrentAnimatorStateInfo(0).IsName("Land"))
        {
            Vector3 idle = new Vector3(0, moveSettings.rBody.velocity.y, 0);
            moveSettings.rBody.velocity = Vector3.Lerp(moveSettings.rBody.velocity, idle, Time.deltaTime * moveSettings.walkAcceleration * 2);
        }

        if (!freezeAirRotation)
        {
            Vector3 characterDirection = moveSettings.rBody.velocity;
            characterDirection.y = 0;
            transform.LookAt(transform.position + characterDirection);
        }
    }
    void StrafeWalk()
    {
        if (combatSettings.currentTarget == null)
            return;

        Vector2 inputDirection = new Vector2(0, 0);
        inputDirection.y = forwardInput;
        inputDirection.x = turnInput;
        inputDirection.Normalize();

        if (moveSettings.isGrounded && moveSettings.anim.GetCurrentAnimatorStateInfo(0).IsName("CombatWalk"))
        {
            direction = moveSettings.CameraMain.transform.forward * inputDirection.y * combatSettings.strafeSpeed;
            direction += moveSettings.CameraMain.transform.right * inputDirection.x * combatSettings.strafeSpeed;

            direction.y = moveSettings.rBody.velocity.y;
            moveSettings.rBody.velocity = Vector3.Lerp(moveSettings.rBody.velocity, direction, Time.deltaTime * moveSettings.walkAcceleration);
        }
        else if (!moveSettings.isGrounded && jumpTimerStart)

        {
            direction = moveSettings.CameraMain.transform.forward * inputDirection.y * moveSettings.airSpeed;
            direction += moveSettings.CameraMain.transform.right * inputDirection.x * moveSettings.airTurnSpeed;

            direction.y = moveSettings.rBody.velocity.y;
            moveSettings.rBody.velocity = Vector3.Lerp(moveSettings.rBody.velocity, direction, Time.deltaTime * moveSettings.walkAcceleration);
        }
        if (forwardInput == 0 && turnInput == 0 && moveSettings.isGrounded)
        {
            Vector3 idle = new Vector3(0, moveSettings.rBody.velocity.y, 0);
            moveSettings.rBody.velocity = Vector3.Lerp(moveSettings.rBody.velocity, idle, Time.deltaTime * moveSettings.walkAcceleration * 2);
        }

        Vector3 enemyDirection = combatSettings.currentTarget.transform.position;

        Vector3 desiredDirection = enemyDirection - transform.position;
        desiredDirection.y = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(desiredDirection), Time.deltaTime * 2.0f);
    }
    void SwitchTarget()
    {
        if (!combatSettings.inCombat )
                 return;
        

        if (combatSettings.currentTarget == null || combatSettings.currentTarget.transform.root.tag == "deadEnemy")
        {
            for (int i = 0; i < combatSettings.enemyTargets.Count; i++)
            {
                if (combatSettings.enemyTargets[i] != null && combatSettings.enemyTargets[i].transform.root.tag != "deadEnemy")
                {
                    combatSettings.currentTarget = combatSettings.enemyTargets[i];
                    break;
                }
            }
            if (combatSettings.currentTarget == null)
            {
                combatSettings.inCombat = false;
                return;
            }

        }

        if (dPadXInput == 0)
        {
            check = true;
        }
        if (dPadXInput < 0 && check)
        {
            GameObject nextTarget = combatSettings.currentTarget;
            float angle = 0;
            for (int i = 0; i < combatSettings.enemyTargets.Count; i++)
            {
                float dir = Vector3.Dot(transform.right, combatSettings.enemyTargets[i].transform.root.position - transform.position);
                if (combatSettings.enemyTargets[i] == combatSettings.currentTarget || dir > 0)
                    continue;

                Vector3 directionVect = combatSettings.enemyTargets[i].transform.root.position - transform.position;
                float angleToTarget = Vector3.Angle(directionVect, transform.forward);

                if (angle == 0)
                    angle = angleToTarget;

                if (angleToTarget <= angle)
                {
                    nextTarget = combatSettings.enemyTargets[i];
                }
            }
            combatSettings.currentTarget = nextTarget;
            check = false;
        }
        if (dPadXInput > 0 && check)
        {
            GameObject nextTarget = combatSettings.currentTarget; 
            float angle = 0;
            for (int i = 0; i < combatSettings.enemyTargets.Count; i++)
            {
                float dir = Vector3.Dot(transform.right, combatSettings.enemyTargets[i].transform.root.position - transform.position);
                if (combatSettings.enemyTargets[i] == combatSettings.currentTarget || dir < 0)
                    continue;

                Vector3 directionVect = combatSettings.enemyTargets[i].transform.root.position - transform.position;
                float angleToTarget = Vector3.Angle(directionVect, transform.forward);
               
                if (angle == 0)
                    angle = angleToTarget;

                if (angleToTarget <= angle)
                {
                    nextTarget = combatSettings.enemyTargets[i];
                }
            }
            combatSettings.currentTarget = nextTarget;
            check = false;
        }
    }
    void Jump()
    {
        if (jumpTimerStart)
        {
            jumpTimer += Time.deltaTime;
        }
        else
        {
            jumpTimer = 0;
        }

        if (jumpInitiation && moveSettings.isGrounded)
        {
            jumpTimerStart = true;
            moveSettings.rBody.AddForce(Vector3.up * 7f, ForceMode.Impulse);
        }
        if (jumpInput && jumpTimer < 1.2f && jumpTimerStart)
        {
           moveSettings.rBody.AddForce(Vector3.up * moveSettings.jumpHeight, ForceMode.Force);
        }
        else
        {
            jumpTimerStart = false;
        }


        if (!moveSettings.isGrounded && moveSettings.wallCollision)
        {
            moveSettings.rBody.velocity = Vector3.zero;
            wallJumpTimer += +Time.deltaTime;
            if (wallJumpTimer > 1f)
            {
                moveSettings.wallCollision = false;
                wallJumpTimer = 0;
            }

            if (jumpInitiation)
            {
                moveSettings.wallCollision = false;
                jumpRef.y = 2f;
                moveSettings.rBody.velocity = jumpRef * 8f;
                freezeAirMovement = true;
                StartCoroutine("FreezeAirMovement");
            }

        }
    }
    void isGrounded()
    {
        RaycastHit hit;

        Vector3 down = transform.TransformDirection(Vector3.down) * 10;
        Vector3 pos = characterModel.transform.position;
        pos += Vector3.up;
        Debug.DrawRay(pos, down, Color.red);

        if (Physics.Raycast(pos, (down), out hit))
        {
            height = hit.distance;

            if (hit.distance < 1.1f)
            {
                moveSettings.isGrounded = true;
            }
            else
            {
                moveSettings.isGrounded = false;

            }
        }
        else
        {
            moveSettings.isGrounded = false;


        }

        if (!moveSettings.isGrounded && !moveSettings.wallCollision && !moveSettings.climbingWall && !jumpTimerStart)
        {
            moveSettings.rBody.AddForce(-Vector3.up * moveSettings.gravity);
        }
    }
    void TrackCombat()
    {
        bool playerDetected = false;


        

        for (int i = 0; i < enemyDetectScripts.Count; i++)
        {
            if (enemyDetectScripts[i] == null)
                continue;

            float distToEnemy = 0;
            distToEnemy = Vector3.Distance(transform.position, enemyDetectScripts[i].transform.position);


            if ((enemyDetectScripts[i].transform.root.gameObject.tag == "deadEnemy" && combatSettings.enemyTargets.Contains(enemyDetectScripts[i].gameObject)) || (combatSettings.enemyTargets.Contains(enemyDetectScripts[i].gameObject) && distToEnemy >= 10))
            { 
                combatSettings.enemyTargets.Remove(enemyDetectScripts[i].gameObject);
            }
            else if (distToEnemy < 10 && enemyDetectScripts[i].playerDetected)
            {
                if (!combatSettings.enemyTargets.Contains(enemyDetectScripts[i].gameObject))
                    combatSettings.enemyTargets.Add(enemyDetectScripts[i].gameObject);

                if(enemyDetectScripts[i].transform.root.tag != "deadEnemy")
                        playerDetected = true;

                combatSettings.inCombat = playerDetected;
               
            }
                
        }

        
        if (runInput > 0 || !moveSettings.isGrounded || moveSettings.climbingWall || combatSettings.enemyTargets.Count == 0)
        {
            combatSettings.inCombat = false;
        }
        else 
        {
            combatSettings.inCombat = playerDetected;
        }
        

    }
    void CheckHealth()
    {
        if (vitalsSettings.health > vitalsSettings.maxHealth)
            vitalsSettings.health = vitalsSettings.maxHealth;

        if (superInput)
        {
            if (!vitalsSettings.energyEffect.activeSelf)
                vitalsSettings.energyEffect.SetActive(true);
            

            if(!vitalsSettings.headphones.activeSelf)
                vitalsSettings.headphones.SetActive(true);


           vitalsSettings.energy -= Time.deltaTime * 5.0f;

            if (vitalsSettings.energy <= 0)
            {
                vitalsSettings.energy = 0;
                superInput = false;
                vitalsSettings.energyEffect.SetActive(false);
                vitalsSettings.headphones.SetActive(false);
            }
        }
        else
        {
            vitalsSettings.energyEffect.SetActive(false);
            vitalsSettings.headphones.SetActive(false);
        }

        if (vitalsSettings.health <= 0.0f)
            StartCoroutine("RespawnPlayer");

    }
    void SizeSphereCapsule()
    {
        if(moveSettings.isCrouching || moveSettings.isHidden)
        {
            moveSettings.sphere.radius = 0.5f;
        }
        else
        {
            moveSettings.sphere.radius = moveSettings.rBody.velocity.z * 2.0f;
        }

        if (moveSettings.sphere.radius > 8.0f)
            moveSettings.sphere.radius = 8.0f;
    }

    IEnumerator FreezeAirMovement()
    {
        while (!moveSettings.isGrounded)
        {
            yield return null;
        }
        freezeAirMovement = false;
    }

    IEnumerator FreezeAirRotation()
    {
        while (!moveSettings.isGrounded)
        {
            yield return null;
        }
        freezeAirRotation = false;
    }

    IEnumerator ClimbLedge(Transform marker)
    {
        moveSettings.climbingWall = false;
        moveSettings.rBody.isKinematic = true;
        capsule.isTrigger = true;
        moveSettings.anim.Play("ClimbLedge");
        Vector3 destination = transform.InverseTransformPoint(marker.position);
        destination.x = 0;
        destination = transform.TransformPoint(destination);
        float distance = Vector3.Distance(transform.position, destination);
        while (distance > 0.5f)
        {
            distance = Vector3.Distance(transform.position, destination);
            transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * 2.0f);
            yield return null;
        }

        marker = marker.GetChild(0).transform;

        destination = transform.InverseTransformPoint(marker.position);
        destination.x = 0;
        destination = transform.TransformPoint(destination);

        distance = Vector3.Distance(transform.position, marker.position);
        while (distance > 0.5f)
        {
            distance = Vector3.Distance(transform.position, destination);
            transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * 2.0f);
            yield return null;
        }

        capsule.isTrigger = false;
       moveSettings.rBody.isKinematic = false;
    }

    IEnumerator RespawnPlayer()
    {

        yield return new WaitForSeconds(3.0f);

        Initiate.Fade("Loading Scene", "Level1", Color.black, 0.5f);
    }
    public void UpdateAttributes()
    {
        if(vitalsSettings.strengthLevel > 1)
            vitalsSettings.strength = 1.0f + (vitalsSettings.strengthLevel * 0.2f);

        if (vitalsSettings.energyLevel > 1)
            vitalsSettings.maxEnergy = 100.0f + (vitalsSettings.energyLevel * 25.0f);

        if (vitalsSettings.healthLevel > 1)
            vitalsSettings.maxHealth = 250.0f + (vitalsSettings.healthLevel * 50.0f);
    }
   
    void OnCollisionEnter(Collision col)
    {
        RaycastHit hit;
        if(col.gameObject.tag == "Wall" && !moveSettings.isGrounded)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.collider.tag == "Wall")
                {
                    moveSettings.wallCollision = true;
                    
                    jumpRef = Vector3.Reflect(transform.forward, hit.normal);
                    wallJumpTimer = 0;
                    float normal = Vector3.Angle(jumpRef, hit.normal);
                    if (normal < 20f)
                    {
                        jumpRef = hit.normal;
                    }
                }
            }
       }
   }
    void OnCollisionExit(Collision col)
    {
        if(col.gameObject.tag == "Wall")
        {
            moveSettings.wallCollision = false;
            jumpRef = Vector3.zero;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "ClimbWallTrigger")
            climbWall = col.gameObject.transform.parent.transform.parent.gameObject;

        if (col.tag == "Climbtop")
        {
            climbTop = true;
            climbLedge = col.transform.GetChild(0).gameObject;
        }
    }
    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Climbright")
        {
            climbRight = true;
        }
        if (col.tag == "Climbleft")
        {
            climbLeft = true;
        }
        if (col.tag == "hidingSpot" && !combatSettings.inCombat)
        {
            if (moveSettings.isCrouching)
            {
                moveSettings.isHidden = true;
            }
            else
            {
                moveSettings.isHidden = false;
            }

        }
        else
        {
            moveSettings.isHidden = false;
        }
        

    }
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Climbright")
        {
            climbRight = false;
        }
        if (col.tag == "Climbtop")
        {
            climbTop = false;
        }
        if (col.tag == "Climbleft")
        {
            climbLeft = false;
        }
        if (col.tag == "ClimbWallTrigger")
        {
          //  moveSettings.climbingWall = false;
            climbWall = null;
        }
        if (col.tag == "hidingSpot")
        {
            moveSettings.isHidden = false;
        }
    }
}
   

