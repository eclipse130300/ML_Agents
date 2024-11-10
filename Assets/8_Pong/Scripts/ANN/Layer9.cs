using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer9 {

	public int numNeurons;
	public List<Neuron9> neurons = new List<Neuron9>();

	public Layer9(int nNeurons, int numNeuronInputs)
	{
		numNeurons = nNeurons;
		for(int i = 0; i < nNeurons; i++)
		{
			neurons.Add(new Neuron9(numNeuronInputs));
		}
	}
}
