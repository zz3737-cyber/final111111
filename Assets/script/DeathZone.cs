using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();

        if (respawn == null)
        {
            respawn = other.GetComponentInParent<PlayerRespawn>();
        }

        if (respawn == null) return;
        if (respawn.isRespawning) return;

        respawn.Respawn();
    }
}