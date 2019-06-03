using UnityEngine;
using System.Collections;

public class FreezeRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        StopRotation();
	}
    void StopRotation()
    {
        Quaternion currentRot = transform.rotation;
        currentRot.x = 0;
        currentRot.z = 0;
        transform.rotation = currentRot;
    }
}
