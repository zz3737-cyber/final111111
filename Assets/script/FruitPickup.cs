using UnityEngine;

public class FruitPickup : MonoBehaviour
{
    public enum FruitType
    {
        ArmStrength,
        Stamina,
        Antidote
    }

    [Header("Fruit Type")]
    public FruitType fruitType;

    [Header("Arm Strength Bonus")]
    public float armStrengthBonus = 5f;

    [Header("Stamina Bonus")]
    public float staminaBonus = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerFruitReceiver receiver = other.GetComponent<PlayerFruitReceiver>();

        if (receiver == null)
        {
            receiver = other.GetComponentInParent<PlayerFruitReceiver>();
        }

        if (receiver == null) return;

        switch (fruitType)
        {
            case FruitType.ArmStrength:
                receiver.AddArmStrength(armStrengthBonus);
                if (GameManager.instance != null)
                    GameManager.instance.ShowText("Arm Strength Up!");
                break;

            case FruitType.Stamina:
                receiver.AddStamina(staminaBonus);
                if (GameManager.instance != null)
                    GameManager.instance.ShowText("Stamina Up!");
                break;

            case FruitType.Antidote:
                receiver.AddAntidote();
                if (GameManager.instance != null)
                    GameManager.instance.ShowText("Antidote Collected!");
                break;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFruitPickup();
        }

        Destroy(gameObject);
    }
}