using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer7 {

	public int numNeurons;
	public List<Neuron7> neurons = new List<Neuron7>();

	public Layer7(int nNeurons, int numNeuronInputs)
	{
		numNeurons = nNeurons;
		for(int i = 0; i < nNeurons; i++)
		{
			neurons.Add(new Neuron7(numNeuronInputs));
		}
	}
}
