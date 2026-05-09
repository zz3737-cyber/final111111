using UnityEngine;
using UnityEngine.SceneManagement;

public class RotatingTailGrab : MonoBehaviour
{
    [Header("Rotate Back And Forth")]
    public Transform tailToRotate;
    public float rotateAngle = 60f;
    public float rotateSpeed = 2f;


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

    }

}