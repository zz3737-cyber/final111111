using UnityEngine;

public class KeepPointOutOfForbiddenZone : MonoBehaviour
{
    [Header("Forbidden Zones")]
    public Collider2D[] forbiddenZones;

    [Header("Settings")]
    public float pushOutPadding = 0.05f;

    private Vector3 lastSafePosition;

    void Start()
    {
        lastSafePosition = transform.position;
    }

    void LateUpdate()
    {
        if (forbiddenZones == null || forbiddenZones.Length == 0)
        {
            lastSafePosition = transform.position;
            return;
        }

        bool insideForbiddenZone = false;

        foreach (Collider2D zone in forbiddenZones)
        {
            if (zone == null) continue;

            // 如果当前点在禁区内部，ClosestPoint 会返回点自己
            Vector2 currentPos = transform.position;
            Vector2 closest = zone.ClosestPoint(currentPos);

            float distance = Vector2.Distance(currentPos, closest);

            if (distance < 0.0001f && zone.OverlapPoint(currentPos))
            {
                insideForbiddenZone = true;
                break;
            }
        }

        if (insideForbiddenZone)
        {
            transform.position = lastSafePosition;
        }
        else
        {
            lastSafePosition = transform.position;
        }
    }
}