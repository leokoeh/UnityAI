using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isActive;
    public GameObject door;

    void Update()
    {
        if (isActive)
        {
            door.SetActive(true);

        } else
        {
            door.SetActive(false);
        }
    }
}