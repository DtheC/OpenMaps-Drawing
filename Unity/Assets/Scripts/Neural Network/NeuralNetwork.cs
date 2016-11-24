using UnityEngine;
using System.Collections;

/*********

    Neural Network Code based on code from AI for Game Developers (2004)

    ******** */

public class NeuralNetwork
{
    public NeuralNetworkLayer InputLayer;
    public NeuralNetworkLayer HiddenLayer;
    public NeuralNetworkLayer OutputLayer;

    public NeuralNetwork()
    {
    }

    public void Initialize(int nNodesInput, int nNodesHidden, int nNodesOutput)
    {
        InputLayer = new NeuralNetworkLayer();
        HiddenLayer = new NeuralNetworkLayer();
        OutputLayer = new NeuralNetworkLayer();

        InputLayer.NumberOfNodes = nNodesInput;
        InputLayer.NumberOfChildNodes = nNodesHidden;
        InputLayer.NumberOfParentNodes = 0;
        NeuralNetworkLayer[] n = new NeuralNetworkLayer[2];
        n[0] = null;
        n[1] = HiddenLayer;
        InputLayer.Initialize(nNodesInput, n);
        InputLayer.RandomizeWeights();

        HiddenLayer.NumberOfNodes = nNodesHidden;
        HiddenLayer.NumberOfChildNodes = nNodesOutput;
        HiddenLayer.NumberOfParentNodes = nNodesInput;
        n = new NeuralNetworkLayer[2];
        n[0] = InputLayer;
        n[1] = OutputLayer;
        HiddenLayer.Initialize(nNodesHidden, n);
        HiddenLayer.RandomizeWeights();

        OutputLayer.NumberOfNodes = nNodesOutput;
        OutputLayer.NumberOfChildNodes = 0;
        OutputLayer.NumberOfParentNodes = nNodesHidden;
        n = new NeuralNetworkLayer[2];
        n[0] = HiddenLayer;
        n[1] = null;
        OutputLayer.Initialize(nNodesOutput, n);
    }

    public void SetInput(int i, float value)
    {
        if ((i >= 0) && (i < InputLayer.NumberOfNodes))
        {
            InputLayer.NeuronValues[i] = value;
        }
    }

    public float GetOutput(int i)
    {
        if ((i >= 0) && (i < OutputLayer.NumberOfNodes))
        {
            return OutputLayer.NeuronValues[i];
        }
        return 2147483647;
    }

    void SetDesiredOutput(int i, float value)
    {
        if ((i >= 0) && (i < OutputLayer.NumberOfNodes))
        {
            OutputLayer.DesiredValues[i] = value;
        }
    }

    public void FeedForward()
    {
        InputLayer.CalculateNeuronValues();
        HiddenLayer.CalculateNeuronValues();
        OutputLayer.CalculateNeuronValues();
    }

    public void BackPropagate()
    {
        OutputLayer.CalculateErrors();
        HiddenLayer.CalculateErrors();
        HiddenLayer.AdjustWeights();
        InputLayer.AdjustWeights();
    }

    public int GetMaxOutputID()
    {
        float maxval = OutputLayer.NeuronValues[0];
        int id = 0;
        for (int i = 1; i < OutputLayer.NumberOfNodes; i++)
        {
            if (OutputLayer.NeuronValues[i] > maxval)
            {
                maxval = OutputLayer.NeuronValues[i];
                id = i;
            }
        }
        return id;
    }

    public float CalculateError()
    {
        float error = 0;
        for (int i = 0; i < OutputLayer.NumberOfNodes; i++)
        {                     //I DON'T KNOW IF THIS IS RIGHT OR NOT
            error += Mathf.Pow(OutputLayer.NeuronValues[i] - -OutputLayer.DesiredValues[i], 2);
        }
        error = error / OutputLayer.NumberOfNodes;
        return error;
    }

    public void SetLearningRate(float rate)
    {
        InputLayer.LearningRate = rate;
        HiddenLayer.LearningRate = rate;
        OutputLayer.LearningRate = rate;
    }

    public void SetLinearOutput(bool useLinear)
    {
        InputLayer.LinearOutput = useLinear;
        HiddenLayer.LinearOutput = useLinear;
        OutputLayer.LinearOutput = useLinear;
    }

    public void SetMomentum(bool useMomentum, float factor)
    {
        InputLayer.UseMomentum = useMomentum;
        HiddenLayer.UseMomentum = useMomentum;
        OutputLayer.UseMomentum = useMomentum;
        InputLayer.MomentumFactor = factor;
        HiddenLayer.MomentumFactor = factor;
        OutputLayer.MomentumFactor = factor;
    }

    /*
    void DumpData(String filename)
    {
        StringList s = new StringList();

        s.append("-----------------------------------\n");
        s.append("Input Layer\n\n");
        s.append("Node Values:\n");
        s.append("\n");
        for (int i = 0; i < InputLayer.NumberOfNodes; i++)
            s.append("(" + i + ") = " + InputLayer.NeuronValues[i] + "\n");
        s.append("\n");
        s.append("Weights:\n");
        s.append("\n");
        for (int i = 0; i < InputLayer.NumberOfNodes; i++)
            for (int j = 0; j < InputLayer.NumberOfChildNodes; j++)
                s.append("(" + i + ", " + j + ") = " + InputLayer.Weights[i][j] + "\n");
        s.append("\n");
        s.append("Bias Weights:\n");
        s.append("\n");
        for (int j = 0; j < InputLayer.NumberOfChildNodes; j++)
            s.append("(" + j + ") = " + InputLayer.BiasWeights[j] + "\n");
        s.append("\n");
        s.append("\n");
        s.append("---------------------------------------------\n");
        s.append("Hidden Layer\n");
        s.append("---------------------------------------------\n");
        s.append("\n");
        s.append("Weights:\n");
        s.append("\n");
        for (int i = 0; i < HiddenLayer.NumberOfNodes; i++)
            for (int j = 0; j < HiddenLayer.NumberOfChildNodes; j++)
                s.append("(" + i + ", " + j + ") = " + HiddenLayer.Weights[i][j] + "\n");
        s.append("\n");
        s.append("Bias Weights:\n");
        s.append("\n");
        for (int j = 0; j < HiddenLayer.NumberOfChildNodes; j++)
            s.append("(" + j + ") = " + HiddenLayer.BiasWeights[j] + "\n");
        s.append("\n");
        s.append("\n");
        s.append("---------------------------------------------\n");
        s.append("Output Layer\n");
        s.append("---------------------------------------------\n");
        s.append("\n");
        s.append("Node Values:\n");
        s.append("\n");
        for (int i = 0; i < OutputLayer.NumberOfNodes; i++)
            s.append("(" + i + ") = " + OutputLayer.NeuronValues[i] + "\n");
        s.append("\n");
        String[] str = s.array();

        saveStrings(filename, str);
    }
    */

}
