using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneBean : MonoBehaviour
{
    [Header("Scene")]
    public string endSceneName = "EndScene";

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        PlayerFruitReceiver receiver = other.GetComponent<PlayerFruitReceiver>();

        if (receiver == null)
        {
            receiver = other.GetComponentInParent<PlayerFruitReceiver>();
        }

        if (receiver == null) return;

        triggered = true;

        Time.timeScale = 1f;
        SceneManager.LoadScene(endSceneName);
    }
}