using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NeuralNetworkToolkit;
using UnityEngine;
using UnityStandardAssets.Vehicles.Ball;
public class PlatformBrain : MonoBehaviour
{
    [Range(1, 100f)]
    public float timeScale = 100.0f;
    public GameObject ball;

    ANN_DeepLearning ann;

    float reward = 0.0f;
    List<Replay> replayMemory = new List<Replay>();
    int mCapacity = 10000;

    float discount = 0.99f;
    float exploreRate = 50.0f;
    float maxExploreRate = 100.0f;
    float minExploreRate = 0.01f;
    float exploreDecay = 0.0001f;

    Vector3 ballStartPosition;
    int failCount = 0;
    float tiltSpeed = 0.5f;

    float timer = 0;
    float maxBalanceTime = 0;

    private void Start()
    {
        ann = new ANN_DeepLearning(3, 2, 1, 6, 0.5f, ActivationFunctionType.TanH, ActivationFunctionType.Sigmoid);
        ballStartPosition = ball.transform.position;
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
        GUI.Label(new Rect(10, 50, 400, 30), "Decay Rate: " + exploreRate, guiStyle);
        GUI.Label(new Rect(10, 75, 400, 30), "Last B Balance: " + maxBalanceTime, guiStyle);
        GUI.Label(new Rect(10, 100, 400, 30), "This Balance: " + timer, guiStyle);
        GUI.EndGroup();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
            ResetBall();
        
        Time.timeScale = timeScale;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        states.Add(transform.position.x);
        states.Add(ball.transform.position.z);
        states.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);

        qs = NeuralNetworkTools.SoftMax(ann.CalcOutput(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        if (UnityEngine.Random.Range(0, 100) < exploreRate)
            maxQIndex = UnityEngine.Random.Range(0, 2);

        var qsMax = qs[maxQIndex];
        if(qsMax == 0f)
            Debug.Log("ZERO!? ");

        if (maxQIndex == 0)
            transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
        else if (maxQIndex == 1)
            transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);

        if (ball.GetComponent<HitState>().hitted)
        {
            reward = -1.0f;
        }
        else
        {
            reward = 0.1f;
        }

        Replay lastMemory = new Replay(transform.position.x, ball.transform.position.z, ball.GetComponent<Rigidbody>().angularVelocity.x, reward);
        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        replayMemory.Add(lastMemory);

        if (ball.GetComponent<HitState>().hitted)
        {
            for (int i = replayMemory.Count - 1; i >= 0; i--)
            {
                List<double> toutputsOld = new List<double>();
                List<double> toutputsNew = new List<double>();
                toutputsOld = NeuralNetworkTools.SoftMax(ann.CalcOutput(replayMemory[i].states));

                double maxQOld = toutputsOld.Max();
                int action = toutputsOld.ToList().IndexOf(maxQOld);

                double feedback;
                if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                    feedback = replayMemory[i].reward;
                else
                {
                    toutputsNew = NeuralNetworkTools.SoftMax(ann.CalcOutput(replayMemory[i + 1].states));
                    maxQ = toutputsNew.Max();
                    feedback = (replayMemory[i].reward + discount * maxQ);
                }

                toutputsOld[action] = feedback;
                ann.Train(replayMemory[i].states, toutputsOld);
            }

            if (timer > maxBalanceTime)
                maxBalanceTime = timer;

            timer = 0;

            ball.GetComponent<HitState>().hitted = false;
            transform.rotation = Quaternion.identity;
            ResetBall();
            replayMemory.Clear();
            failCount++;
        }
    }

    private void ResetBall()
    {
        ball.transform.position = ballStartPosition;
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        ball.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
    }
}
