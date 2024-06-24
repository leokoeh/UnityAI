using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class Door : MonoBehaviour
{
    [Header("Door Configuration")]
    public GameObject door;
    public bool isActive;

    // Currently non-functional reward system

    // [Header("Reward Configuration")]
    // Reward that will be given to each agent that stepped on a linked button when the door opens
    // public int doorReward;
    // Agents that already collected the door reward
    // public List<Agent> blacklist = new List<Agent>();

    [Header("Button Configuration")]
    // List storing all linked buttons
    public List<Button> requiredButtons = new List<Button>();

    // Whether all linked buttons must be activated at the same time or not
    public bool requireSimultaneousActivation;

    // Whether door requires constant or only singular activation to remain open
    public bool requireConstantActivation;

    public void CheckLinks()
    {
        // Function to check linked buttons
        bool doorOpen;

        if (requireSimultaneousActivation)
        {
            // Set door state to open, if single button is not activated, set state to closed
            doorOpen = true;

            foreach (Button button in requiredButtons)
            {
                if (button.isActive == false)
                {
                    doorOpen = false;
                    break;
                }
            }
        }
        else
        {
            // Set door state to closed, if single button is activated, set state to open
            doorOpen = false;

            foreach (Button button in requiredButtons)
            {
                if (button.isActive == true)
                {
                    doorOpen = true;
                    break;
                }
            }
        }

        if (doorOpen)
        {
            foreach (Button button in requiredButtons)
            {
                // ISSUE: Function is called multiple times in a very short time frame and thus leads to the blacklist not being able to keep up.

                /*
                Agent agent = button.lastContact;

                // Check if agent is eligible for reward
                if (!blacklist.Contains(agent))
                {
                    blacklist.Add(agent);
                    Debug.LogWarning(blacklist[0]);
                    agent.AddReward(doorReward);
                }
                else
                {
                    Debug.Log(agent + " is already in the blacklist.");

                }*/

                if (requireConstantActivation == false)
                {
                    // Disable all linked butons if constant activation is not required
                    button.isActive = true;
                    button.gameObject.GetComponent<Collider>().enabled = false;

                }


            } 
            isActive = false;
        }
        else
        {
            isActive = true;
        }

        if (isActive)
        {
            door.SetActive(true);

        }
        else
        {
            door.SetActive(false);
        }
    }
}