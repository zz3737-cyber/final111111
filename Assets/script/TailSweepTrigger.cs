using UnityEngine;

public class TailSweepTrigger : MonoBehaviour
{
    public BossTailSweep bossTailSweep;
    private bool hasTriggered = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasTriggered) return;

        hasTriggered = true;

        if (bossTailSweep != null)
        {
            bossTailSweep.StartTailSweep();
        }
    }
}