using System.Collections;
using UnityEngine;

public class BossTailSweep : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject warningSprite;
    public GameObject sweepSprite;

    [Header("Sweep Settings")]
    public float warningTime = 0.8f;
    public float sweepDuration = 0.35f;
    public float cooldown = 1.5f;

    [Header("Angles")]
    public float startAngle = 60f;
    public float endAngle = -60f;

    [Header("Hit")]
    public float gripDisableDuration = 0.5f;

    private bool isSweeping = false;
    private bool canHit = false;

    private void Start()
    {
        if (warningSprite != null) warningSprite.SetActive(false);
        if (sweepSprite != null) sweepSprite.SetActive(false);

        transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);
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

        // reset rotation
        transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);

        // show warning
        if (warningSprite != null) warningSprite.SetActive(true);
        if (sweepSprite != null) sweepSprite.SetActive(false);

        yield return new WaitForSeconds(warningTime);

        // start sweep
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

private void OnTriggerEnter2D(Collider2D other)
{
    if (!canHit) return;

    if (other.gameObject.layer != LayerMask.NameToLayer("LimbSensor"))
        return;

    HandGrip grip = other.GetComponent<HandGrip>();
    if (grip != null)
    {
        grip.DisableGrip(gripDisableDuration);
        return;
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