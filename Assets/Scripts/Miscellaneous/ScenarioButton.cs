using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class ScenarioButton : MonoBehaviour
{
    [Header("Visual Configuration")]
    [SerializeField] private MeshRenderer buttonDisplayRenderer;
    [SerializeField] private Material materialActive;
    [SerializeField] private Material materialInactive;
    public bool IsActive {get; set;}
    private void OnTriggerEnter(Collider collision)
    {
        // Change state if agent steps on button
        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent)) IsActive = true;
    }

    private void OnTriggerExit(Collider collision)
    {
        // Change state if agent steps off button
        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent)) IsActive = false;
    }

    void Update()
    {
        if (IsActive) buttonDisplayRenderer.material = materialActive;
        else buttonDisplayRenderer.material = materialInactive;
    }
}
