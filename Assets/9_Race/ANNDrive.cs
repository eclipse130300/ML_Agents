using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class ANNDrive : MonoBehaviour
{
    ANN8 ann;
    public float visibleDistance = 200;
    public int epochs = 1000;
    public float speed = 50f;
    public float rotationSpeed = 100f;
    
    bool trainingDone = false;
    float trainingProgress = 0;
    double sse = 0;
    double lastSSE = 1;

    public float translation;
    public float rotation;
    
    public bool loadWeightsFromFile = false;

    private void Start()
    {
        ann = new ANN8(5, 2, 1, 10, 0.5f);

        if (loadWeightsFromFile)
        {
            LoadWeightsFromFile();
            trainingDone = true;
        }
        else
        {
            StartCoroutine(LoadTrainingSet());
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + sse);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + ann.alpha);
        GUI.Label(new Rect(25, 55, 250, 30), "Trained: " + trainingProgress + "%");
    }

    IEnumerator LoadTrainingSet()
    {
        string path = Application.dataPath + "/9_Race/trainingData.txt";
        string line;

        if (File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader tdf = File.OpenText(path);
            List<double> calcOutputs = new List<double>();
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            for (int i = 0; i < epochs; i++)
            {
                sse = 0;
                tdf.BaseStream.Position = 0;
                string currentWeights = ann.PrintWeights();
                
                while ((line = tdf.ReadLine()) != null)
                {
                    string[] data = line.Split(',');

                    float thisError = 0;
                    var data5 = ConvertFromString(data[5]);
                    var data6 = ConvertFromString(data[6]);
                    
                    if(data5 != 0f && data6 != 0f)
                    {
                        inputs.Clear();
                        outputs.Clear();
                        
                        inputs.Add(ConvertFromString(data[0]));
                        inputs.Add(ConvertFromString(data[1]));
                        inputs.Add(ConvertFromString(data[2]));
                        inputs.Add(ConvertFromString(data[3]));
                        inputs.Add(ConvertFromString(data[4]));
                        
                        double o1 = Map(0, 1, -0.5f, 0.5f, ConvertToSingleFromString(data[5]));
                        outputs.Add(o1);
                        
                        double o2 = Map(0, 1, -0.5f, 0.5f, ConvertToSingleFromString(data[6]));
                        outputs.Add(o2);

                        calcOutputs = ann.Train(inputs, outputs);
                        thisError = Mathf.Pow((float)(outputs[0] - calcOutputs[0]), 2);
                        thisError += Mathf.Pow((float)(outputs[1] - calcOutputs[1]), 2);
                        thisError /= 2f;
                    }
                    
                    sse += thisError;
                }
                
                trainingProgress = ((float)i / (float)epochs);
                sse /= lineCount;
                
                if(lastSSE < sse)
                {
                    ann.LoadWeights(currentWeights);
                    ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f, 0.01f, 0.9f);
                }
                else
                {
                    ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f, 0.01f, 0.9f);
                    lastSSE = sse;
                }
                
                yield return null;
            }
            
        }

        trainingDone = true;
        SaveWeightsToFile();
    }

    private void SaveWeightsToFile()
    {
        string path = Application.dataPath + "/9_Race/weights.txt";
        StreamWriter sw = File.CreateText(path);
        sw.WriteLine(ann.PrintWeights());
        sw.Close();
    }

    void LoadWeightsFromFile()
    {
        string path = Application.dataPath + "/9_Race/weights.txt";
        StreamReader sw = File.OpenText(path);
        
        if(File.Exists(path))
        {
            string line = sw.ReadLine();
            ann.LoadWeights(line);
        }
    }

    private double ConvertFromString(string str)
    {
        return System.Convert.ToDouble(str, CultureInfo.InvariantCulture);
    }

    private float ConvertToSingleFromString(string str)
    {
        return System.Convert.ToSingle(str, CultureInfo.InvariantCulture);
    }
    
    float Map(float newFrom, float newTo, float origFrom, float origTo, float value)
    {
        if (value <= origFrom)
        {
            return newFrom;
        }
        else if (value >= origTo)
        {
            return newTo;
        }
        return (newTo - newFrom) * ((value - origFrom) / (origTo - origFrom)) + newFrom;
    }

    private void Update()
    {
        if(!trainingDone)
            return;
        
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();
        List<double> calcOutputs = new List<double>();
        
        RaycastHit hit;
        float fDist = 0,
              rDist = 0,
              lDist = 0,
              r45Dist = 0,
              l45Dist = 0;

        if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
        {
            fDist = 1 - (hit.distance / visibleDistance);
        }
        
        if (Physics.Raycast(transform.position, transform.right, out hit, visibleDistance))
        {
            rDist = 1 - (hit.distance / visibleDistance);
        }
        
        if (Physics.Raycast(transform.position, -transform.right, out hit, visibleDistance))
        {
            lDist = 1 - (hit.distance / visibleDistance);
        }
        
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, transform.up) * transform.right, out hit, visibleDistance))
        {
            r45Dist = 1 - (hit.distance / visibleDistance);
        }
        
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, transform.up) * -transform.right, out hit, visibleDistance))
        {
            l45Dist = 1 - (hit.distance / visibleDistance);
        }
        
        inputs.Add(fDist);
        inputs.Add(rDist);
        inputs.Add(lDist);
        inputs.Add(r45Dist);
        inputs.Add(l45Dist);
        
        outputs.Add(0);
        outputs.Add(0);
        
        calcOutputs = ann.CalcOutput(inputs, outputs);
        float translationInput = (float)Map(-1, 1, -0.5f, 0.5f, (float)calcOutputs[0]);
        float rotationInput = (float)Map(-1, 1, -0.5f, 0.5f, (float)calcOutputs[1]);
        translation = Time.deltaTime * translationInput * speed;
        rotation = Time.deltaTime * rotationInput * rotationSpeed;
        
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }
}
