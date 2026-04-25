using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D bodyRb;

    [Header("Respawn Point")]
    public Transform startRespawnPoint;
    public Vector3 currentRespawnPosition;

    [Header("Hands")]
    public HandGrip leftHandGrip;
    public HandGrip rightHandGrip;

    [Header("Feet")]
    public FootPlant leftFootPlant;
    public FootPlant rightFootPlant;

    [Header("Respawn Pause")]
    public float respawnFreezeTime = 0.8f;

    [Header("Safety")]
    public float respawnInvincibleTime = 0.5f;
    public bool isRespawning = false;

    [Header("Test")]
    public KeyCode testRespawnKey = KeyCode.R;

    private Coroutine respawnRoutine;

    void Start()
    {
        if (startRespawnPoint != null)
        {
            currentRespawnPosition = startRespawnPoint.position;
        }
        else
        {
            currentRespawnPosition = transform.position;
        }

        if (bodyRb == null)
        {
            bodyRb = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(testRespawnKey))
        {
            Respawn();
        }
    }

    public void SetRespawnPoint(Vector3 newPosition)
    {
        currentRespawnPosition = newPosition;
        Debug.Log("Respawn point set to: " + currentRespawnPosition);
    }

    public void Respawn()
    {
        if (isRespawning) return;

        if (respawnRoutine != null)
        {
            StopCoroutine(respawnRoutine);
        }

        respawnRoutine = StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (leftHandGrip != null) leftHandGrip.ForceReleaseForRespawn();
        if (rightHandGrip != null) rightHandGrip.ForceReleaseForRespawn();

        if (leftFootPlant != null) leftFootPlant.ReleaseFoot();
        if (rightFootPlant != null) rightFootPlant.ReleaseFoot();

        if (bodyRb != null)
        {
            bodyRb.linearVelocity = Vector2.zero;
            bodyRb.angularVelocity = 0f;
            bodyRb.position = currentRespawnPosition;

            bodyRb.bodyType = RigidbodyType2D.Kinematic;
            bodyRb.linearVelocity = Vector2.zero;
            bodyRb.angularVelocity = 0f;
        }
        else
        {
            transform.position = currentRespawnPosition;
        }

        yield return new WaitForSeconds(respawnFreezeTime);

        if (bodyRb != null)
        {
            bodyRb.bodyType = RigidbodyType2D.Dynamic;
            bodyRb.linearVelocity = Vector2.zero;
            bodyRb.angularVelocity = 0f;
        }

        yield return new WaitForSeconds(respawnInvincibleTime);

        isRespawning = false;
        respawnRoutine = null;
    }
}