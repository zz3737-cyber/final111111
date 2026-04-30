using System.Collections;
using UnityEngine;

public class BossTailSweep : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject warningSprite;
    public GameObject sweepSprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip warningClip;
    public AudioClip sweepClip;

    [Header("Sweep Settings")]
    public float warningTime = 2f;
    public float sweepDuration = 1f;
    public float cooldown = 1.5f;

    [Header("Angles")]
    public float startAngle = 100f;
    public float endAngle = -60f;

    [Header("Hit")]
    public float gripDisableDuration = 1f;

    private bool isSweeping = false;
    private bool canHit = false;
    private int limbSensorLayer;

    private void Start()
    {
        limbSensorLayer = LayerMask.NameToLayer("LimbSensor");

        if (warningSprite != null) warningSprite.SetActive(false);
        if (sweepSprite != null) sweepSprite.SetActive(true);

        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void StartTailSweep()
    {
        if (!isSweeping)
        {
            StartCoroutine(TailSweepRoutine());
        }
    }

    private IEnumerator TailSweepRoutine()
    {
        isSweeping = true;

        transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);

        if (audioSource != null && warningClip != null)
        {
            audioSource.PlayOneShot(warningClip);
        }

        if (warningSprite != null) warningSprite.SetActive(true);
        if (sweepSprite != null) sweepSprite.SetActive(false);

        yield return new WaitForSeconds(warningTime);

        if (audioSource != null && sweepClip != null)
        {
            audioSource.PlayOneShot(sweepClip);
        }

        if (warningSprite != null) warningSprite.SetActive(false);
        if (sweepSprite != null) sweepSprite.SetActive(true);

        canHit = true;

        float timer = 0f;
        while (timer < sweepDuration)
        {
            timer += Time.deltaTime;
            float t = timer / sweepDuration;

            float z = Mathf.Lerp(startAngle, endAngle, t);
            transform.localRotation = Quaternion.Euler(0f, 0f, z);

            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, endAngle);

        canHit = false;

        if (sweepSprite != null) sweepSprite.SetActive(false);

        yield return new WaitForSeconds(cooldown);

        isSweeping = false;
    }

    public void TryDisableLimb(Collider2D other)
    {
        if (!canHit) return;
        if (other.gameObject.layer != limbSensorLayer) return;

        HandGrip grip = other.GetComponent<HandGrip>();
        if (grip != null)
        {
            grip.DisableGrip(gripDisableDuration);
        }

        FootPlant foot = other.GetComponent<FootPlant>();
        if (foot != null)
        {
            foot.DisablePlant(gripDisableDuration);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartTailSweep();
        }
    }
}