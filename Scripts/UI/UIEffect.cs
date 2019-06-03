using UnityEngine;
using System.Collections;

public class UIEffect : MonoBehaviour {

    ParticleSystem part;

    void Start()
    {
        part = GetComponent<ParticleSystem>();

    }
    
    void Update()
    {
        if (Time.timeScale < 0.01f)
        {
            part.Simulate(Time.unscaledDeltaTime, true, false);
        }
    }
}
