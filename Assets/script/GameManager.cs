using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pickup UI")]
    public TextMeshProUGUI pickupText;

    [Header("Pause UI")]
    public GameObject pausePanel;

    private bool isPaused = false;

    private void Awake()
    {
        instance = this;

        if (pickupText != null)
            pickupText.gameObject.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void ShowText(string message)
    {
        if (pickupText == null) return;

        StopAllCoroutines();
        pickupText.text = message;
        pickupText.gameObject.SetActive(true);
        StartCoroutine(HideText());
    }

    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(3f);
        pickupText.gameObject.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }
}