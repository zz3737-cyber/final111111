using UnityEngine;

public class FootPlant : MonoBehaviour
{
    [Header("State")]
    public bool isPlanted = false;
    public Transform currentFootHold;

    [Header("References")]
    public Transform hipPivot;

    [Header("Settings")]
    public float maxLegStretch = 1.8f;
    public float detachInputThreshold = 0.85f;
    public float minLockTime = 0.25f;

    [Header("External Input")]
    public bool useExternalInput = false;
    public Vector2 externalInput;

    [Header("Keyboard Input")]
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    [Header("Debug")]
    public bool debugLog = false;

    private Transform candidateFootHold;
    private Collider2D candidateFootCollider;

    private float plantedTime = -999f;

    private enum FootHoldType
    {
        Long,
        Slippery
    }

    private FootHoldType candidateHoldType = FootHoldType.Long;
    private FootHoldType currentHoldType = FootHoldType.Long;

    private Collider2D currentHoldCollider;
    private Vector3 localFootPoint;

    void Update()
    {
        Vector2 inputDir = GetInputDirection();

        // 还没踩住时，碰到候选长点 / 滑点就自动吸附
        if (candidateFootHold != null && candidateFootCollider != null && !isPlanted)
        {
            PlantFoot();
        }

        if (isPlanted && currentFootHold != null)
        {
            UpdatePlantedPosition();

            CheckLegStretch();

            CheckInputDetach(inputDir);
        }
    }

    void PlantFoot()
    {
        isPlanted = true;

        currentFootHold = candidateFootHold;
        currentHoldCollider = candidateFootCollider;
        currentHoldType = candidateHoldType;
        plantedTime = Time.time;

        // 关键：用实际碰到的 Collider2D 算最近表面点
        // 这样 PolygonCollider2D / BoxCollider2D 都能用
        Vector3 closestWorldPoint = currentHoldCollider.ClosestPoint(transform.position);

        // 记录在 holdRoot 的局部坐标里
        // 如果 Tag 在父物体上，Collider 在子物体上，也能跟随父物体
        localFootPoint = currentFootHold.InverseTransformPoint(closestWorldPoint);

        if (debugLog)
        {
            Debug.Log(
                $"{name} planted on {currentFootHold.name}, " +
                $"type = {currentHoldType}, " +
                $"collider = {currentHoldCollider.GetType().Name}, " +
                $"worldPoint = {closestWorldPoint}"
            );
        }
    }

    void UpdatePlantedPosition()
    {
        if (currentHoldCollider == null)
        {
            ReleaseFoot();
            return;
        }

        // 长点 / 滑点：脚固定在实际踩到的位置，不吸到中心
        transform.position = currentFootHold.TransformPoint(localFootPoint);
    }

    void CheckLegStretch()
    {
        if (hipPivot == null) return;

        float dist = Vector2.Distance(hipPivot.position, transform.position);

        if (dist > maxLegStretch)
        {
            if (debugLog)
            {
                Debug.Log($"{name} released because leg stretched too far. Distance = {dist}");
            }

            ReleaseFoot();
        }
    }

    void CheckInputDetach(Vector2 inputDir)
    {
        // 刚吸上的一小段时间内不允许因为输入立刻脱离
        if (Time.time - plantedTime <= minLockTime) return;

        if (inputDir.magnitude <= 0.2f) return;
        if (hipPivot == null) return;

        Vector2 footToHip = ((Vector2)hipPivot.position - (Vector2)transform.position).normalized;

        float dot = Vector2.Dot(inputDir.normalized, footToHip);

        if (dot > detachInputThreshold)
        {
            if (debugLog)
            {
                Debug.Log($"{name} released by input. Dot = {dot}");
            }

            ReleaseFoot();
        }
    }

    Vector2 GetInputDirection()
    {
        Vector2 inputDir = Vector2.zero;

        if (useExternalInput)
        {
            inputDir += externalInput;
        }

        if (Input.GetKey(upKey)) inputDir += Vector2.up;
        if (Input.GetKey(downKey)) inputDir += Vector2.down;
        if (Input.GetKey(leftKey)) inputDir += Vector2.left;
        if (Input.GetKey(rightKey)) inputDir += Vector2.right;

        return inputDir;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TrySetCandidate(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // 防止脚一开始就在 collider 里面，Enter 没触发
        if (!isPlanted && candidateFootHold == null)
        {
            TrySetCandidate(other);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (candidateFootCollider == other)
        {
            candidateFootHold = null;
            candidateFootCollider = null;
            candidateHoldType = FootHoldType.Long;

            if (debugLog)
            {
                Debug.Log($"{name} candidate cleared from {other.name}");
            }
        }
    }

    void TrySetCandidate(Collider2D other)
    {
        if (isPlanted) return;

        FootHoldType holdType;
        Transform holdRoot;

        if (!TryGetHoldInfo(other, out holdType, out holdRoot))
            return;

        candidateFootHold = holdRoot;
        candidateFootCollider = other;
        candidateHoldType = holdType;

        if (debugLog)
        {
            Debug.Log(
                $"{name} found candidate {candidateFootHold.name}, " +
                $"type = {candidateHoldType}, " +
                $"collider = {other.GetType().Name}"
            );
        }
    }

    bool TryGetHoldInfo(Collider2D other, out FootHoldType holdType, out Transform holdRoot)
    {
        holdType = FootHoldType.Long;
        holdRoot = null;

        // 先检查 collider 自己
        if (other.CompareTag("LongHandHold"))
        {
            holdType = FootHoldType.Long;
            holdRoot = other.transform;
            return true;
        }

        if (other.CompareTag("SlipperyHandHold"))
        {
            holdType = FootHoldType.Slippery;
            holdRoot = other.transform;
            return true;
        }

        // 再检查父物体，防止 collider 在子物体上，Tag 在父物体上
        Transform parent = other.transform.parent;

        while (parent != null)
        {
            if (parent.CompareTag("LongHandHold"))
            {
                holdType = FootHoldType.Long;
                holdRoot = parent;
                return true;
            }

            if (parent.CompareTag("SlipperyHandHold"))
            {
                holdType = FootHoldType.Slippery;
                holdRoot = parent;
                return true;
            }

            parent = parent.parent;
        }

        return false;
    }

    public void ReleaseFoot()
    {
        isPlanted = false;
        currentFootHold = null;
        currentHoldCollider = null;

        candidateFootHold = null;
        candidateFootCollider = null;

        candidateHoldType = FootHoldType.Long;
        currentHoldType = FootHoldType.Long;
    }
}