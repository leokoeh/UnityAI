using System.Collections;
using Unity.MLAgents;
using UnityEngine;

public class StuckReset : MonoBehaviour
{
    // This script resets Agents if they get stuck

    [Header("Agent Configuration")]
    [SerializeField] Transform agentTransform;
    [SerializeField] Agent agent;
    [SerializeField] BehaviorUpdater behaviorUpdater;

    [Header("Respawn Configuration")]
    [SerializeField] float checkInterval;
    [SerializeField] float checksAmount;
    [SerializeField] float minDistance;

    IEnumerator CheckStuck()
    {
        while (true)
        {
            float totalDistance = 0;

            for (int i = 0; i < checksAmount; i++)
            {
                // Store old position, wait and then add distance to total distance
                Vector3 oldPosition = agentTransform.position;
                yield return new WaitForSeconds(checkInterval);
                totalDistance += Vector3.Distance(oldPosition, agentTransform.position);
            }

            // Reset if agent is considered stuck and is not controlled by the Player
            if (totalDistance < minDistance && behaviorUpdater.currentModel != 0) agent.EndEpisode();
        }
    }

    void Start()
    {
        StartCoroutine(CheckStuck());
    }
}
