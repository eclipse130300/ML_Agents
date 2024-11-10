using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PongMoveBall : MonoBehaviour
{
    Vector3 ballStartPosition;
    Rigidbody2D rb;
    public float speed = 400;
    public AudioSource blip;
    public AudioSource blop;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballStartPosition = transform.position;
        ResetBall();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "backwall")
            blop.Play();
        else
            blip.Play();
    }

    public void ResetBall()
    {
        transform.position = ballStartPosition;
        rb.velocity = new Vector2(0, 0);
        
        Vector3 dir = new Vector3(Random.Range(100,300), Random.Range(-100,100), 0).normalized;
        rb.AddForce(dir * speed);
    }

    private void Update()
    {
        if(Input.GetKeyDown("space"))
            ResetBall();
    }
}
