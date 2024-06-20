using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("Visual Config")]
    public MeshRenderer buttonDisplayRenderer;
    public Material materialActive;
    public Material materialInactive;

    [Header("Door Linking")]
    public List<Button> requiredButtons = new List<Button>();
    public bool requireSimultaneousActivation;
    public bool requireSingularActivation;
    public GameObject linkedDoor;

    [Header("Misc")]
    public bool isActive;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent))
        {

            isActive = true;

        }

    }

    private void OnTriggerExit(Collider collision)
    {

        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent))
        {

            isActive = false;

        }

    }

    void CheckLinks()
    {
        bool doorOpen;

        if (requireSimultaneousActivation)
        {
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
            if (requireSingularActivation)
            {

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
