using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("Visual Configuration")]
    public MeshRenderer buttonDisplayRenderer;
    public Material materialActive;
    public Material materialInactive;

    [Header("Door Linking")]

    // List storing all linked buttons
    public List<Button> requiredButtons = new List<Button>();

    // Whether all linked buttons must be activated at the same time or not
    public bool requireSimultaneousActivation;

    // Whether door requires constant or only singular activation to remain open
    public bool requireConstantActivation;

    // Door linked to buttons
    public GameObject linkedDoor;

    [Header("Miscellaneous")]

    // Storing button state
    public bool isActive;

    private void OnTriggerEnter(Collider collision)
    {
        // Change state if agent steps on button
        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent))
        {
            isActive = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        // Change state if agent steps off button
        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent))
        {
            isActive = false;
        }
    }
    void CheckLinks()
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
            if (requireConstantActivation == false)
            {
                // Disable all linked butons if constant activation is not required
                foreach (Button button in requiredButtons)
                {
                    button.isActive = true;
                    button.gameObject.GetComponent<Collider>().enabled = false;
                }
            }

            linkedDoor.SetActive(false);
        }
        else
        {
            linkedDoor.SetActive(true);
        }

    }

    void Update()
    {
        // Check links and update button materials
        CheckLinks();

        if (isActive)
        {
            buttonDisplayRenderer.material = materialActive;
        }
        else
        {
            buttonDisplayRenderer.material = materialInactive;
        }
    }
}