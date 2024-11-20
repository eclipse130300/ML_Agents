using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : MonoBehaviour
{
    public bool hitted = false;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("drop"))
        {
            hitted = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("drop"))
        {
            hitted = true;
        }
    }
}
