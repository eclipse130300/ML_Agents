using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationManager3 : MonoBehaviour
{
    public GameObject botPrefab;
    public GameObject startPos;
    public GameObject finishPos;
    public int populationSize = 50;
    List<GameObject> population = new List<GameObject>();
    public static float elapsed = 0;
    public float trialTime = 5;
    int generation = 1;

    GUIStyle guiStyle = new GUIStyle();

    void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Generation: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
        
    }

    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 startingPos = startPos.transform.position;
            GameObject b = Instantiate(botPrefab, startingPos, this.transform.rotation);
            b.GetComponent<Brain3>().Init(finishPos.transform.position);

            population.Add(b);
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 startingPos = startPos.transform.position;
        GameObject offspring = Instantiate(botPrefab, startingPos, this.transform.rotation);
        Brain3 b = offspring.GetComponent<Brain3>();
        if (Random.Range(0, 100) == 1)
        {
            b.Init(finishPos.transform.position);
            b.dna.Mutate();
        }
        else
        {
            b.Init(finishPos.transform.position);
            b.dna.Combine(parent1.GetComponent<Brain3>().dna, parent2.GetComponent<Brain3>().dna);
        }
        return offspring;
    }

    void BreedNewPopulation()
    {
        // get rid of unfit individuals
        
        List<GameObject> sortedList = population.OrderByDescending(x => x.GetComponent<Brain3>().distanceToFinish).ToList();
        population.Clear();

        // breed upper half of sorted list
        for (int i = (int)(sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }
        
        // destroy all parents and previous population
        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }
        
        generation++;
    }
    
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= trialTime)
        {
            BreedNewPopulation();
            elapsed = 0;
        }
    }
}
