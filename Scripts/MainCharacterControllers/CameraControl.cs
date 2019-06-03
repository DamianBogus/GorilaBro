using UnityEngine;

public class CameraControl : MonoBehaviour
{


    [Header("Input Settings")]
    public float inputDelay = 0.1f;
    public string verticalAxis = "RVertical";
    public string horizontalAxis = "RHorizontal";
    public string rightstickButton = "RStickButton";
    public string horizontalMove = "Horizontal";
    public string verticalMove = "Vertical";
    public float ySensitivity = 5f;
    public float xSensitivity = 5f;
    [Header("Camera Control")]
    
    [SerializeField]
    private Transform target;

    [SerializeField]
    private GameObject player;


    [SerializeField]
    private float height = 2f;

    [SerializeField]
    private float distance = 2f;

    [SerializeField]
    private float smoothTime = 1f;

    private float maxDistance;
    private float occludedDistance = 5f;

    [SerializeField]
    private float rotationDamping = 2.0F;


    private Vector3 pos;

  //Camera Orbit Variables
    float xAxisOrbit;
    float yAxisOrbit;

    float currentRotationAngle;
    float currentHeight;

    float deadzone = 0.2f;
    

    void Start()
    {
        if (!target)
        {
            Debug.LogError("Camera is missing Target");
        }
        else
        {
            maxDistance = distance;
            xAxisOrbit = target.eulerAngles.y;
            yAxisOrbit = target.eulerAngles.x + 20;
            currentRotationAngle = transform.eulerAngles.y;
            currentHeight = transform.position.y;
        }
    }

    void LateUpdate()
    {
        if (Time.timeScale == 0)
            return;
        collisionCheck();
        cameraControl();
        transform.LookAt(target);
    }




    void cameraControl()
    {
        xAxisOrbit = currentRotationAngle = transform.eulerAngles.y;
        yAxisOrbit = currentHeight = transform.eulerAngles.x;

        if ((Mathf.Abs(Input.GetAxis(horizontalAxis)) > deadzone || Mathf.Abs(Input.GetAxis(verticalAxis)) > deadzone))
        {
           

            xAxisOrbit += xSensitivity * Input.GetAxis(horizontalAxis);
            yAxisOrbit -= ySensitivity * Input.GetAxis(verticalAxis);

            if (yAxisOrbit < 320 && yAxisOrbit > 100)
            {
                yAxisOrbit = 320;
            }
            else if (yAxisOrbit > 75 && yAxisOrbit < 300)
            {
                yAxisOrbit = 75;
            }

            currentRotationAngle = xAxisOrbit;
            currentHeight = yAxisOrbit;
        }
        else
        {
            if ((Mathf.Abs(Input.GetAxis(horizontalMove)) > 0 && Input.GetAxis(verticalMove) >= -0.2f) || Input.GetAxis(verticalAxis) > 0)
            {
                xAxisOrbit = target.eulerAngles.y;
                yAxisOrbit = target.eulerAngles.x + 10;
            }
            
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, xAxisOrbit, rotationDamping * Time.deltaTime);
            currentHeight = Mathf.LerpAngle(currentHeight, yAxisOrbit, rotationDamping * Time.deltaTime); 
        }
        Quaternion currentRotation = Quaternion.Euler(currentHeight, currentRotationAngle, 0);
        transform.position = target.position;
        transform.position -= currentRotation * Vector3.forward * distance;

    }

    void collisionCheck()
    {
        float resetDistance = 0f;

        occludedDistance = GetComponent<CameraCollision>().CheckCameraPoints(target.position, transform.position, distance);

        pos = transform.position;
        pos -= transform.forward * 5;
        resetDistance = GetComponent<CameraCollision>().CheckCameraPoints(target.position, pos, maxDistance);
        
      
        if(occludedDistance > resetDistance)
            distance = Mathf.Lerp(distance, occludedDistance, Time.deltaTime * smoothTime);

        if (resetDistance > occludedDistance)
            distance = Mathf.Lerp(distance, resetDistance, Time.deltaTime * smoothTime);

        distance = Mathf.Clamp(distance, 1, 5);
    }

   
}
