using UnityEngine;

public class NeedleAimToTimingPointer : MonoBehaviour
{
    public Transform needle;
    public Transform needleVisual;
    public Transform timingPointer;
    public float needleRotationOffset = 0f;
    public bool stretchVisualToPointer = false;
    public float visualLengthMultiplier = 1f;

    private NeedleMover needleMover;
    private Vector3 visualStartScale;
    private Vector3 visualStartLocalPosition;

    private void Awake()
    {
        if (needle == null)
        {
            needle = transform;
        }

        if (needleVisual == null && needle != null)
        {
            needleVisual = needle.Find("NeedleVisual");
        }

        if (needle != null)
        {
            needleMover = needle.GetComponent<NeedleMover>();
        }

        if (needleVisual != null)
        {
            visualStartScale = needleVisual.localScale;
            visualStartLocalPosition = needleVisual.localPosition;
        }
    }

    private void LateUpdate()
    {
        if (needle == null || timingPointer == null)
        {
            return;
        }

        if (needleMover != null && needleMover.IsMoving)
        {
            return;
        }

        Vector3 directionToPointer = timingPointer.position - needle.position;

        if (directionToPointer.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(directionToPointer.y, directionToPointer.x) * Mathf.Rad2Deg;
        needle.rotation = Quaternion.Euler(0f, 0f, angle - 90f + needleRotationOffset);

        if (stretchVisualToPointer)
        {
            StretchVisual(directionToPointer.magnitude);
        }
    }

    private void StretchVisual(float targetLength)
    {
        if (needleVisual == null)
        {
            return;
        }

        Vector3 scale = visualStartScale;
        scale.y = targetLength * visualLengthMultiplier;
        needleVisual.localScale = scale;

        Vector3 localPosition = visualStartLocalPosition;
        localPosition.y = targetLength * 0.5f;
        needleVisual.localPosition = localPosition;
    }
}
