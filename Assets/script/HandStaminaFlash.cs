using UnityEngine;

public class HandStaminaFlash : MonoBehaviour
{
    [Header("Reference")]
    public HandGrip handGrip;

    [Header("Renderers")]
    public SpriteRenderer[] renderersToFlash;

    [Header("Flash Color")]
    public Color flashColor = Color.red;

    [Header("Flash Settings")]
    public bool onlyFlashWhenGripping = true;

    [Range(0f, 1f)]
    public float startFlashBelowRatio = 0.8f;

    public float minFlashSpeed = 2f;
    public float maxFlashSpeed = 12f;

    [Range(0f, 1f)]
    public float maxFlashStrength = 0.8f;

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
        if (handGrip == null || renderersToFlash == null || renderersToFlash.Length == 0)
            return;

        if (handGrip.maxStamina <= 0f)
            return;

        float staminaRatio = handGrip.currentStamina / handGrip.maxStamina;
        staminaRatio = Mathf.Clamp01(staminaRatio);

        bool shouldFlash = staminaRatio < startFlashBelowRatio;

        if (onlyFlashWhenGripping && !handGrip.isGripping)
        {
            shouldFlash = false;
        }

        if (!shouldFlash)
        {
            RestoreOriginalColors();
            return;
        }

        // 耐力越低，danger 越高
        float danger = 1f - staminaRatio;

        // 映射闪烁速度
        float flashSpeed = Mathf.Lerp(minFlashSpeed, maxFlashSpeed, danger);

        // 0 到 1 的闪烁波
        float flashWave = (Mathf.Sin(Time.time * flashSpeed) + 1f) * 0.5f;

        // 耐力越低，颜色越明显
        float flashStrength = flashWave * Mathf.Lerp(0.2f, maxFlashStrength, danger);

        for (int i = 0; i < renderersToFlash.Length; i++)
        {
            if (renderersToFlash[i] == null) continue;

            Color baseColor = originalColors[i];
            renderersToFlash[i].color = Color.Lerp(baseColor, flashColor, flashStrength);
        }
    }

    void RestoreOriginalColors()
    {
        for (int i = 0; i < renderersToFlash.Length; i++)
        {
            if (renderersToFlash[i] == null) continue;

            renderersToFlash[i].color = originalColors[i];
        }
    }
}