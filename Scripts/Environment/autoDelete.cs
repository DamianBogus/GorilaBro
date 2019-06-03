using UnityEngine;
using System.Collections;


public class autoDelete : MonoBehaviour {

    float timer = 0;
    public float duration = 2.0f;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        DeleteObject();
	}

    void DeleteObject()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            Destroy(gameObject);
        }
    }
}
