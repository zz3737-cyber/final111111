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

    void Start()
    {
        if (handRenderer == null)
        {
            handRenderer = GetComponent<SpriteRenderer>();
        }

        UpdateSprite();
    }

    void Update()
    {
        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (handGrip == null || handRenderer == null) return;

        if (handGrip.isGripping)
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