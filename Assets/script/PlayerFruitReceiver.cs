using UnityEngine;

public class PlayerFruitReceiver : MonoBehaviour
{
    [Header("References")]
    public ClimberPhysicsGripMotor gripMotor;
    public HandGrip leftHandGrip;
    public HandGrip rightHandGrip;

    [Header("Antidote")]
    public int antidoteCount = 0;
    public int antidotesToEndGame = 3;

    public void AddArmStrength(float amount)
    {
        if (gripMotor != null)
        {
            gripMotor.handPullForce += amount;
            Debug.Log("Arm strength increased to: " + gripMotor.handPullForce);
        }
    }

    public void AddStamina(float amount)
    {
        if (leftHandGrip != null)
        {
            leftHandGrip.maxStamina += amount;
            leftHandGrip.currentStamina = leftHandGrip.maxStamina;
        }

        if (rightHandGrip != null)
        {
            rightHandGrip.maxStamina += amount;
            rightHandGrip.currentStamina = rightHandGrip.maxStamina;
        }

        Debug.Log("Stamina increased by: " + amount);
    }

    public void AddAntidote()
    {
        antidoteCount++;

        Debug.Log("Antidote collected: " + antidoteCount + " / " + antidotesToEndGame);

        if (antidoteCount >= antidotesToEndGame)
        {
            if (GameEndManager.Instance != null)
            {
                GameEndManager.Instance.EndGame();
            }
            else
            {
                Debug.Log("Game End! Collected all antidotes.");
            }
        }
    }
}