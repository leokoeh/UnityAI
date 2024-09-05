using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartWall : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject doorway;

    public void SetMode(int mode)
    {
        switch (mode) {
            case 0:
                wall.SetActive(true);
                doorway.SetActive(false);
                break;

            case 1:
                wall.SetActive(false);
                doorway.SetActive(true);
                break;
        }
    }
}