using System;
using UnityEngine;

public class Brain3 : MonoBehaviour
{
    [SerializeField]
    private GameObject nose;
    
    int DNALength = 60;
    public float distanceToFinish;
    public DNA3 dna;

    private Vector3 finishPos;
    private int collisionAmount = 0;

    public float turn;

    public void Init(Vector3 finish)
    {
        // Initialize DNA
        // 0 forward
        // 1 left
        // 2 right
        dna = new DNA3(DNALength, 2);
        finishPos = finish;
    }
    
    private void FixedUpdate()
    {
        // read DNA
        float turn = 0;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        
        transform.Translate(0, 0, 8f * Time.deltaTime);
        distanceToFinish = Vector3.Distance(transform.position, finishPos);
        if(distanceToFinish < 3f)
            return;
        
        var colliders = Physics.OverlapBox(nose.transform.position, Vector3.one * nose.transform.localScale.x / 2, nose.transform.rotation);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "wall")
            {
                var turnType = GetCollisionTurn();
        
                if (turnType == 0)
                {
                    turn = -90;
                }
                else if (turnType == 1)
                {
                    turn = 90;
                }
                
                this.transform.Rotate(0, turn, 0);
                
                return;
            }
        }
        

        transform.Translate(0, 0, 32f * Time.deltaTime);
    }

    /*private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "wall")
        {
            var turnType = GetCollisionTurn();
        
            if (turnType == 0)
            {
                turn = -90;
            }
            else if (turnType == 1)
            {
                turn = 90;
            }
                
            this.transform.Rotate(0, turn, 0);
            collisionAmount++;
        }
    
    }*/

    private int GetCollisionTurn()
    {
        var genePos = collisionAmount % DNALength;
        collisionAmount++;
        return dna.GetGene(genePos);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(nose.transform.position, nose.transform.localScale);
    }
}
