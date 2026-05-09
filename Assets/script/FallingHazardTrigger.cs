using System.Collections;
using UnityEngine;

public class FallingHazardTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject fallingHazardObject;
    public GameObject warningSign;

    [Header("Warning")]
    public float warningDuration = 1.5f;
    public float flickerInterval = 0.15f;

    [Header("Hazard Lifetime")]
    public float destroyHazardAfterSeconds = 10f;

    [Header("Options")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void Start()
    {
        if (warningSign != null)
        {
            warningSign.SetActive(false);
        }

        if (fallingHazardObject != null)
        {
            fallingHazardObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered) return;

        if (IsGrabbingThisHold(other))
        {
            hasTriggered = true;
            StartCoroutine(WarningThenActivateHazard());
        }
    }

    private bool IsGrabbingThisHold(Collider2D other)
    {
        HandGrip grip = other.GetComponent<HandGrip>();
        if (grip == null) grip = other.GetComponentInParent<HandGrip>();

        if (grip != null && grip.isGripping && grip.currentHold == transform)
        {
            return true;
        }

        FootPlant foot = other.GetComponent<FootPlant>();
        if (foot == null) foot = other.GetComponentInParent<FootPlant>();

        if (foot != null && foot.isPlanted && foot.currentFootHold == transform)
        {
            return true;
        }

        return false;
    }

    private IEnumerator WarningThenActivateHazard()
    {
        if (warningSign != null)
        {
            warningSign.SetActive(true);

            float timer = 0f;
            bool visible = true;

            while (timer < warningDuration)
            {
                yield return new WaitForSeconds(flickerInterval);

                timer += flickerInterval;
                visible = !visible;
                warningSign.SetActive(visible);
            }

            warningSign.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(warningDuration);
        }

        if (fallingHazardObject != null)
        {
            fallingHazardObject.SetActive(true);
            StartCoroutine(DestroyHazardAfterDelay());
        }
    }

    private IEnumerator DestroyHazardAfterDelay()
    {
        yield return new WaitForSeconds(destroyHazardAfterSeconds);

        if (fallingHazardObject != null)
        {
            Destroy(fallingHazardObject);
        }
    }
}