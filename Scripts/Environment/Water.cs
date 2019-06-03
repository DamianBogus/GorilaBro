using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

    public float waterSpeed = 1.0f;
    public Renderer rend;
    public Animator fadeAnim;
    public bool freezeGino;
    Vector2 offset = Vector2.zero;
    public Transform respawnPoint;
	void Start ()
    {
        //  mat = GetComponent<Material>();
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        offset.y += Time.deltaTime * waterSpeed;
        rend.material.SetTextureOffset("_MainTex", offset);
	}

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            float height = col.gameObject.transform.position.y;
            if (height < 11)
            {
                StartCoroutine("RespawnPlayer", col.transform);
                
            }
        }
    }

    IEnumerator RespawnPlayer(Transform gino)
    {
        fadeAnim.Play("WaterFade");
        freezeGino = true;
        yield return new WaitForSeconds(0.35f);
        freezeGino = false;
        gino.position = respawnPoint.position;
    }
}

