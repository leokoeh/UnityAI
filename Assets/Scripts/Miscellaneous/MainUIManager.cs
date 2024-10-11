using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    // This script manages all functionality of the Main UI

    [Header("Canvas Configuration")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas infoCanvas;

    [Header("Miscellaneous Configuration")]
    [SerializeField] private List<GameObject> arenas;
    [SerializeField] private List<Camera> cameras;
    [SerializeField] private ScenarioUIManager scenarioUIManager;

    public void SelectArena(int index)
    {
        Vector3 arenaPosition = arenas[index].transform.position;

        foreach (Camera camera in cameras)
        {
            // Move each camera to the selected Arena
            Vector3 targetPosition = new(arenaPosition.x, camera.transform.position.y, camera.transform.position.z);
            camera.transform.position = targetPosition;
        }

        // Switch canvases
        infoCanvas.enabled = true;
        mainCanvas.enabled = false;

        // Adjust Scenario UI
        scenarioUIManager.SetScenario(index);
    }

    void Update() 
    {
        // Application is exited with the Escape key
        if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

}
