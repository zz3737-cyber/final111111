using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pickup UI")]
    public TextMeshProUGUI pickupText;

    [Header("Pause UI")]
    public GameObject pausePanel;
    public PauseButton[] pauseButtons;

    [Header("Scene")]
    public string startSceneName = "StartScene";

    [Header("Start Activate Objects")]
    public TimedObject[] startObjects;

    private bool isPaused = false;
    private int selectedButtonIndex = 0;
    private bool stickCanMove = true;
    private Coroutine textCoroutine;

    [System.Serializable]
    public class TimedObject
    {
        public GameObject obj;
        public float activeDuration = 2f;
    }

    [System.Serializable]
    public class PauseButton
    {
        public GameObject highlightImage;
        public ButtonType buttonType;
    }

    public enum ButtonType
    {
        Back,
        Exit
    }

    private void Awake()
    {
        instance = this;

        if (pickupText != null)
            pickupText.gameObject.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(ActivateObjectsOneByOne());
    }

    private void Update()
    {
        if (Gamepad.current == null) return;

        if (Gamepad.current.startButton.wasPressedThisFrame)
        {
            TogglePause();
        }

        if (isPaused)
        {
            HandlePauseMenuInput();
        }
    }

    private void HandlePauseMenuInput()
    {
        // Press A to confirm selected button
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            PressSelectedButton();
        }

        Vector2 leftStick = Gamepad.current.leftStick.ReadValue();
        Vector2 rightStick = Gamepad.current.rightStick.ReadValue();

        float verticalInput = Mathf.Abs(leftStick.y) > Mathf.Abs(rightStick.y) 
            ? leftStick.y 
            : rightStick.y;

        if (Mathf.Abs(verticalInput) < 0.3f)
        {
            stickCanMove = true;
            return;
        }

        if (stickCanMove)
        {
            if (verticalInput > 0.3f)
                ChangeSelectedButton(-1);
            else if (verticalInput < -0.3f)
                ChangeSelectedButton(1);

            stickCanMove = false;
        }
    }

    private void ChangeSelectedButton(int direction)
    {
        selectedButtonIndex += direction;

        if (selectedButtonIndex < 0)
            selectedButtonIndex = pauseButtons.Length - 1;

        if (selectedButtonIndex >= pauseButtons.Length)
            selectedButtonIndex = 0;

        UpdateButtonHighlights();
    }

    private void UpdateButtonHighlights()
    {
        for (int i = 0; i < pauseButtons.Length; i++)
        {
            if (pauseButtons[i].highlightImage != null)
                pauseButtons[i].highlightImage.SetActive(i == selectedButtonIndex);
        }
    }

    private void PressSelectedButton()
    {
        if (pauseButtons.Length == 0) return;

        switch (pauseButtons[selectedButtonIndex].buttonType)
        {
            case ButtonType.Back:
                BackToGame();
                break;

            case ButtonType.Exit:
                ExitToStartScene();
                break;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            selectedButtonIndex = 0;
            UpdateButtonHighlights();
        }
    }

    public void BackToGame()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void ExitToStartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);
    }

    public void ShowText(string message)
    {
        if (pickupText == null) return;

        if (textCoroutine != null)
            StopCoroutine(textCoroutine);

        pickupText.text = message;
        pickupText.gameObject.SetActive(true);
        textCoroutine = StartCoroutine(HideText());
    }

    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(3f);
        pickupText.gameObject.SetActive(false);
    }

    private IEnumerator ActivateObjectsOneByOne()
    {
        foreach (TimedObject item in startObjects)
        {
            if (item.obj == null) continue;

            item.obj.SetActive(true);
            yield return new WaitForSeconds(item.activeDuration);
            item.obj.SetActive(false);
        }
    }
}