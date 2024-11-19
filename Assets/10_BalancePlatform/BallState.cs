using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallState : MonoBehaviour
{
    public bool dropped = false;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("drop"))
        {
            dropped = true;
        }
    }
}
