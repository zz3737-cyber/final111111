using System.Collections;
using UnityEngine;

public class BossBodyRandomRotate : MonoBehaviour
{
    [Header("Startup Delay")]
    public float initialCooldown = 15f;

    [Header("Random Interval")]
    public float minTimeBetweenRotations = 5f;
    public float maxTimeBetweenRotations = 10f;

    [Header("Rotation")]
    public float maxAngle = 50f;
    public float rotateDuration = 0.4f;

    private bool isRotating = false;

    // sequence: 50 -> 0 -> -50 -> 0
    private int sequenceIndex = 0;
    private float[] angleSequence;

    private void Start()
    {
        angleSequence = new float[] { 50f, 0f, -50f, 0f };
        StartCoroutine(RandomRotateRoutine());
    }

    private IEnumerator RandomRotateRoutine()
    {
        yield return new WaitForSeconds(initialCooldown);

        while (true)
        {
            float waitTime = Random.Range(minTimeBetweenRotations, maxTimeBetweenRotations);
            yield return new WaitForSeconds(waitTime);

            if (!isRotating)
            {
                float targetAngle = angleSequence[sequenceIndex];
                sequenceIndex = (sequenceIndex + 1) % angleSequence.Length;

                yield return StartCoroutine(RotateToAngle(targetAngle));
            }
        }
    }

    private IEnumerator RotateToAngle(float targetZAngle)
    {
        isRotating = true;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(0f, 0f, targetZAngle);

        float timer = 0f;

        while (timer < rotateDuration)
        {
            timer += Time.deltaTime;
            float t = timer / rotateDuration;
            transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        transform.localRotation = endRot;
        isRotating = false;
    }
}