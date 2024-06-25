using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class Reward : MonoBehaviour
{
    [Header("Reward Configuration")]
    // Amount of reward added when collected by an agent
    public float rewardAmount;

    // If agent's epiosde is ended if reward is collected
    public bool endAgentEpisode;

    // If reward gets disabled after collection
    public bool singleCollection;

    [Header("Resetter Configuration")]
    // Ability to link scenario resetter to reward
    public ScenarioResetter linkedResetter;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent))
        {
            // Add reward
            agent.AddReward(rewardAmount);

            if (endAgentEpisode)
            {
                // End agent's epiosde
                agent.EndEpisode();
            }

            if (singleCollection)
            {
                // Disable the game object
                transform.gameObject.SetActive(false);
            }

            if (linkedResetter)
            {
                linkedResetter.Reset();
            }
        }
    }
}