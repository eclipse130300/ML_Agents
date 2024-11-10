using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain7 : MonoBehaviour {

	ANN7 ann7;
	double sumSquareError = 0;

	void Start () {
		ann7 = new ANN7(2, 1, 1, 2, 0.9);
		
		List<double> result;
		
		for(int i = 0; i < 200000; i++)
		{
			sumSquareError = 0;
			result = Train(1, 1, 0);
			sumSquareError += Mathf.Pow((float)result[0] - 0,2);
			result = Train(1, 0, 1);
			sumSquareError += Mathf.Pow((float)result[0] - 1,2);
			result = Train(0, 1, 1);
			sumSquareError += Mathf.Pow((float)result[0] - 1,2);
			result = Train(0, 0, 0);
			sumSquareError += Mathf.Pow((float)result[0] - 0,2);
		}
		Debug.Log("SSE: " + sumSquareError);

		result = Train(1, 1, 0);
		Debug.Log(" 1 1 " + result[0]);
		result = Train(1, 0, 1);
		Debug.Log(" 1 0 " + result[0]);
		result = Train(0, 1, 1);
		Debug.Log(" 0 1 " + result[0]);
		result = Train(0, 0, 0);
		Debug.Log(" 0 0 " + result[0]);
	}

	List<double> Train(double i1, double i2, double o)
	{
		List<double> inputs = new List<double>();
		List<double> outputs = new List<double>();
		inputs.Add(i1);
		inputs.Add(i2);
		outputs.Add(o);
		return (ann7.Go(inputs,outputs));
	}
}
