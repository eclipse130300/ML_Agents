using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetworkToolkit
{
    //for Q-Learning
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
        
        public Replay(double reward, params double[] values)
        {
            states = new List<double>();
            foreach (var value in values)
            {
                states.Add(value);
            }
            
            this.reward = reward;
        }
    }
    
    public enum ActivationFunctionType
    {
        LeakyReLU,
        Sigmoid,
        Step,
        TanH
    }
    public static class NeuralNetworkTools
    {
        public static double GetActivationFunctionDerivativeByType(double value, ActivationFunctionType activationFunctionType)
        {
            switch (activationFunctionType)
            {
                case ActivationFunctionType.LeakyReLU:
                    return Derivatives.LeakyReLUDerivative(value);
                case ActivationFunctionType.Sigmoid:
                    return Derivatives.SigmoidDerivative(value);
                case ActivationFunctionType.Step:
                    return Derivatives.StepDerivative(value);
                case ActivationFunctionType.TanH:
                    return Derivatives.TanHDerivative(value);
                default:
                    return Derivatives.TanHDerivative(value);
            }
        }

        public static double GetActivationFunctionByType(double value, ActivationFunctionType activationFunctionType)
        {
            switch (activationFunctionType)
            {
                case ActivationFunctionType.LeakyReLU:
                    return ActivationFunctions.LeakyReLU(value);
                case ActivationFunctionType.Sigmoid:
                    return ActivationFunctions.Sigmoid(value);
                case ActivationFunctionType.Step:
                    return ActivationFunctions.Step(value);
                case ActivationFunctionType.TanH:
                    return ActivationFunctions.TanH(value);
                default:
                    return ActivationFunctions.TanH(value);
            }
        }
        
        //used on TOP of activation functions in Q-Learning
        public static List<double> SoftMax(List<double> values)
        {
            double max = values.Max();
            float scale = 0.0f;

            for (int i = 0; i < values.Count; i++)
            {
                scale += (float)Math.Exp(values[i] - max);
            }
        
            List<double> result = new List<double>();
            for (int i = 0; i < values.Count; i++)
                result.Add(Math.Exp(values[i] - max) / scale);
            return result;
        }
    }

    public static class Derivatives
    {
        public static double LeakyReLUDerivative(double value)
        {
            if(value > 0)
                return 1;
            else
                return 0.01;
        }
	
        public static double StepDerivative(double value)
        {
            return 0;
        }
	
        public static double TanHDerivative(double value)
        {
            return 1 - value * value;
        }
	
        public static double SigmoidDerivative(double value)
        {
            return value * (1 - value);
        } 
    }

    public static class ActivationFunctions
    {
        public static double LeakyReLU(double value)
        {
            if(value > 0)
                return value;
            else
                return 0.01 * value;
        }
	
        public static double Step(double value) //(aka binary step)
        {
            if(value < 0) return 0;
            else return 1;
        }

        public static double TanH(double value)
        {
            double k = (double) System.Math.Exp(-2*value);
            return 2 / (1.0f + k) - 1;
        }

        public static double Sigmoid(double value) 
        {
            double k = (double) System.Math.Exp(value);
            return k / (1.0f + k);
        }
    }
}