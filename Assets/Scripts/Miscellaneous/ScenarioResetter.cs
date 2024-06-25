using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class ScenarioResetter : MonoBehaviour
{
    // List of all doors in a scenario
    public List<Door> doors = new List<Door>();

    // List of all agents in a scenario
    public List<Agent> agents = new List<Agent>();

    public List<Reward> rewards = new List<Reward>();

    // Stores the agents' respawn positions
    private List<Vector3> respawnPositions = new List<Vector3>();

    void Start()
    {
        foreach (Agent agent in agents)
        {
            respawnPositions.Add(agent.transform.position);
        }
    }

    public void Reset()
    {
        foreach (Agent agent in agents)
        {
            agent.transform.position = respawnPositions[agents.IndexOf(agent)];
            agent.EndEpisode();
        }

        foreach (Door door in doors)
        {
            door.Reset();
        }

        foreach (Reward reward in rewards)
        {
            reward.transform.gameObject.SetActive(true);
        }
    }
}
