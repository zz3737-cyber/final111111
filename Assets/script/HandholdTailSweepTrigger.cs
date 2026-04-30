using UnityEngine;

public class HandholdTailSweepTrigger : MonoBehaviour
{
    public BossTailSweep bossTailSweep;

    [Header("Tail Location")]
    public Transform tailPoint;

    [Header("Optional Angles")]
    public bool overrideAngles = false;
    public float startAngle = 100f;
    public float endAngle = -60f;

    [Header("Filter")]
    public string requiredLayerName = "LimbSensor";

    [Header("Cooldown")]
    public float cooldown = 10f;

    private int requiredLayer;
    private float cooldownTimer = 0f;
    private bool playerInside = false;

    void Start()
    {
        requiredLayer = LayerMask.NameToLayer(requiredLayerName);
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (playerInside && cooldownTimer <= 0f)
        {
            TriggerTailSweep();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != requiredLayer) return;

        playerInside = true;

        if (cooldownTimer <= 0f)
        {
            TriggerTailSweep();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != requiredLayer) return;

        playerInside = false;
    }

    void TriggerTailSweep()
    {
        if (bossTailSweep == null) return;

        cooldownTimer = cooldown;

        if (tailPoint != null)
        {
            bossTailSweep.transform.position = tailPoint.position;
            bossTailSweep.transform.rotation = tailPoint.rotation;
        }

        if (overrideAngles)
        {
            bossTailSweep.startAngle = startAngle;
            bossTailSweep.endAngle = endAngle;
        }

        bossTailSweep.StartTailSweep();
    }
}