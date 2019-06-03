using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
    CharacterControl player;

    [Header("Spin and Float Settings")]
    public float spinSpeed = 120.0f;
    public float frequency = 1.0f;
    public float amplitude = 3.0f;

    public bool move = true;

    Vector3 pos;
    Vector3 tempPos;
    AudioSource audioSource;

    void Start()
    {
        pos = transform.position;

        if (!move)
            transform.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)), ForceMode.Impulse);
    }
    void Update()
    {
        if(move)
        MovePickup();

    }

    void MovePickup()
    {
        transform.Rotate(transform.up * Time.deltaTime * spinSpeed);
        tempPos = pos;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetType() == typeof(SphereCollider))
            return;

        if(col.tag == "Player" && transform.tag == "healthPickup")
        {
            player = col.gameObject.GetComponent<CharacterControl>();
            player.vitalsSettings.health += 30.0f;
            AudioClip clip = Resources.Load("Sounds/health") as AudioClip;
            GameObject exp = Instantiate(Resources.Load("healthPickupExp")) as GameObject;
            audioSource = exp.GetComponent<AudioSource>();
            audioSource.PlayOneShot(clip);
            exp.transform.position = transform.position;
            Destroy(gameObject);

        }

        if (col.tag == "Player" && transform.tag == "energyPickup")
        {
            player = col.gameObject.GetComponent<CharacterControl>();
            player.vitalsSettings.energy += 30.0f;
            AudioClip clip = Resources.Load("Sounds/energy") as AudioClip;
            GameObject exp = Instantiate(Resources.Load("energyPickupExp")) as GameObject;
            audioSource = exp.GetComponent<AudioSource>();
            audioSource.PlayOneShot(clip);
            exp.transform.position = transform.position;
            Destroy(gameObject);

        }

        if (col.tag == "Player" && transform.tag == "coinPickup")
        {
            player = col.gameObject.GetComponent<CharacterControl>();
            player.vitalsSettings.coins++;
            AudioClip clip = Resources.Load("Sounds/coin") as AudioClip;
            GameObject exp = Instantiate(Resources.Load("coinPickupExp")) as GameObject;
            audioSource = exp.GetComponent<AudioSource>();
            audioSource.PlayOneShot(clip);
            exp.transform.position = transform.position;
            Destroy(gameObject);

        }
    }
}
