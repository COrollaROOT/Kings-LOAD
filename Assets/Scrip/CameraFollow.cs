using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;

    [Header("Follow")]
    [SerializeField] Vector3 offset = new Vector3(0f, 10f, -10f);
    [SerializeField] float followSmoothTime = 0.12f;

    [Header("Rotation")]
    [SerializeField] bool canLookAtTarget = true;
    [SerializeField] bool canRotateWithTarget = false;
    [SerializeField] float rotationSmoothSpeed = 12f;

    Vector3 velocity;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredOffset = offset;

        if (canRotateWithTarget)
            desiredOffset = target.rotation * offset;

        Vector3 desiredPosition = target.position + desiredOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            followSmoothTime
        );

        if (canLookAtTarget)
        {
            Vector3 lookDir = target.position - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSmoothSpeed * Time.deltaTime
                );
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}