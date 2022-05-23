using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
    public void ScreneLoader(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
