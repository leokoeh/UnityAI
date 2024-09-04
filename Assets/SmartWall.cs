using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartWall : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Doorway;
    private int Mode;

    public void SetMode(int mode)
    {
        Mode = mode;

        if (Mode == 0)
        {
            Wall.SetActive(true);
            Doorway.SetActive(false);
        }else
        {
            Wall.SetActive(false);
            Doorway.SetActive(true);
        }
    }
}
