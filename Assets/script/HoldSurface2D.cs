using UnityEngine;

public enum HoldSurfaceType
{
    Long,
    Slippery
}

public class HoldSurface2D : MonoBehaviour
{
    [Header("Hold Type")]
    public HoldSurfaceType holdType = HoldSurfaceType.Long;

    [Header("Slippery Settings")]
    public float slipperySpeed = 0.2f;

    [Tooltip("1 = 向本地右边滑，-1 = 向本地左边滑")]
    public float slipperyDirection = 1f;
}