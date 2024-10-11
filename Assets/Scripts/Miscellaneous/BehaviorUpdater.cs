using UnityEngine;
using Unity.MLAgents;

using Unity.Barracuda;
using System.Collections.Generic;

public class BehaviorUpdater : MonoBehaviour
{
    // This script can change an Agent's Neural Network Model (NNModel)

    [Header("Agent Configuration")]
    [SerializeField] private string behaviorName;
    [SerializeField] private Agent agent;

    [Header("NNModel Configuration")]
    [SerializeField] public int currentModel;
    [SerializeField] private List<NNModel> NNModels;

    public void SetModel(int modelNumber)
    {
        currentModel = modelNumber;
        if (currentModel == 0) agent.SetModel(behaviorName, null);
        else agent.SetModel(behaviorName, NNModels[currentModel - 1]);
    }
}
