using UnityEngine;

public class ClimberBodyMotor : MonoBehaviour
{
    [Header("Body Root")]
    public Transform bodyRoot;

    [Header("Shoulder Pivots")]
    public Transform leftShoulderPivot;
    public Transform rightShoulderPivot;

    [Header("Hand Grips")]
    public HandGrip leftHandGrip;
    public HandGrip rightHandGrip;

    [Header("Arm Length Limits")]
    public float leftArmLength = 1.2f;
    public float rightArmLength = 1.2f;

    [Header("Climbing Movement")]
    public float climbMoveSpeed = 2f;

    [Header("Sliding")]
    public float slideSpeed = 1.2f;

    [Header("Ground Limit")]
    public float minY = -4f;

    void Update()
    {
        if (bodyRoot == null) return;

        bool leftGripping = IsHandGripping(leftHandGrip);
        bool rightGripping = IsHandGripping(rightHandGrip);

        Vector3 bodyMove = Vector3.zero;

        // 左手抓住时，用 WASD 拉身体
        if (leftGripping)
        {
            Vector2 input = Vector2.zero;

            if (Input.GetKey(KeyCode.W)) input += Vector2.up;
            if (Input.GetKey(KeyCode.S)) input += Vector2.down;
            if (Input.GetKey(KeyCode.A)) input += Vector2.left;
            if (Input.GetKey(KeyCode.D)) input += Vector2.right;

            if (input != Vector2.zero)
            {
                bodyMove += (Vector3)(input.normalized * climbMoveSpeed * Time.deltaTime);
            }
        }

        // 右手抓住时，用方向键拉身体
        if (rightGripping)
        {
            Vector2 input = Vector2.zero;

            if (Input.GetKey(KeyCode.UpArrow)) input += Vector2.up;
            if (Input.GetKey(KeyCode.DownArrow)) input += Vector2.down;
            if (Input.GetKey(KeyCode.LeftArrow)) input += Vector2.left;
            if (Input.GetKey(KeyCode.RightArrow)) input += Vector2.right;

            if (input != Vector2.zero)
            {
                bodyMove += (Vector3)(input.normalized * climbMoveSpeed * Time.deltaTime);
            }
        }

        // 先应用主动移动
        bodyRoot.position += bodyMove;

        // 如果两只手都没抓住，就自动下滑
        if (!leftGripping && !rightGripping)
        {
            bodyRoot.position += Vector3.down * slideSpeed * Time.deltaTime;
        }

        // 地面最低高度限制，防止一直掉下去
        ClampToGround();

        // 抓住时做手臂长度限制，防止身体飞走
        ClampToGripDistance(leftGripping, rightGripping);

        // 再做一次地面限制，防止修正后又掉到地面以下
        ClampToGround();
    }

    bool IsHandGripping(HandGrip grip)
    {
        return grip != null && grip.isGripping && grip.currentHold != null;
    }

    void ClampToGround()
    {
        Vector3 pos = bodyRoot.position;

        if (pos.y < minY)
        {
            pos.y = minY;
            bodyRoot.position = pos;
        }
    }

    void ClampToGripDistance(bool leftGripping, bool rightGripping)
    {
        // 左手抓住时：左肩到抓点的距离不能超过左手长度
        if (leftGripping && leftShoulderPivot != null)
        {
            Vector3 shoulderToHold = leftShoulderPivot.position - leftHandGrip.currentHold.position;
            float dist = shoulderToHold.magnitude;

            if (dist > leftArmLength)
            {
                Vector3 correction = shoulderToHold.normalized * (dist - leftArmLength);
                bodyRoot.position -= correction;
            }
        }

        // 右手抓住时：右肩到抓点的距离不能超过右手长度
        if (rightGripping && rightShoulderPivot != null)
        {
            Vector3 shoulderToHold = rightShoulderPivot.position - rightHandGrip.currentHold.position;
            float dist = shoulderToHold.magnitude;

            if (dist > rightArmLength)
            {
                Vector3 correction = shoulderToHold.normalized * (dist - rightArmLength);
                bodyRoot.position -= correction;
            }
        }
    }
}