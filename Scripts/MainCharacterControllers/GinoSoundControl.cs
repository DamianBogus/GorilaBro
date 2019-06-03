using UnityEngine;
using System.Collections;

public class GinoSoundControl : MonoBehaviour {
    public AudioSource audioSource;

    [Header("Movement Sound Effects")]
    public AudioClip[] forestFootSteps;
    public AudioClip[] jumpSounds;
    public AudioClip[] WallSounds;

    [Header("Combat Sound Effects")]
    public AudioClip[] attacks;
    public AudioClip[] woosh;

    CharacterControl player;
    bool inAir;
    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
        player = transform.root.GetComponent<CharacterControl>();
    }
    void Update()
    {
        checkLanding();
    }

    void checkLanding()
    {
        if (!player.moveSettings.isGrounded)
            inAir = true;

        if (player.moveSettings.isGrounded)
        {
            if (inAir)
                Land();

            inAir = false;
        }
    }
    public void BreakWall(int i)
    {
        AudioClip sound = forestFootSteps[0];

        if (i == 0)
            sound = Resources.Load("Sounds/Bricks") as AudioClip;

        if (i == 1)
            sound = Resources.Load("Sounds/Glass") as AudioClip;

        audioSource.PlayOneShot(sound);
    }
    public void Attack(int clip)
    {
        int i = 0;
        switch (clip)
        {
            case 15: audioSource.PlayOneShot(attacks[0]);
                     i = 0;
                     break;
            case 30: audioSource.PlayOneShot(attacks[1]);
                     i = 1;
                     break;
            case 50: audioSource.PlayOneShot(attacks[2]);
                     i = 2;
                     break;
            case 35:
            case 10: audioSource.PlayOneShot(attacks[3]);
                     i = 3;
                     break;
            case 28:
            case 60: audioSource.PlayOneShot(attacks[4]);
                     i = 4;
                     break;
            case 100: audioSource.PlayOneShot(attacks[5]);
                      i = 6;
                     break;
            default: PlayMissSound();
                     i = 100;
                     break;
        }
        if (i != 100)
        {
            GameObject exp = Instantiate(Resources.Load("Combat Effects/attackFX" + i)) as GameObject;

            exp.transform.position = transform.position + transform.forward + Vector3.up;
        }
    }

    void PlayMissSound()
    {
        int rand = Random.Range(0, 2);

        audioSource.PlayOneShot(woosh[rand]);
    }
    void LeftStep()
    {
          audioSource.PlayOneShot(forestFootSteps[Random.Range(0, 5)], 0.3f);
    }

    void RightStep()
    {
          audioSource.PlayOneShot(forestFootSteps[Random.Range(0, 5)], 0.3f);
    }

    void Jump()
    {
        audioSource.PlayOneShot(jumpSounds[0], 0.3f);
    }

    void Land()
    {
        audioSource.PlayOneShot(jumpSounds[1], 0.3f);
    }
}
