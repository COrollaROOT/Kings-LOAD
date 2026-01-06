using System;
using UnityEngine;

public class InteractionSensor : MonoBehaviour
{
    [Header("Detect")]
    [SerializeField] float detectRadius = 2f;
    [SerializeField] float detectAngle = 140f;
    [SerializeField] LayerMask interactableMask;

    [Header("Optional Line Of Sight")]
    [SerializeField] bool canUseLineOfSight = false;
    [SerializeField] LayerMask obstacleMask;

    [Header("Perf")]
    [SerializeField] int maxCandidates = 16;

    Collider[] results;
    IInteractable currentTarget;

    public IInteractable CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null;

    public event Action<IInteractable> TargetChanged;

    void Awake()
    {
        results = new Collider[Mathf.Max(1, maxCandidates)];
    }

    void Update()
    {
        RefreshTarget();
    }

    public bool TryInteract(PlayerController controller)
    {
        if (currentTarget == null)
            return false;

        if (!currentTarget.CanInteract(controller))
            return false;

        currentTarget.Interact(controller);
        return true;
    }

    void RefreshTarget()
    {
        IInteractable best = FindBestTarget();

        if (best != currentTarget)
        {
            currentTarget = best;
            TargetChanged?.Invoke(currentTarget);
        }
    }

    IInteractable FindBestTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            detectRadius,
            results,
            interactableMask,
            QueryTriggerInteraction.Collide
        );

        float bestScore = float.MaxValue;
        IInteractable best = null;

        Vector3 origin = transform.position;
        origin.y = 0f;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;
        forward.Normalize();

        for (int i = 0; i < count; i++)
        {
            Collider col = results[i];
            if (col == null)
                continue;

            IInteractable candidate = col.GetComponent<IInteractable>();
            if (candidate == null)
                candidate = col.GetComponentInParent<IInteractable>();

            if (candidate == null)
                continue;

            Vector3 targetPos = col.bounds.center;
            targetPos.y = 0f;

            Vector3 to = targetPos - origin;
            float distance = to.magnitude;
            if (distance <= 0.001f)
                continue;

            Vector3 toDir = to / distance;

            float angle = Vector3.Angle(forward, toDir);
            if (angle > detectAngle * 0.5f)
                continue;

            if (canUseLineOfSight && !HasLineOfSight(col))
                continue;

            float score = distance + (angle * 0.02f);
            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    bool HasLineOfSight(Collider col)
    {
        Vector3 from = transform.position + Vector3.up * 0.6f;
        Vector3 to = col.bounds.center + Vector3.up * 0.1f;

        Vector3 dir = to - from;
        float dist = dir.magnitude;
        if (dist <= 0.001f)
            return true;

        dir /= dist;

        return !Physics.Raycast(from, dir, dist, obstacleMask, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
