using UnityEngine;

public class FootPlantInputBinder : MonoBehaviour
{
    [Header("Left Side")]
    public LeftSideGamepadInput leftInput;
    public FootPlant leftFootPlant;

    [Header("Right Side")]
    public RightSideGamepadInput rightInput;
    public FootPlant rightFootPlant;

    void Update()
    {
        if (leftInput != null && leftFootPlant != null)
        {
            leftFootPlant.useExternalInput = true;
            leftFootPlant.externalInput = leftInput.footMove;

            leftFootPlant.useExternalPlantButton = true;
            leftFootPlant.externalPlantHeld = leftInput.footPlantHeld;
        }

        if (rightInput != null && rightFootPlant != null)
        {
            rightFootPlant.useExternalInput = true;
            rightFootPlant.externalInput = rightInput.footMove;

            rightFootPlant.useExternalPlantButton = true;
            rightFootPlant.externalPlantHeld = rightInput.footPlantHeld;
        }
    }
}