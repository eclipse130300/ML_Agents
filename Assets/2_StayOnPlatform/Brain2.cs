using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brain2 : MonoBehaviour
{
    int DNALength = 2;
    public float timeAlive;
    public  float timeWalking;
    public float distanceToClosestBro;
    public DNA2 dna;
    public GameObject eyes;
    bool alive = true;
    bool seeGround = true;

    public GameObject ethanPrefab;
    GameObject ethan;

    private void OnDestroy()
    {
        Destroy(ethan);
    }

    void OnCollisionEnter(Collision obj)
    {
        if (obj.gameObject.tag == "dead")
        {
            alive = false;
        }
    }

    public void Init()
    {
        // Initialize DNA
        // 0 forward
        // 1 left
        // 2 right
        dna = new DNA2(DNALength, 3);
        timeAlive = 0;
        alive = true;
        ethan = Instantiate(ethanPrefab, this.transform.position, this.transform.rotation);
        ethan.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().target = this.transform;
    }

    void Update()
    {
        if (!alive) return;

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10f, Color.red, 0.1f);
        seeGround = false;
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10f, out hit))
        {
            if (hit.collider.tag == "platform")
            {
                seeGround = true;
            }
        }
        timeAlive = PopulationManager2.elapsed;

        var allbrosInRadius = Physics.OverlapSphere(this.transform.position, 2f, 1 << 6);
        foreach (var bro in allbrosInRadius)
        {
            if (bro.gameObject == this.gameObject) continue;
            var dist = Vector3.Distance(this.transform.position, bro.transform.position);
            if (dist < distanceToClosestBro)
            {
                distanceToClosestBro += 1/dist;
            }
        }
        
        // read DNA
        float turn = 0;
        float move = 0;
        if (seeGround)
        {
            if (dna.GetGene(0) == 0)
            {
                move = 1;
                timeWalking += Time.deltaTime;
            }
            else if (dna.GetGene(0) == 1) turn = -90;
            else if (dna.GetGene(0) == 2) turn = 90;
        }
        else
        {
            if (dna.GetGene(1) == 0)
            {
                move = 1;
                timeWalking += Time.deltaTime;
            }
            else if (dna.GetGene(1) == 1) turn = -90;
            else if (dna.GetGene(1) == 2) turn = 90;
        }

        this.transform.Translate(0, 0, move * 8f * Time.deltaTime);
        this.transform.Rotate(0, turn, 0);
    }
}
