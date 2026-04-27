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
    public float staminaDrainMultiplier = 1f;

    [Header("Regrab Cooldown")]
    public float regrabCooldown = 0.5f;
    private float regrabTimer = 0f;

    [Header("Grip Disable")]
    public bool gripDisabled = false;
    private float gripDisableTimer = 0f;

    [Header("Slippery Hold")]
    public float slipperySpeed = 0.5f;

    [Header("Reach Limit")]
    public Transform shoulderPivot;
    public float maxReach = 1.5f;

    private Transform candidateHold;

    private enum HoldType
    {
        Normal,
        Long,
        Slippery
    }

    private HoldType candidateHoldType = HoldType.Normal;
    private HoldType currentHoldType = HoldType.Normal;

    private Collider2D currentHoldCollider;

    private Vector3 localGripPoint;
    private float currentSlipDirection = 1f;

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

        if (gripHeld && candidateHold != null && !isGripping && regrabTimer <= 0f)
        {
            isGripping = true;
            currentHold = candidateHold;
            currentHoldType = candidateHoldType;

            currentStamina = maxStamina;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHandGrab();
            }

            if (currentHoldType == HoldType.Long || currentHoldType == HoldType.Slippery)
            {
                currentHoldCollider = currentHold.GetComponent<Collider2D>();

                if (currentHoldCollider != null)
                {
                    Vector3 closestWorldPoint = currentHoldCollider.ClosestPoint(transform.position);
                    localGripPoint = currentHold.InverseTransformPoint(closestWorldPoint);
                }
                else
                {
                    currentHoldType = HoldType.Normal;
                    currentHoldCollider = null;
                }
            }
            else
            {
                currentHoldCollider = null;
            }
        }

        if (!gripHeld && isGripping)
        {
            ReleaseCurrentGrip(true);
            return;
        }

        if (isGripping && currentHold != null)
        {
            currentStamina -= drainPerSecond * staminaDrainMultiplier * Time.deltaTime;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                ReleaseCurrentGrip(true);
                return;
            }

            switch (currentHoldType)
            {
                case HoldType.Normal:
                    transform.position = currentHold.position;
                    break;

                case HoldType.Long:
                    if (currentHoldCollider == null)
                    {
                        ForceReleaseAll(true);
                        return;
                    }

                    Vector3 longTarget = currentHold.TransformPoint(localGripPoint);
                    transform.position = ClampToReach(longTarget);
                    break;

                case HoldType.Slippery:
                    if (currentHoldCollider == null)
                    {
                        ForceReleaseAll(true);
                        return;
                    }

                    UpdateSlipperyGrip();
                    break;
            }
        }
    }

    void UpdateSlipperyGrip()
    {
        Vector3 nextLocalGripPoint = localGripPoint;
        nextLocalGripPoint.x += currentSlipDirection * slipperySpeed * Time.deltaTime;

        Vector3 desiredWorldPoint = currentHold.TransformPoint(nextLocalGripPoint);
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

    public Vector2 GetConnectedAnchorLocal()
    {
        if (currentHold == null)
        {
            return Vector2.zero;
        }

        if (currentHoldType == HoldType.Long || currentHoldType == HoldType.Slippery)
        {
            return localGripPoint;
        }

        return Vector2.zero;
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
        currentHoldType = HoldType.Normal;
        currentHoldCollider = null;
        candidateHold = null;
        candidateHoldType = HoldType.Normal;

        currentStamina = maxStamina;
    }

    void ReleaseCurrentGrip(bool startCooldown)
    {
        isGripping = false;
        currentHold = null;
        currentHoldType = HoldType.Normal;
        currentHoldCollider = null;

        if (startCooldown)
        {
            regrabTimer = regrabCooldown;
        }
    }

    void ForceReleaseAll(bool startCooldown)
    {
        isGripping = false;
        currentHold = null;
        currentHoldType = HoldType.Normal;
        currentHoldCollider = null;

        candidateHold = null;
        candidateHoldType = HoldType.Normal;

        if (startCooldown)
        {
            regrabTimer = regrabCooldown;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (TryReadHoldSurface(other, out HoldType holdType, out float slipDirection))
        {
            candidateHold = other.transform;
            candidateHoldType = holdType;
            currentSlipDirection = slipDirection;
            return;
        }

        if (other.CompareTag("HandHold"))
        {
            candidateHold = other.transform;
            candidateHoldType = HoldType.Normal;
            currentSlipDirection = 1f;
        }
        else if (other.CompareTag("LongHandHold"))
        {
            candidateHold = other.transform;
            candidateHoldType = HoldType.Long;
            currentSlipDirection = 1f;
        }
        else if (other.CompareTag("SlipperyHandHold"))
        {
            candidateHold = other.transform;
            candidateHoldType = HoldType.Slippery;
            currentSlipDirection = 1f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (candidateHold == other.transform)
        {
            candidateHold = null;
            candidateHoldType = HoldType.Normal;
        }

        if (currentHold == other.transform)
        {
            ReleaseCurrentGrip(true);
        }
    }

    bool TryReadHoldSurface(Collider2D other, out HoldType holdType, out float slipDirection)
    {
        holdType = HoldType.Normal;
        slipDirection = 1f;

        HoldSurface2D surface = other.GetComponent<HoldSurface2D>();
        if (surface == null) return false;

        switch (surface.holdType)
        {
            case HoldSurfaceType.Normal:
                holdType = HoldType.Normal;
                break;

            case HoldSurfaceType.Long:
                holdType = HoldType.Long;
                break;

            case HoldSurfaceType.Slippery:
                holdType = HoldType.Slippery;
                break;
        }

        slipDirection = Mathf.Sign(surface.slipperyDirection);
        if (slipDirection == 0f) slipDirection = 1f;

        return true;
    }
}