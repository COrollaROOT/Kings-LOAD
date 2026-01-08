using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    static PoolManager instance;
    public static PoolManager Instance => instance;

    Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
    Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
            return null;

        if (!pools.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            pools[prefab] = pool;
        }

        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
            instanceToPrefab[obj] = prefab;
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        if (obj == null)
            return;

        if (!instanceToPrefab.TryGetValue(obj, out GameObject prefab))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        pools[prefab].Enqueue(obj);
    }
}