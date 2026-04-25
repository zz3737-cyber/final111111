using UnityEngine;

public class PlayerFruitReceiver : MonoBehaviour
{
    [Header("References")]
    public ClimberPhysicsGripMotor gripMotor;
    public HandGrip leftHandGrip;
    public HandGrip rightHandGrip;

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
}