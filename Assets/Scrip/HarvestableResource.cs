using System;
using UnityEngine;

public class HarvestableResource : MonoBehaviour, IInteractable
{
    [Header("Interact")]
    [SerializeField] string promptText = "채집하기 (E)";
    [SerializeField] bool canHarvest = true;

    [Header("Drop")]
    [SerializeField] ResourceType resourceType;
    [SerializeField] GameObject dropPrefab;
    [SerializeField] int dropCountMin = 1;
    [SerializeField] int dropCountMax = 3;
    [SerializeField] float dropScatterRadius = 0.6f;
    [SerializeField] float dropUpForce = 2.2f;

    public string PromptText => promptText;

    public event Action<ResourceType, int> Dropped; 
    // ✅ “자원 생성(드랍) 이벤트”: (type, count)

    public bool CanInteract(PlayerController controller)
    {
        return canHarvest && dropPrefab != null;
    }

    public void Interact(PlayerController controller)
    {
        if (!CanInteract(controller))
            return;

        DoHarvest();
    }

    void DoHarvest()
    {
        canHarvest = false;

        int dropCount = UnityEngine.Random.Range(dropCountMin, dropCountMax + 1);
        Dropped?.Invoke(resourceType, dropCount); // ✅ “생성 이벤트”

        for (int i = 0; i < dropCount; i++)
        {
            Vector3 pos = transform.position + GetRandomScatter();
            GameObject drop = PoolManager.Instance.Spawn(dropPrefab, pos, Quaternion.identity);

            ResourcePickup pickup = drop.GetComponent<ResourcePickup>();
            if (pickup != null)
                pickup.Initialize(resourceType, 1);

            AddDropForce(drop);
        }

        // (기초) 한번 채집하면 끝. 나중에 respawnTime 붙이면 재생성 가능.
        gameObject.SetActive(false);
    }

    Vector3 GetRandomScatter()
    {
        Vector2 circle = UnityEngine.Random.insideUnitCircle * dropScatterRadius;
        return new Vector3(circle.x, 0.2f, circle.y);
    }

    void AddDropForce(GameObject drop)
    {
        Rigidbody rb = drop.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        Vector3 dir = UnityEngine.Random.onUnitSphere;
        dir.y = Mathf.Abs(dir.y) + 0.2f;
        dir.Normalize();

        rb.AddForce(dir * dropUpForce, ForceMode.Impulse);
    }
}
