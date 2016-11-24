using UnityEngine;
using System.Collections;

/*********

    Neural Network Code based on code from AI for Game Developers (2004)

    ******** */

public class NeuralNetworkLayer {

    public int NumberOfNodes;
    public int NumberOfChildNodes;
    public int NumberOfParentNodes;
    public float[,] Weights;
    public float[,] WeightChanges;
    public float[] NeuronValues;
    public float[] DesiredValues;
    public float[] Errors;
    public float[] BiasWeights;
    public float[] BiasValues;
    public float LearningRate;
    public bool LinearOutput;
    public bool UseMomentum;
    public float MomentumFactor;

    public NeuralNetworkLayer ParentLayer;
    public NeuralNetworkLayer ChildLayer;

    public NeuralNetworkLayer()
    {
        ParentLayer = null;
        ChildLayer = null;
        LinearOutput = false;
        UseMomentum = false;
        MomentumFactor = 0.9f;
    }

    public void SetLayers(NeuralNetworkLayer[] ParentChild)
    {
        if (ParentChild[0] != null)
        {
            ParentLayer = ParentChild[0];
        }

        if (ParentChild[1] != null)
        {
            ChildLayer = ParentChild[1];
        }
    }

    public void Initialize(int NumNodes, NeuralNetworkLayer[] ParentChild)
    {
        //Allocate everything
        NeuronValues = new float[NumberOfNodes];
        DesiredValues = new float[NumberOfNodes];
        Errors = new float[NumberOfNodes];

        if (ParentChild[0] != null)
        {
            ParentLayer = ParentChild[0];
        }

        if (ParentChild[1] != null)
        {
            ChildLayer = ParentChild[1];

            Weights = new float[NumberOfNodes, NumberOfChildNodes];
            WeightChanges = new float[NumberOfNodes, NumberOfChildNodes];

            BiasValues = new float[NumberOfChildNodes];
            BiasWeights = new float[NumberOfChildNodes];

        }
        else
        {
            Weights = null;
            BiasValues = null;
            BiasWeights = null;
            WeightChanges = null;
        }
        //Make sure everything contains 0s
        for (int i = 0; i < NumberOfNodes; i++)
        {
            NeuronValues[i] = 0;
            DesiredValues[i] = 0;
            Errors[i] = 0;

            if (ChildLayer != null)
            {
                for (int j = 0; j < NumberOfChildNodes; j++)
                {
                    Weights[i,j] = 0;
                    WeightChanges[i,j] = 0;
                }
            }
        }
        //Init the bias values and weights
        if (ChildLayer != null)
        {
            for (int j = 0; j < NumberOfChildNodes; j++)
            {
                BiasValues[j] = -1;
                BiasWeights[j] = 0;
            }
        }
    }

    public void RandomizeWeights()
    {
        for (int i = 0; i < NumberOfNodes; i++)
        {
            for (int j = 0; j < NumberOfChildNodes; j++)
            {
                Weights[i,j] = Random.Range(-1.00f, 1.00f);
                BiasWeights[j] = Random.Range(-1.00f, 1.00f);
            }
        }
    }

    public void CalculateErrors()
    {
        float sum = 0;
        if (ChildLayer == null)
        { // Output Layer
            for (int i = 0; i < NumberOfNodes; i++)
            {
                Errors[i] = (DesiredValues[i] - NeuronValues[i]) * NeuronValues[i] * (1 - NeuronValues[i]);
            }
        }
        else if (ParentLayer == null)
        { // Input Layer
            for (int i = 0; i < NumberOfNodes; i++)
            {
                Errors[i] = 0;
            }
        }
        else
        { // Hidden Layer
            for (int i = 0; i < NumberOfNodes; i++)
            {
                sum = 0;
                for (int j = 0; j < NumberOfChildNodes; j++)
                {
                    sum += ChildLayer.Errors[j] * Weights[i,j];
                }
                Errors[i] = sum * NeuronValues[i] * (1 - NeuronValues[i]);
            }
        }
    }

    public void AdjustWeights()
    {
        float dw = 0;

        if (ChildLayer != null)
        {
            for (int i = 0; i < NumberOfNodes; i++)
            {
                for (int j = 0; j < NumberOfChildNodes; j++)
                {
                    dw = LearningRate * ChildLayer.Errors[j] * NeuronValues[i];
                    if (UseMomentum)
                    {
                        Weights[i,j] += dw + MomentumFactor * WeightChanges[i,j];
                        WeightChanges[i,j] = dw;
                    }
                    else
                    {
                        Weights[i,j] += dw;
                    }
                }
            }
            for (int j = 0; j < NumberOfChildNodes; j++)
            {
                BiasWeights[j] += LearningRate * ChildLayer.Errors[j] * BiasValues[j];
            }
        }
    }

    public void CalculateNeuronValues()
    {
        if (ParentLayer != null)
        {
            for (int i = 0; i < NumberOfNodes; i++)
            {
                float x = 0;
                for (int j = 0; j < NumberOfParentNodes; j++)
                {
                    x += ParentLayer.NeuronValues[j] * ParentLayer.Weights[j,i];
                }
                x += ParentLayer.BiasValues[i] * ParentLayer.BiasWeights[i];
                if ((ChildLayer == null) && LinearOutput)
                {
                    NeuronValues[i] = x;
                }
                else
                {
                    NeuronValues[i] = 1 / (1 + Mathf.Exp(-x));
                }
            }
        }
    }
}
