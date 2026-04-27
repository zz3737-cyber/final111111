using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();

        if (respawn == null)
        {
            respawn = other.GetComponentInParent<PlayerRespawn>();
        }

        if (respawn == null) return;

        respawn.SetRespawnPoint(transform.position);

        if (!activated)
        {
            activated = true;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCheckpoint();
            }

            Debug.Log("Checkpoint activated: " + gameObject.name);
        }
    }
}