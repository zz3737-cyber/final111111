using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public bool followX = true;
    public bool followY = true;
    public float smoothSpeed = 5f;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Min Y Only")]
    public bool useMinYOnly = true;
    public float cameraMinY = 0f;

    [Header("Camera Bounds")]
    public bool useBounds = false;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 30f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = target.position + offset;

        if (!followX)
        {
            targetPos.x = currentPos.x;
        }

        if (!followY)
        {
            targetPos.y = currentPos.y;
        }

        // 只限制最低 Y，不限制最高 Y / X
        if (useMinYOnly)
        {
            targetPos.y = Mathf.Max(targetPos.y, cameraMinY);
        }

        // 如果你想完整限制相机范围，再打开 useBounds
        if (useBounds)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(
            currentPos,
            targetPos,
            smoothSpeed * Time.deltaTime
        );
    }
}