using UnityEngine;

public class RightSideGamepadBinder : MonoBehaviour
{
    public RightSideGamepadInput inputSource;

    [Header("Right Side Targets")]
    public FourDirectionLimb rightHandLimb;
    public FourDirectionLimb rightFootLimb;
    public HandGrip rightHandGrip;
    public FootPlant rightFootPlant;

    [Header("Dead Zone")]
    public float deadZone = 0.2f;

    void Update()
    {
        if (inputSource == null) return;

        bool handStickActive = inputSource.handMove.magnitude > deadZone;
        bool footStickActive = inputSource.footMove.magnitude > deadZone;

        if (rightHandLimb != null)
        {
            rightHandLimb.useExternalInput = handStickActive;
            rightHandLimb.externalInput = inputSource.handMove;
        }

        if (rightFootLimb != null)
        {
            rightFootLimb.useExternalInput = footStickActive;
            rightFootLimb.externalInput = inputSource.footMove;
        }

        if (rightHandGrip != null)
        {
            rightHandGrip.useExternalGrip = inputSource.gripHeld;
            rightHandGrip.externalGripHeld = inputSource.gripHeld;
        }

        if (rightFootPlant != null)
        {
            rightFootPlant.useExternalInput = true;
            rightFootPlant.externalInput = inputSource.footMove;
        }
    }
}