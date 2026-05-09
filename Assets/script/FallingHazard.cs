using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    [Header("Fall")]
    public float fallSpeed = 4f;
    public bool startFallingOnStart = false;
    public float destroyAfterSeconds = 5f;

    [Header("Hit")]
    public float disableDuration = 1f;
    public string targetLayerName = "LimbSensor";

    private bool isFalling = false;
    private float fallTimer = 0f;
    private int targetLayer;


    private void Start()
    {
        targetLayer = LayerMask.NameToLayer(targetLayerName);

        if (startFallingOnStart)
        {
            StartFalling();
        }
    }

    private void Update()
    {
        if (!isFalling) return;

        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        fallTimer += Time.deltaTime;
        if (fallTimer >= destroyAfterSeconds)
        {
            Destroy(gameObject);
        }
    }

    public void StartFalling()
    {
        isFalling = true;
        fallTimer = 0f;
    }

    
    public void TryDisableLimb(Collider2D other)
    {

        HandGrip grip = other.GetComponent<HandGrip>();
        if (grip != null)
        {
            grip.DisableGrip(disableDuration);
        }

        FootPlant foot = other.GetComponent<FootPlant>();
        if (foot != null)
        {
            foot.DisablePlant(disableDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hazard hit: " + other.name);
        TryDisableLimb(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Hazard staying on: " + other.name);
        TryDisableLimb(other);
    }
    

}