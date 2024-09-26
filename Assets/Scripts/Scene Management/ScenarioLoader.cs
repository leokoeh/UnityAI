using UnityEngine;
using UnityEngine.SceneManagement;
public class ScenarioLoader : MonoBehaviour
{
    public void OpenScene(string name) {
        // Open a unity scene based on the provided string
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }
}
