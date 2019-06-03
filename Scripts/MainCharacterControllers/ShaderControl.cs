using UnityEngine;
using System.Collections;

public class ShaderControl : MonoBehaviour {

    public CharacterControl player;

    Renderer rend;
    float outlineWidth;
    bool superMode;
    Color normal;
    Color super;

    float widthOsc;
    float frequency = 6.0f;
    float amplitude = 0.2f;
	void Start ()
    {
        rend = GetComponent<Renderer>();
        outlineWidth = rend.material.GetFloat("_Outline");
        widthOsc = outlineWidth; 
        normal = new Color(0.23f, 0.23f, 0.23f, 1.0f);
        super = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
       ChangeOutline();
    }

    void ChangeOutline()
    {
        superMode = player.superInput;

        if (!superMode)
        {
            rend.material.SetColor("_OutlineColor", normal);
            rend.material.SetFloat("_Outline", outlineWidth);
        }
        else
        {
            widthOsc += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
            rend.material.SetColor("_OutlineColor", super);
            rend.material.SetFloat("_Outline", widthOsc);
        }
    }
}
