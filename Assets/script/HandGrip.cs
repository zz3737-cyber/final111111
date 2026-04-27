using UnityEngine;

public class HandGrip : MonoBehaviour
{
    [Header("Keyboard Grip")]
    public KeyCode gripKey = KeyCode.Space;

    [Header("External Grip")]
    public bool useExternalGrip = false;
    public bool externalGripHeld = false;

    [Header("State")]
    public bool isGripping = false;
    public Transform currentHold;

    [Header("Stamina")]
    public float maxStamina = 10f;
    public float currentStamina = 10f;
    public float drainPerSecond = 1f;

    [Header("Regrab Cooldown")]
    public float regrabCooldown = 0.2f;

    [Header("Stamina Empty Cooldown")]
    public float staminaEmptyCooldown = 0.5f;

    private float regrabTimer = 0f;

    [Header("Grip Disable")]
    public bool gripDisabled = false;
    private float gripDisableTimer = 0f;

    [Header("Default Slippery Hold")]
    public float defaultSlipperySpeed = 0.2f;

    [Header("Reach Limit")]
    public Transform shoulderPivot;
    public float maxReach = 1.5f;

    private Transform candidateHold;
    private Collider2D candidateCollider;

    private enum HoldType
    {
        Long,
        Slippery
    }

    private HoldType candidateHoldType = HoldType.Long;
    private HoldType currentHoldType = HoldType.Long;

    private Collider2D currentHoldCollider;

    private Vector3 localGripPoint;

    private float candidateSlipDirection = 1f;
    private float candidateSlipSpeed = 0.2f;

    private float currentSlipDirection = 1f;
    private float currentSlipSpeed = 0.2f;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        bool gripHeld = Input.GetKey(gripKey) || (useExternalGrip && externalGripHeld);

        if (gripDisabled)
        {
            gripDisableTimer -= Time.deltaTime;

            if (gripDisableTimer <= 0f)
            {
                gripDisabled = false;
            }

            ForceReleaseAll(false);
            return;
        }

        if (regrabTimer > 0f)
        {
            regrabTimer -= Time.deltaTime;
        }

        // 开始抓住
        if (gripHeld && candidateHold != null && candidateCollider != null && !isGripping && regrabTimer <= 0f)
        {
            isGripping = true;

            currentHold = candidateHold;
            currentHoldCollider = candidateCollider;
            currentHoldType = candidateHoldType;

            currentSlipDirection = candidateSlipDirection;
            currentSlipSpeed = candidateSlipSpeed;

            // 重新抓住时耐力回满
            currentStamina = maxStamina;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHandGrab();
            }

