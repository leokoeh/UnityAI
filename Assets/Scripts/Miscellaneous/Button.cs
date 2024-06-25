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

    // Door linked to buttons
    public Door linkedDoor;

    [Header("Miscellaneous")]

    // Storing button state
    public bool isActive;

    // Whether button can be updated or not
    public bool isLocked = false;

    public Agent lastContact;

    private void OnTriggerEnter(Collider collision)
    {
        // Change state if agent steps on button
        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent) && !isLocked)
        {
            lastContact = agent;
            isActive = true;
            linkedDoor.CheckLinks();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        // Change state if agent steps off button
        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent) && !isLocked)
        {
            lastContact = null;
            isActive = false;
            linkedDoor.CheckLinks();
        }
    }

    void Update()
    {
        if (isActive)
        {
            buttonDisplayRenderer.material = materialActive;
        }
        else
        {
            buttonDisplayRenderer.material = materialInactive;
        }
    }

    public void Reset()
    {
        isActive = false;
        isLocked = false;
        lastContact = null;
    }
}