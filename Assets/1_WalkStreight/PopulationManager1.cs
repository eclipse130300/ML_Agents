using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationManager1 : MonoBehaviour
{
    public GameObject botPrefab;
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
        GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + generation, guiStyle);
        GUI.Label(new Rect(10, 35, 100, 20), "Trial Time: " + (int)elapsed, guiStyle);
    }

    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 startingPos = new Vector3(this.transform.position.x + Random.Range(-2, 2),
                this.transform.position.y,
                this.transform.position.z + Random.Range(-2, 2));
            GameObject b = Instantiate(botPrefab, startingPos, this.transform.rotation);
            b.GetComponent<Brain1>().Init();
            population.Add(b);
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 startingPos = new Vector3(this.transform.position.x + Random.Range(-2, 2),
            this.transform.position.y,
            this.transform.position.z + Random.Range(-2, 2));
        GameObject offspring = Instantiate(botPrefab, startingPos, this.transform.rotation);
        Brain1 b = offspring.GetComponent<Brain1>();
        if (Random.Range(0, 100) == 1)
        {
            b.Init();
            b.dna.Mutate();
        }
        else
        {
            b.Init();
            b.dna.Combine(parent1.GetComponent<Brain1>().dna, parent2.GetComponent<Brain1>().dna);
        }
        return offspring;
    }

    void BreedNewPopulation()
    {
        // get rid of unfit individuals
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain1>().distanceTravelled).ToList();

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
