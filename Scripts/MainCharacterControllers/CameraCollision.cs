using UnityEngine;
using System.Collections;

public class CameraCollision : MonoBehaviour
{
    //Camera and Player Transforms
    private static Transform camTransform;
    private Transform playerTransform;

    
    //Camera Variables
    private static Camera mainCam;
    private static Vector3 pos;
    public float distance;

    public struct ClipPlanePoints
    {
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
    }

    void Start()
    {
        mainCam = Camera.main;
        camTransform = mainCam.transform;
        playerTransform = GameObject.FindGameObjectWithTag("CameraTarget").transform;
    }

    public float CheckCameraPoints(Vector3 from, Vector3 to, float currentDistance)
    {
        float nearestDistance = -1f;

        RaycastHit hitInfo;

        ClipPlanePoints clipPlanePoints = ClipPlaneAtNear(to);
        //Debug Lines
        Debug.DrawLine(from, to + camTransform.forward * -mainCam.nearClipPlane, Color.red);
        Debug.DrawLine(from, clipPlanePoints.topLeft, Color.blue);
        Debug.DrawLine(from, clipPlanePoints.topRight, Color.blue);
        Debug.DrawLine(from, clipPlanePoints.bottomLeft, Color.blue);
        Debug.DrawLine(from, clipPlanePoints.bottomRight, Color.blue);

        Debug.DrawLine(clipPlanePoints.topLeft, clipPlanePoints.topRight, Color.green);
        Debug.DrawLine(clipPlanePoints.topRight, clipPlanePoints.bottomRight, Color.green);
        Debug.DrawLine(clipPlanePoints.bottomRight, clipPlanePoints.bottomLeft, Color.green);
        Debug.DrawLine(clipPlanePoints.bottomLeft, clipPlanePoints.topLeft, Color.green);


        if(Physics.Linecast(from, clipPlanePoints.topLeft, out hitInfo) && hitInfo.collider.tag != "Player")
             nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.bottomLeft, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.bottomRight, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.topRight, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, to + camTransform.forward * -mainCam.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "MainCamera")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
                nearestDistance = hitInfo.distance;

        if (nearestDistance != -1f)
             return nearestDistance;

        return currentDistance;
    }
    
    public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
    {
        var clipPlanePoints = new ClipPlanePoints();

        float halfFOV = (mainCam.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = mainCam.aspect;
        float nearClipPlaneDist = mainCam.nearClipPlane;
        float height = nearClipPlaneDist * Mathf.Tan(halfFOV);
        float width = height * aspect;

        //Bottom Right
        clipPlanePoints.bottomRight = pos + camTransform.right * width;
        clipPlanePoints.bottomRight -= camTransform.up * height;
        clipPlanePoints.bottomRight += camTransform.forward * nearClipPlaneDist;
        //Bottom Left
        clipPlanePoints.bottomLeft = pos - camTransform.right * width;
        clipPlanePoints.bottomLeft -= camTransform.up * height;
        clipPlanePoints.bottomLeft += camTransform.forward * nearClipPlaneDist;
        //Top Right
        clipPlanePoints.topRight = pos + camTransform.right * width;
        clipPlanePoints.topRight += camTransform.up * height;
        clipPlanePoints.topRight += camTransform.forward * nearClipPlaneDist;
        //Top Left
        clipPlanePoints.topLeft = pos - camTransform.right * width;
        clipPlanePoints.topLeft += camTransform.up * height;
        clipPlanePoints.topLeft += camTransform.forward * nearClipPlaneDist;

        return clipPlanePoints;
    }
}