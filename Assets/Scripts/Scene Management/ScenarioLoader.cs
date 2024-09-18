using UnityEngine;
using UnityEngine.SceneManagement;
public class ScenarioLoader : MonoBehaviour
{
    public void OpenScene(string name) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }
}
