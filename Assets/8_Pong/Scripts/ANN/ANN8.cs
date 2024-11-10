﻿using System.Collections;
using System.Collections.Generic;
using NeuralNetworkToolkit;
using UnityEngine;

public class ANN8{

	public int numInputs;
	public int numOutputs;
	public int numHidden;
	public int numNPerHidden;
	public double alpha;
	List<Layer9> layers = new List<Layer9>();

	public ANN8(int nI, int nO, int nH, int nPH, double a)
	{
		numInputs = nI;
		numOutputs = nO;
		numHidden = nH;
		numNPerHidden = nPH;
		alpha = a;

		if(numHidden > 0)
		{
			layers.Add(new Layer9(numNPerHidden, numInputs));

			for(int i = 0; i < numHidden-1; i++)
			{
				layers.Add(new Layer9(numNPerHidden, numNPerHidden));
			}

			layers.Add(new Layer9(numOutputs, numNPerHidden));
		}
		else
		{
			layers.Add(new Layer9(numOutputs, numInputs));
		}
	}

	public List<double> Train(List<double> inputValues, List<double> desiredOutput)
	{
		List<double> outputValues = new List<double>();
		outputValues = CalcOutput(inputValues, desiredOutput);
		UpdateWeights(outputValues, desiredOutput);
		return outputValues;
	}

	public List<double> CalcOutput(List<double> inputValues, List<double> desiredOutput)
	{
		List<double> inputs = new List<double>();
		List<double> outputValues = new List<double>();
		int currentInput = 0;

		if(inputValues.Count != numInputs)
		{
			Debug.Log("ERROR: Number of Inputs must be " + numInputs);
			return outputValues;
		}

		inputs = new List<double>(inputValues);
		for(int i = 0; i < numHidden + 1; i++)
		{
				if(i > 0)
				{
					inputs = new List<double>(outputValues);
				}
				outputValues.Clear();

				for(int j = 0; j < layers[i].numNeurons; j++)
				{
					double N = 0;
					layers[i].neurons[j].inputs.Clear();

					for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
					{
					    layers[i].neurons[j].inputs.Add(inputs[currentInput]);
						N += layers[i].neurons[j].weights[k] * inputs[currentInput];
						currentInput++;
					}

					N -= layers[i].neurons[j].bias;

					if(i == numHidden)
						layers[i].neurons[j].output = ActivationFunctionO(N);
					else
						layers[i].neurons[j].output = ActivationFunction(N);
					
					outputValues.Add(layers[i].neurons[j].output);
					currentInput = 0;
				}
		}
		return outputValues;
	}

	public string PrintWeights()
	{
		string weightStr = "";
		foreach(Layer9 l in layers)
		{
			foreach(Neuron9 n in l.neurons)
			{
				foreach(double w in n.weights)
				{
					weightStr += w + ",";
				}
			}
		}
		return weightStr;
	}

	public void LoadWeights(string weightStr)
	{
		if(weightStr == "") return;
		string[] weightValues = weightStr.Split(',');
		int w = 0;
		foreach(Layer9 l in layers)
		{
			foreach(Neuron9 n in l.neurons)
			{
				for(int i = 0; i < n.weights.Count; i++)
				{
					n.weights[i] = System.Convert.ToDouble(weightValues[w]);
					w++;
				}
			}
		}
	}
	
	void UpdateWeights(List<double> outputs, List<double> desiredOutput)
	{
		double error;
		for(int i = numHidden; i >= 0; i--)
		{
			for(int j = 0; j < layers[i].numNeurons; j++)
			{
				if(i == numHidden)
				{
					error = desiredOutput[j] - outputs[j];
					layers[i].neurons[j].errorGradient = GetDerivativeO(outputs[j]) * error;
				}
				else
				{
					layers[i].neurons[j].errorGradient = GetDerivative(layers[i].neurons[j].output);
					double errorGradSum = 0;
					for(int p = 0; p < layers[i+1].numNeurons; p++)
					{
						errorGradSum += layers[i+1].neurons[p].errorGradient * layers[i+1].neurons[p].weights[j];
					}
					layers[i].neurons[j].errorGradient *= errorGradSum;
				}	
				for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
				{
					if(i == numHidden)
					{
						error = desiredOutput[j] - outputs[j];
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;
					}
					else
					{
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
					}
				}
				layers[i].neurons[j].bias += alpha * -1 * layers[i].neurons[j].errorGradient;
			}

		}

	}

	double ActivationFunction(double value)
	{
		return NeuralNetworkTools.GetActivationFunctionByType(value, ActivationFunctionType.TanH);
	}

	double ActivationFunctionO(double value)
	{
		return NeuralNetworkTools.GetActivationFunctionByType(value, ActivationFunctionType.TanH);
	}
	
	double GetDerivative(double value)
	{
		return NeuralNetworkTools.GetActivationFunctionDerivativeByType(value, ActivationFunctionType.TanH);
	}
	
	double GetDerivativeO(double value)
	{
		return NeuralNetworkTools.GetActivationFunctionDerivativeByType(value, ActivationFunctionType.TanH);
	}
}


