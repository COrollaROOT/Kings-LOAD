using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    [SerializeField] ResourceType resourceType;
    [SerializeField] int amount = 1;
    [SerializeField] float lifeTime = 12f;

    float remainingLifeTime;
    bool hasCollected;

    void OnEnable()
    {
        remainingLifeTime = lifeTime;
        hasCollected = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        remainingLifeTime -= Time.deltaTime;
        if (remainingLifeTime <= 0f)
            ReturnToPool();
    }

    public void Initialize(ResourceType type, int newAmount)
    {
        resourceType = type;
        amount = Mathf.Max(1, newAmount);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasCollected)
            return;

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
        if (inventory == null)
            return;

        hasCollected = true;
        inventory.AddResource(resourceType, amount); // ✅ “획득 이벤트”
        ReturnToPool();
    }

    void ReturnToPool()
    {
        if (PoolManager.Instance != null)
            PoolManager.Instance.Despawn(gameObject);
        else
            Destroy(gameObject);
    }
}