using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer_DeepLearning {

	public int numNeurons;
	public List<Neuron_DeepLearning> neurons = new List<Neuron_DeepLearning>();

	public Layer_DeepLearning(int nNeurons, int numNeuronInputs)
	{
		numNeurons = nNeurons;
		for(int i = 0; i < nNeurons; i++)
		{
			neurons.Add(new Neuron_DeepLearning(numNeuronInputs));
		}
	}
}
