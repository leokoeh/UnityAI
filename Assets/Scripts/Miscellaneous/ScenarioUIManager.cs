using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScenarioUIManager : MonoBehaviour
{
    // This script manages all functionality of the Scenario UI

    [Header("Canvas Configuration")]
    [SerializeField] private Canvas infoCanvas;
    [SerializeField] private Canvas mainCanvas;

    [Header("GUI Configuration")]
    [SerializeField] private TMP_Dropdown arenaStageDropdown;
    [SerializeField] private TMP_Dropdown selectableAgentsDropdown;
    [SerializeField] private TMP_Dropdown cameraSettingsDropdown;
    [SerializeField] private TextMeshProUGUI scenarioText;

    [Header("Miscellaneous Configuration")]
    [SerializeField] private List<ArenaManager> arenaManagers;
    [SerializeField] private List<string> scenarioNames;
    [SerializeField] private List<Camera> cameras;
    private ArenaManager currentArena;

    void Start()
    {
        SetScenario(0);
    }

    public void SetArenaStage(int index)
    {
        // Arena stages begin with 1
        currentArena.SetArenaStage(index + 1);
    }

    public void SetScenario(int index)
    {
        // Adjusts UI based on currently selected Arena
        currentArena = arenaManagers[index];
        UpdateAgentsDropdown();
        SetScenarioText(index);
    }

    public void SetBehavior(int index)
    {
        // Start counting at 1 since first element of selectable Agents dropdown (index 0) selects no Agent
        int counter = 1;

        foreach (BehaviorUpdater behaviorUpdater in currentArena.BehaviorUpdaters)
        {
            // Determine selected Agent, this Agent's model is set to 0 which means manually controlled
            if (index == counter) behaviorUpdater.SetModel(0);

            // Other Agents are assigned Model based on the current Arena stage
            else behaviorUpdater.SetModel(currentArena.currentStage);

            counter++;
        }
    }

    void UpdateAgentsDropdown()
    {
        // Reset dropdown
        selectableAgentsDropdown.ClearOptions();

        // First element of the dropdown is always "Keiner"
        List<TMP_Dropdown.OptionData> options = new()
        {
            new TMP_Dropdown.OptionData("Keiner")
        };

        foreach (BehaviorUpdater behaviorUpdater in currentArena.BehaviorUpdaters)
        {
            // Add option for each Behavior Updater (Agent) in the Scenario
            options.Add(new TMP_Dropdown.OptionData(behaviorUpdater.name));
        }

        // Assign list to dropdown
        selectableAgentsDropdown.AddOptions(options);

    }

    void SetScenarioText(int index)
    {
        scenarioText.text = scenarioNames[index];
    }

    public void ReturnToMain()
    {
        // Reset all customizations made by player to default
        SetArenaStage(0);
        SetBehavior(0);
        SetCamera(0);

        // Reset all dropdowns to default (0)
        arenaStageDropdown.value = 0;
        cameraSettingsDropdown.value = 0;

        // Switch canvases
        mainCanvas.enabled = true;
        infoCanvas.enabled = false;
    }

    public void SetCamera(int index)
    {
        // Deactivate all cameras
        foreach (Camera camera in cameras) camera.gameObject.SetActive(false);

        // Reactivate selected camera
        cameras[index].gameObject.SetActive(true);
    }
}
