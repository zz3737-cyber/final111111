using UnityEngine;
using UnityEngine.SceneManagement;

public class RotatingTailGrab : MonoBehaviour
{
    [Header("Rotate Back And Forth")]
    public Transform tailToRotate;
    public float rotateAngle = 60f;
    public float rotateSpeed = 2f;

    [Header("Stay Collision")]
    public float stayDuration = 3f;
    public string sceneToLoad = "GameOverScene";

    private float stayTimer = 0f;
    private bool isTouching = false;
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

        if (isTouching)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= stayDuration)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            stayTimer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        isTouching = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isTouching = false;
        stayTimer = 0f;
    }
}