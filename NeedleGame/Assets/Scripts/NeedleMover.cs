using UnityEngine;

public class NeedleMover : MonoBehaviour
{
    public Transform testStartPoint;
    public Transform testTargetPoint;
    public float moveSpeed = 5f;
    public bool enableTestInput = false;
    public float needleRotationOffset = 0f;

    private Vector3 targetPosition;
    private bool isMoving;

    public bool IsMoving
    {
        get { return isMoving; }
    }

    private void Start()
    {
        if (testStartPoint != null)
        {
            transform.position = testStartPoint.position;
        }
    }

    private void Update()
    {
        if (enableTestInput && Input.GetKeyDown(KeyCode.Space) && testTargetPoint != null)
        {
            MoveTo(testTargetPoint.position);
        }

        if (!isMoving)
        {
            return;
        }

        FaceTarget(targetPosition);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }

    public void MoveTo(Vector3 worldPosition)
    {
        targetPosition = worldPosition;
        isMoving = true;
        FaceTarget(targetPosition);
    }

    public void FaceTarget(Vector3 worldPosition)
    {
        FaceDirection(worldPosition - transform.position);
    }

    public void FaceDirection(Vector3 worldDirection)
    {
        if (worldDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(worldDirection.y, worldDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f + needleRotationOffset);
    }
}
