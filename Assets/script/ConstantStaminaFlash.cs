using UnityEngine;

public class ConstantStaminaFlash : MonoBehaviour
{
    [Header("Renderers")]
    public SpriteRenderer[] renderersToFlash;

    [Header("Flash Color")]
    public Color flashColor = Color.red;

    [Header("Flash Settings")]
    public float flashSpeed = 14f;
    [Range(0f, 1f)]
    public float flashStrength = 0.8f;

    private Color[] originalColors;

    void Start()
    {
        if (renderersToFlash == null || renderersToFlash.Length == 0)
        {
            renderersToFlash = GetComponentsInChildren<SpriteRenderer>();
        }

        originalColors = new Color[renderersToFlash.Length];

        for (int i = 0; i < renderersToFlash.Length; i++)
        {
            if (renderersToFlash[i] != null)
            {
                originalColors[i] = renderersToFlash[i].color;
            }
        }
    }

    void Update()
    {
        if (renderersToFlash == null || renderersToFlash.Length == 0) return;

        float wave = (Mathf.Sin(Time.time * flashSpeed) + 1f) * 0.5f;
        float amount = wave * flashStrength;

        for (int i = 0; i < renderersToFlash.Length; i++)
        {
            if (renderersToFlash[i] == null) continue;

            Color baseColor = originalColors[i];
            renderersToFlash[i].color = Color.Lerp(baseColor, flashColor, amount);
        }
    }
}