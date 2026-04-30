using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RotatingTailGrab : MonoBehaviour
{
    [Header("Rotate Back And Forth")]
    public Transform tailToRotate;
    public float rotateAngle = 60f;
    public float rotateSpeed = 2f;

    [Header("Grab")]
    public float grabDuration = 2f;
    public string sceneToLoad = "GameOverScene";

    private bool playerInRange = false;
    private float grabTimer = 0f;
    private Quaternion tailStartLocalRotation;

    void Start()
    {
        if (tailToRotate != null)
            tailStartLocalRotation = tailToRotate.localRotation;
    }

    void Update()
    {
        if (tailToRotate != null)
        {
            float angle = Mathf.Sin(Time.time * rotateSpeed) * rotateAngle;
            tailToRotate.localRotation = tailStartLocalRotation * Quaternion.Euler(0f, 0f, angle);
        }

        if (!playerInRange)
        {
            grabTimer = 0f;
            return;
        }

        if (Gamepad.current != null && Gamepad.current.leftTrigger.isPressed)
        {
            grabTimer += Time.deltaTime;

            if (grabTimer >= grabDuration)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            grabTimer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            grabTimer = 0f;
        }
    }
}