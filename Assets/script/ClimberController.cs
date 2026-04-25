using UnityEngine;

public class ClimberController : MonoBehaviour
{
    [Header("Limb End Transforms")]
    public Transform leftHandEnd;
    public Transform leftFootEnd;
    public Transform rightHandEnd;
    public Transform rightFootEnd;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float maxLimbDistanceFromBody = 2f;

    [Header("Body Reference")]
    public Transform bodyCenter;

    void Update()
    {
        float step = moveSpeed * Time.deltaTime;

        MoveLeftHand(step);
        MoveLeftFoot(step);
        MoveRightHand(step);
        MoveRightFoot(step);

        ClampLimb(leftHandEnd);
        ClampLimb(leftFootEnd);
        ClampLimb(rightHandEnd);
        ClampLimb(rightFootEnd);
    }

    void MoveLeftHand(float step)
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += Vector3.up;
        if (Input.GetKey(KeyCode.S)) move += Vector3.down;
        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        leftHandEnd.position += move.normalized * step;
    }

    void MoveLeftFoot(float step)
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.I)) move += Vector3.up;
        if (Input.GetKey(KeyCode.K)) move += Vector3.down;
        if (Input.GetKey(KeyCode.J)) move += Vector3.left;
        if (Input.GetKey(KeyCode.L)) move += Vector3.right;

        leftFootEnd.position += move.normalized * step;
    }

    void MoveRightHand(float step)
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow)) move += Vector3.up;
        if (Input.GetKey(KeyCode.DownArrow)) move += Vector3.down;
        if (Input.GetKey(KeyCode.LeftArrow)) move += Vector3.left;
        if (Input.GetKey(KeyCode.RightArrow)) move += Vector3.right;

        rightHandEnd.position += move.normalized * step;
    }

    void MoveRightFoot(float step)
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.Keypad5)) move += Vector3.up;
        if (Input.GetKey(KeyCode.Keypad2)) move += Vector3.down;
        if (Input.GetKey(KeyCode.Keypad1)) move += Vector3.left;
        if (Input.GetKey(KeyCode.Keypad3)) move += Vector3.right;

        rightFootEnd.position += move.normalized * step;
    }

    void ClampLimb(Transform limb)
    {
        Vector3 offset = limb.position - bodyCenter.position;

        if (offset.magnitude > maxLimbDistanceFromBody)
        {
            limb.position = bodyCenter.position + offset.normalized * maxLimbDistanceFromBody;
        }
    }
}