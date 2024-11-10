using System;
using UnityEngine;

public class MoveByPlayer : MonoBehaviour
{
    [SerializeField]
    private PuddleMover PuddleMover;
    
    private void Update()
    {
        float vY = Input.GetAxis("Vertical");
        PuddleMover.Move(vY);
    }
}