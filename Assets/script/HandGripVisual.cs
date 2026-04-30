using UnityEngine;

public class HandGripVisual : MonoBehaviour
{
    [Header("Reference")]
    public HandGrip handGrip;

    [Header("Sprite Renderer")]
    public SpriteRenderer handRenderer;

    [Header("Sprites")]
    public Sprite normalHandSprite;
    public Sprite grippingHandSprite;

    private bool lastGripState;

    void Start()
    {
        if (handRenderer == null)
        {
            handRenderer = GetComponent<SpriteRenderer>();
        }

        lastGripState = false;
        UpdateSprite(true);
    }

    void Update()
    {
        UpdateSprite(false);
    }

    void UpdateSprite(bool forceUpdate)
    {
        if (handGrip == null || handRenderer == null) return;

        bool gripping = handGrip.isGripping;

        if (!forceUpdate && gripping == lastGripState) return;

        lastGripState = gripping;

        if (gripping)
        {
            if (grippingHandSprite != null)
            {
                handRenderer.sprite = grippingHandSprite;
            }
        }
        else
        {
            if (normalHandSprite != null)
            {
                handRenderer.sprite = normalHandSprite;
            }
        }
    }
}