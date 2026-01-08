using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    Dictionary<ResourceType, int> resourceAmounts = new Dictionary<ResourceType, int>();

    public event Action<ResourceType, int, int> ResourceChanged; 
    // (type, delta, total)

    public int GetAmount(ResourceType type)
    {
        if (resourceAmounts.TryGetValue(type, out int value))
            return value;

        return 0;
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0)
            return;

        int current = GetAmount(type);
        int total = current + amount;
        resourceAmounts[type] = total;

        ResourceChanged?.Invoke(type, amount, total);
        Debug.Log($"Get Resource: {type} +{amount} (Total:{total})");
    }
}