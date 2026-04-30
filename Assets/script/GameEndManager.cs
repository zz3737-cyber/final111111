using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance;

    [Header("End UI")]
    public GameObject gameEndPanel;

    [Header("Settings")]
    public bool pauseGameOnEnd = true;

    private bool gameEnded = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(false);
        }
    }

    public void EndGame()
    {
        if (gameEnded) return;

        gameEnded = true;

        Debug.Log("Game End!");

        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(true);
        }

        if (pauseGameOnEnd)
        {
            Time.timeScale = 0f;
        }
    }
}