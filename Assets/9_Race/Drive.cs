using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public float speed = 50.0f;
    public float rotationSpeed = 100.0f;
    public float visibleDistance = 200.0f;
    List<string> collectedTrainingData = new List<string>();
    StreamWriter tdf;

    private void Start()
    {
        string path = Application.dataPath + "/9_Race/trainingData.txt";
        tdf = File.CreateText(path);
    }

    private void OnApplicationQuit()
    {
        foreach (string td in collectedTrainingData)
        {
            tdf.WriteLine(td);
        }
        tdf.Close();
        Debug.Log("Training data saved.");
    }

    float Round(float x)
    {
        return (float)Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
    }

    void Update()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");
        
        /*Debug.Log("translationInput: " + translationInput);
        Debug.Log("rotationInput: " + rotationInput);*/
        
        float translation = Time.deltaTime * translationInput * speed;
        float rotation = Time.deltaTime * rotationInput * rotationSpeed;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);
        
        Debug.DrawRay(transform.position, transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, transform.right * visibleDistance, Color.red);
        
        RaycastHit hit;
        float fdist = 0,
              rdist = 0,
              ldist = 0,
              r45dist = 0,
              l45dist = 0;

        if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
        {
            fdist = 1 - Round(hit.distance / visibleDistance);
        }
        
        if (Physics.Raycast(transform.position, transform.right, out hit, visibleDistance))
        {
            rdist = 1 - Round(hit.distance / visibleDistance);
        }
        
        if (Physics.Raycast(transform.position, -transform.right, out hit, visibleDistance))
        {
            ldist = 1 - Round(hit.distance / visibleDistance);
        }
        
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, transform.up) * transform.right, out hit, visibleDistance))
        {
            r45dist = 1 - Round(hit.distance / visibleDistance);
        }
        
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, transform.up) * -transform.right, out hit, visibleDistance))
        {
            l45dist = 1 - Round(hit.distance / visibleDistance);
        }
        
        var translationInputRounded = Round(translationInput).ToString(CultureInfo.InvariantCulture);
        var rotationInputRounded = Round(rotationInput).ToString(CultureInfo.InvariantCulture);
        
        /*Debug.Log("Translation input rounded: " + translationInputRounded);
        Debug.Log("Rotation input rounded: " + rotationInputRounded);*/
        
        string td = fdist.ToString(CultureInfo.InvariantCulture) + "," + rdist.ToString(CultureInfo.InvariantCulture) + "," + ldist.ToString(CultureInfo.InvariantCulture) + "," 
                  + r45dist.ToString(CultureInfo.InvariantCulture) + "," + l45dist.ToString(CultureInfo.InvariantCulture) + "," +
                    translationInputRounded
                  + "," + rotationInputRounded;
        
        if(!collectedTrainingData.Contains(td))
            collectedTrainingData.Add(td);
    }
}
