using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    public float linearFadeStep = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var spriteColor = spriteRenderer.color;
        if (spriteColor.a > 0f)
        {
            spriteColor = new Color(spriteColor.r, spriteColor.g, spriteColor.b, spriteColor.a - linearFadeStep * Time.deltaTime);
            spriteRenderer.color = spriteColor;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}