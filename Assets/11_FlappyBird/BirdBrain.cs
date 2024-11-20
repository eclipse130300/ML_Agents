using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NeuralNetworkToolkit;
using UnityEngine;
using UnityStandardAssets.Vehicles.Ball;

public class BirdBrain : MonoBehaviour
{
    [Range(1, 100f)]
    public float timeScale = 100.0f;
    //public GameObject ball;
    public GameObject bird;
    
    ANN_DeepLearning ann;

    float reward = 0.0f;
    List<Replay> replayMemory = new List<Replay>();
    int mCapacity = 10000;

    float discount = 0.99f;
    float exploreRate = 50.0f;
    float maxExploreRate = 100.0f;
    float minExploreRate = 0.01f;
    float exploreDecay = 0.0001f;

    Vector3 birdStartPosition;
    int failCount = 0;

    float timer = 0;
    float maxBalanceTime = 0;
    
    float maxFlapForce = 5.0f;
    float flapTimeMax = 0.3f;
    float flapTimer = 0.0f;

    private void Start()
    {
        ann = new ANN_DeepLearning(4, 1, 1, 8, 0.3f, ActivationFunctionType.TanH, ActivationFunctionType.Sigmoid);
        birdStartPosition = bird.transform.position;
        //Time.timeScale = 100.0f;
    }

    GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 400, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 400, 30), "Fails: " + failCount, guiStyle);
        GUI.Label(new Rect(10, 50, 400, 30), "Explore Rate: " + exploreRate, guiStyle);
        GUI.Label(new Rect(10, 75, 400, 30), "Last Attempt t: " + maxBalanceTime, guiStyle);
        GUI.Label(new Rect(10, 100, 400, 30), "This Attempt t: " + timer, guiStyle);
        GUI.EndGroup();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
            ResetBird();
        
        Time.timeScale = timeScale;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        List<double> states = new List<double>();
        List<double> qs = new List<double>();
        
        var distnaceDown = float.MaxValue;
        var layerMask = 1 << 7;
        
        var hit = Physics2D.Raycast(bird.transform.position, Vector2.down, 50f, layerMask);
        if (hit.collider != null)
        {
            if(hit.collider.gameObject.tag == "drop")
            {
                distnaceDown = hit.distance;
            }
        }
        
        var distnaceUp = float.MaxValue;
        hit = Physics2D.Raycast(bird.transform.position, Vector2.up, 50f, layerMask);
        if (hit.collider != null)
        {
            if(hit.collider.gameObject.tag == "drop")
            {
                distnaceUp = hit.distance;
            }
        }
        
        states.Add(bird.transform.position.y);
        states.Add(distnaceDown);
        states.Add(distnaceUp);
        
        var rb2d = bird.GetComponent<Rigidbody2D>();
        states.Add(rb2d.velocity.y);
        
        qs = ann.CalcOutput(states);
        var force = (float)qs[0];
        //double maxQ = qs.Max();
        //int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        /*if (UnityEngine.Random.Range(0, 100) < exploreRate)
            force = -force;*/

        //var qsMax = qs[maxQIndex];

        flapTimer += Time.deltaTime;
        if (flapTimer > flapTimeMax)
        {
            flapTimer = 0;
            rb2d.AddForce(Vector2.up * force * maxFlapForce, ForceMode2D.Impulse);
            Debug.Log("Force: " + force);
        }

        /*if (maxQIndex == 0)
            transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
        else if (maxQIndex == 1)
            transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);*/

        if (bird.GetComponent<HitState>().hitted)
        {
            reward = -1.0f;
        }
        else
        {
            reward = 0.05f;
        }
        
        Replay lastMemory = new Replay(reward, states.ToArray());

        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        replayMemory.Add(lastMemory);

        if (bird.GetComponent<HitState>().hitted)
        {
            for (int i = replayMemory.Count - 1; i >= 0; i--)
            {
                List<double> toutputsOld = new List<double>();
                List<double> toutputsNew = new List<double>();
                toutputsOld = ann.CalcOutput(replayMemory[i].states);

                double maxQOld = toutputsOld.Max();
                int action = toutputsOld.ToList().IndexOf(maxQOld);

                double feedback;
                if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                    feedback = replayMemory[i].reward;
                else
                {
                    toutputsNew = ann.CalcOutput(replayMemory[i + 1].states);
                    var maxQ = toutputsNew.Max();
                    feedback = (replayMemory[i].reward + discount * maxQ);
                }

                toutputsOld[action] = feedback;
                ann.Train(replayMemory[i].states, toutputsOld);
            }

            if (timer > maxBalanceTime)
                maxBalanceTime = timer;

            timer = 0;

            bird.GetComponent<HitState>().hitted = false;
            transform.rotation = Quaternion.identity;
            ResetBird();
            replayMemory.Clear();
            failCount++;
        }
    }

    private void ResetBird()
    {
        bird.transform.position = birdStartPosition;
        bird.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
    }
}
