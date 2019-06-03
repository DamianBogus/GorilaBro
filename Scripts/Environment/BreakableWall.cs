using UnityEngine;
using System.Collections;

public class BreakableWall : MonoBehaviour
{
    CharacterControl player;
    GinoSoundControl sound;
    public GameObject explosionPrefab;
    public int i = 1;
    Material mat;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControl>();
        sound = GameObject.FindGameObjectWithTag("playerModel").GetComponent<GinoSoundControl>();
        mat = transform.GetComponent<Renderer>().material;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag != "Player" || !player.moveSettings.anim.GetCurrentAnimatorStateInfo(0).IsName("Charge"))
            return;

        float checkDirection = Vector3.Dot(transform.forward, player.transform.forward);

            if (Mathf.Abs(checkDirection) > 0.6f)
            {
                float dir = checkDirection / Mathf.Abs(checkDirection);
                transform.Rotate(transform.forward * dir, 15.0f, Space.Self);
                Destroy(this.gameObject);
                sound.BreakWall(i);
                GameObject exp = Instantiate(explosionPrefab, transform.position + player.transform.forward, transform.rotation) as GameObject;
                for (int j = 0; j < exp.transform.childCount; j++)
                {
                    exp.transform.GetChild(j).GetComponent<Renderer>().material = mat;
                }
            //Instantiate(Resources.Load("rubble"), transform.position, transform.rotation);

        }
    }
}

