using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replay
{
    public List<double> states;
    public double reward;

    public Replay(double xr, double ballz, double ballvx, double r)
    {
        states = new List<double>();
        states.Add(xr);
        states.Add(ballz);
        states.Add(ballvx);
        reward = r;
    }
}

public class PlatformBrain : MonoBehaviour
{

    public GameObject ball;
    
    ANN_DeepLearning ann;
    
    float reward = 0.0f;
    List<Replay> replayMemory = new List<Replay>();
    int mCapacity = 10000;
    
    float discount = 0.99f;
    float exploreRate = 100.0f;
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
        
    }
}
