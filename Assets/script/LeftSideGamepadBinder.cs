using UnityEngine;

public class LeftSideGamepadBinder : MonoBehaviour
{
    public LeftSideGamepadInput inputSource;

    [Header("Left Side Targets")]
    public FourDirectionLimb leftHandLimb;
    public FourDirectionLimb leftFootLimb;
    public HandGrip leftHandGrip;
    public FootPlant leftFootPlant;

    [Header("Dead Zone")]
    public float deadZone = 0.2f;

    void Update()
    {
        if (inputSource == null) return;

        bool handStickActive = inputSource.handMove.magnitude > deadZone;
        bool footStickActive = inputSource.footMove.magnitude > deadZone;

        if (leftHandLimb != null)
        {
            leftHandLimb.useExternalInput = handStickActive;
            leftHandLimb.externalInput = inputSource.handMove;
        }

        if (leftFootLimb != null)
        {
            leftFootLimb.useExternalInput = footStickActive;
            leftFootLimb.externalInput = inputSource.footMove;
        }

        if (leftHandGrip != null)
        {
            leftHandGrip.useExternalGrip = inputSource.gripHeld;
            leftHandGrip.externalGripHeld = inputSource.gripHeld;
        }

        if (leftFootPlant != null)
        {
            leftFootPlant.useExternalInput = true;
            leftFootPlant.externalInput = inputSource.footMove;
        }
    }
}