            Vector3 closestWorldPoint = currentHoldCollider.ClosestPoint(transform.position);
            localGripPoint = currentHold.InverseTransformPoint(closestWorldPoint);
        }

        // 主动松手
        if (!gripHeld && isGripping)
        {
            ReleaseCurrentGrip(regrabCooldown);
            return;
        }

        // 抓住时持续扣耐力
        if (isGripping && currentHold != null)
        {
            currentStamina -= drainPerSecond * Time.deltaTime;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;

                // 耐力空了以后，进入专门的 0.5 秒抓取冷却
                ReleaseCurrentGrip(staminaEmptyCooldown);
                return;
            }

            switch (currentHoldType)
            {
                case HoldType.Long:
                    UpdateLongGrip();
                    break;

                case HoldType.Slippery:
                    UpdateSlipperyGrip();
                    break;
            }
        }
    }

    void UpdateLongGrip()
    {
        if (currentHoldCollider == null)
        {
            ForceReleaseAll(true);
            return;
        }

        Vector3 target = currentHold.TransformPoint(localGripPoint);
        transform.position = ClampToReach(target);
    }

    void UpdateSlipperyGrip()
    {
        if (currentHoldCollider == null)
        {
            ForceReleaseAll(true);
            return;
        }

        Vector3 nextLocalGripPoint = localGripPoint;

        // 每个滑点自己的速度和方向
        nextLocalGripPoint.x += currentSlipDirection * currentSlipSpeed * Time.deltaTime;

        Vector3 desiredWorldPoint = currentHold.TransformPoint(nextLocalGripPoint);

        // 支持 PolygonCollider2D，把目标点拉回 collider 表面
        Vector3 surfaceWorldPoint = currentHoldCollider.ClosestPoint(desiredWorldPoint);

        Vector3 slipperyTarget = surfaceWorldPoint;

        transform.position = ClampToReach(slipperyTarget);

        if (shoulderPivot == null || Vector2.Distance(shoulderPivot.position, slipperyTarget) <= maxReach)
        {
            localGripPoint = currentHold.InverseTransformPoint(surfaceWorldPoint);
        }
    }

    Vector3 ClampToReach(Vector3 targetWorldPos)
    {
        if (shoulderPivot == null) return targetWorldPos;

        Vector2 shoulderPos = shoulderPivot.position;
        Vector2 targetPos = targetWorldPos;

        Vector2 offset = targetPos - shoulderPos;
        float dist = offset.magnitude;

        if (dist <= maxReach)
        {
            return targetWorldPos;
        }

        return shoulderPos + offset.normalized * maxReach;
    }

    // 给 ClimberPhysicsGripMotor 用
    public Vector2 GetConnectedAnchorLocal()
    {
        if (currentHold == null)
        {
            return Vector2.zero;
        }

        return localGripPoint;
    }

    public void DisableGrip(float duration)
    {
        gripDisabled = true;
        gripDisableTimer = duration;
        ForceReleaseAll(false);
    }

    public void ForceReleaseForRespawn()
    {
        isGripping = false;
        currentHold = null;
        currentHoldCollider = null;

        candidateHold = null;
        candidateCollider = null;

        currentStamina = maxStamina;
        regrabTimer = 0f;
    }

    void ReleaseCurrentGrip(float cooldownTime)
    {
        isGripping = false;
        currentHold = null;
        currentHoldCollider = null;

        regrabTimer = cooldownTime;
    }

    void ForceReleaseAll(bool startCooldown)
    {
        isGripping = false;
        currentHold = null;
        currentHoldCollider = null;

        candidateHold = null;
        candidateCollider = null;

        if (startCooldown)
        {
            regrabTimer = regrabCooldown;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TrySetCandidate(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isGripping && candidateHold == null)
        {
            TrySetCandidate(other);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (candidateCollider == other)
        {
            candidateHold = null;
            candidateCollider = null;

            candidateHoldType = HoldType.Long;
            candidateSlipDirection = 1f;
            candidateSlipSpeed = defaultSlipperySpeed;
        }

        if (currentHoldCollider == other)
        {
            ReleaseCurrentGrip(regrabCooldown);
        }
    }

    void TrySetCandidate(Collider2D other)
    {
        if (isGripping) return;

        HoldType holdType;
        Transform holdRoot;
        float slipDirection;
        float slipSpeed;

        if (!TryGetHoldInfo(other, out holdType, out holdRoot, out slipDirection, out slipSpeed))
        {
            return;
        }

        candidateHold = holdRoot;
        candidateCollider = other;
        candidateHoldType = holdType;
        candidateSlipDirection = slipDirection;
        candidateSlipSpeed = slipSpeed;
    }

    bool TryGetHoldInfo(
        Collider2D other,
        out HoldType holdType,
        out Transform holdRoot,
        out float slipDirection,
        out float slipSpeed
    )
    {
        holdType = HoldType.Long;
        holdRoot = null;
        slipDirection = 1f;
        slipSpeed = defaultSlipperySpeed;

        HoldSurface2D surface = other.GetComponent<HoldSurface2D>();

        if (surface == null)
        {
            surface = other.GetComponentInParent<HoldSurface2D>();
        }

        if (surface != null)
        {
            holdRoot = surface.transform;

            if (surface.holdType == HoldSurfaceType.Slippery)
            {
                holdType = HoldType.Slippery;
            }
            else
            {
                holdType = HoldType.Long;
            }

            slipDirection = Mathf.Sign(surface.slipperyDirection);
            if (slipDirection == 0f) slipDirection = 1f;

            slipSpeed = Mathf.Max(0f, surface.slipperySpeed);

            return true;
        }

        if (other.CompareTag("LongHandHold"))
        {
            holdType = HoldType.Long;
            holdRoot = other.transform;
            return true;
        }

        if (other.CompareTag("SlipperyHandHold"))
        {
            holdType = HoldType.Slippery;
            holdRoot = other.transform;
            slipDirection = 1f;
            slipSpeed = defaultSlipperySpeed;
            return true;
        }

        Transform parent = other.transform.parent;

        while (parent != null)
        {
            if (parent.CompareTag("LongHandHold"))
            {
                holdType = HoldType.Long;
                holdRoot = parent;
                return true;
            }

            if (parent.CompareTag("SlipperyHandHold"))
            {
                holdType = HoldType.Slippery;
                holdRoot = parent;
                slipDirection = 1f;
                slipSpeed = defaultSlipperySpeed;
                return true;
            }

            parent = parent.parent;
        }

        return false;
    }
}