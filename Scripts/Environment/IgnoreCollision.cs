using UnityEngine;
using System.Collections;

public class IgnoreCollision : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>());
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
