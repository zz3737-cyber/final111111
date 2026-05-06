using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHotkeyLoader : MonoBehaviour
{
    [Header("Scene Names")]
    public string scene1Name = "StartScene";
    public string scene2Name = "Jonas 1";
    public string scene3Name = "boss";

    [Header("Options")]
    public bool resetTimeScaleOnLoad = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadScene(scene1Name);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadScene(scene2Name);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadScene(scene3Name);
        }
    }

    void LoadScene(string sceneName)
    {
        if (resetTimeScaleOnLoad)
        {
            Time.timeScale = 1f;
        }

        SceneManager.LoadScene(sceneName);
    }
}