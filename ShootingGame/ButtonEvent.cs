using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
    public void ScreneLoader(string sceneName) // using UnityEngine
    {
        SceneManager.LoadScene(sceneName);
    }
}
