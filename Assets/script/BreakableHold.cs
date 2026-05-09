using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableHold : MonoBehaviour
{
    [Header("Break Settings")]
    public float breakTime = 2f;
    public float disableDuration = 0.5f;
    public bool onlyLimbSensorLayer = true;
    public string limbSensorLayerName = "LimbSensor";

    [Header("Fall Settings")]
    public float fallSpeed = 3f;
    public float destroyAfterFallTime = 3f;
    public bool breakOnlyOnce = true;

    [Header("Destroy Immediately On Break")]
    public GameObject objectToDestroyImmediately;
    public GameObject anotherObjectToDestroyImmediately;

    [Header("Activate Later")]
    public GameObject objectToActivateAfterDisableDuration;

    private readonly HashSet<Collider2D> touchingColliders = new HashSet<Collider2D>();
    private readonly HashSet<HandGrip> touchingHands = new HashSet<HandGrip>();
    private readonly HashSet<FootPlant> touchingFeet = new HashSet<FootPlant>();

    private float contactTimer = 0f;
    private bool isBroken = false;
    private bool isFalling = false;
    private float fallTimer = 0f;
    private int limbSensorLayer = -1;

    private void Awake()
    {
        limbSensorLayer = LayerMask.NameToLayer(limbSensorLayerName);
    }

    private void Update()
    {
        if (!isBroken)
        {
            if (touchingColliders.Count > 0)
            {
                contactTimer += Time.deltaTime;

                if (contactTimer >= breakTime)
                {
                    BreakHold();
                }
            }
            else
            {
                contactTimer = 0f;
            }
        }

        if (isFalling)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            fallTimer += Time.deltaTime;
            if (fallTimer >= destroyAfterFallTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RegisterCollider(collision.collider);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        UnregisterCollider(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RegisterCollider(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        UnregisterCollider(other);
    }

    private void RegisterCollider(Collider2D other)
    {
        if (isBroken && breakOnlyOnce) return;
        if (!IsValidCollider(other)) return;

        touchingColliders.Add(other);

        HandGrip grip = other.GetComponent<HandGrip>();
        if (grip == null) grip = other.GetComponentInParent<HandGrip>();
        if (grip != null)
        {
            touchingHands.Add(grip);
        }

        FootPlant foot = other.GetComponent<FootPlant>();
        if (foot == null) foot = other.GetComponentInParent<FootPlant>();
        if (foot != null)
        {
            touchingFeet.Add(foot);
        }
    }

    private void UnregisterCollider(Collider2D other)
    {
        if (touchingColliders.Contains(other))
        {
            touchingColliders.Remove(other);
        }

        HandGrip grip = other.GetComponent<HandGrip>();
        if (grip == null) grip = other.GetComponentInParent<HandGrip>();
        if (grip != null)
        {
            touchingHands.Remove(grip);
        }

        FootPlant foot = other.GetComponent<FootPlant>();
        if (foot == null) foot = other.GetComponentInParent<FootPlant>();
        if (foot != null)
        {
            touchingFeet.Remove(foot);
        }

        if (touchingColliders.Count == 0)
        {
            contactTimer = 0f;
        }
    }

    private bool IsValidCollider(Collider2D other)
    {
        if (other == null) return false;

        if (!onlyLimbSensorLayer) return true;

        return other.gameObject.layer == limbSensorLayer;
    }

    private void BreakHold()
{
    if (isBroken && breakOnlyOnce) return;

    isBroken = true;
    contactTimer = 0f;

    if (objectToDestroyImmediately != null)
    {
        Destroy(objectToDestroyImmediately);
    }

    if (anotherObjectToDestroyImmediately != null)
    {
        Destroy(anotherObjectToDestroyImmediately);
    }

    foreach (HandGrip grip in touchingHands)
    {
        if (grip != null)
        {
            grip.DisableGrip(disableDuration);
        }
    }

    foreach (FootPlant foot in touchingFeet)
    {
        if (foot != null)
        {
            foot.DisablePlant(disableDuration);
        }
    }

    if (objectToActivateAfterDisableDuration != null)
    {
        Invoke(nameof(ActivateTargetObject), disableDuration);
    }

    touchingHands.Clear();
    touchingFeet.Clear();
    touchingColliders.Clear();

    StartFalling();
}

private void ActivateTargetObject()
{
    if (objectToActivateAfterDisableDuration != null)
    {
        objectToActivateAfterDisableDuration.SetActive(true);
    }
}

    private void StartFalling()
    {
        isFalling = true;
        fallTimer = 0f;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
    }
}