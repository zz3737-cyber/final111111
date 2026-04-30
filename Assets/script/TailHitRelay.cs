using UnityEngine;

public class TailHitRelay : MonoBehaviour
{
    public BossTailSweep bossTailSweep;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (bossTailSweep != null)
        {
            bossTailSweep.TryDisableLimb(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (bossTailSweep != null)
        {
            bossTailSweep.TryDisableLimb(other);
        }
    }
}