using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour
{
    public float r;
    public float g;
    public float b;
    
    public float scaleMultiplier = 1f;
    
    bool dead = false;
    public float timeToDie = 0;
    
    SpriteRenderer sRenderer;
    Collider2D sCollider;

    private void OnMouseDown()
    {
        dead = true;
        timeToDie = PopulationManager.elapsed;
        Debug.Log("Dead At: " + timeToDie);
        sRenderer.enabled = false;
        sCollider.enabled = false;
    }

    private void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        sCollider = GetComponent<Collider2D>();
        sRenderer.color = new Color(r, g, b);
        transform.localScale *= scaleMultiplier;
    }
}
