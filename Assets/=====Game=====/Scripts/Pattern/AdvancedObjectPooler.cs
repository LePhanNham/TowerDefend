using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedObjectPooler : Singleton<AdvancedObjectPooler> 
{
    [System.Serializable]
    public class PoolConfig
    {
        public string poolID; 
        public GameObject prefab;
        public int initialSize = 10;
        public bool expandable = true; 
        public Transform customParent; 
    }


    [SerializeField] private List<PoolConfig> poolConfigs = new List<PoolConfig>();
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, PoolConfig> configLookup;
    private Dictionary<string, Transform> poolContainers;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject); 
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        configLookup = new Dictionary<string, PoolConfig>();
        poolContainers = new Dictionary<string, Transform>();

        foreach (var config in poolConfigs)
        {
            CreatePool(config);
        }
    }

    private void CreatePool(PoolConfig config)
    {
        if (poolDictionary.ContainsKey(config.poolID))
        {
            Debug.LogWarning($"Pool with ID {config.poolID} already exists!");
            return;
        }

        var container = config.customParent != null ? config.customParent :
            new GameObject($"Pool_{config.poolID}").transform;
        container.SetParent(this.transform);
        poolContainers[config.poolID] = container;

        var objectQueue = new Queue<GameObject>();

        for (int i = 0; i < config.initialSize; i++)
        {
            var newObj = CreateNewObject(config.poolID, container);
            objectQueue.Enqueue(newObj);
        }

        poolDictionary.Add(config.poolID, objectQueue);
        configLookup.Add(config.poolID, config);
    }

    private GameObject CreateNewObject(string poolID, Transform parent)
    {
        if (!configLookup.ContainsKey(poolID))
        {
            Debug.LogError($"No config found for pool ID: {poolID}");
            return null;
        }

        var config = configLookup[poolID];
        var obj = Instantiate(config.prefab, parent);
        obj.SetActive(false);

        var returnToPool = obj.AddComponent<ReturnToPool>();
        returnToPool.poolID = poolID;

        return obj;
    }

    public GameObject GetFromPool(string poolID, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolID))
        {
            Debug.LogError($"Pool with ID {poolID} doesn't exist!");
            return null;
        }

        var pool = poolDictionary[poolID];
        var config = configLookup[poolID];
        if (pool.Count == 0 && config.expandable)
        {
            var newObj = CreateNewObject(poolID, poolContainers[poolID]);
            pool.Enqueue(newObj);

            config.initialSize++;
            Debug.Log($"Expanded pool {poolID} to size {config.initialSize}");
        }
        else if (pool.Count == 0)
        {
            Debug.LogWarning($"Pool {poolID} is empty and not expandable!");
            return null;
        }

        var obj = pool.Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnSpawnFromPool();

        return obj;
    }

    public void ReturnToPool(string poolID, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(poolID))
        {
            Debug.LogError($"Pool with ID {poolID} doesn't exist!");
            return;
        }

        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnReturnToPool();

        obj.SetActive(false);
        obj.transform.SetParent(poolContainers[poolID]);
        poolDictionary[poolID].Enqueue(obj);
    }

    public static IEnumerator ReturnToPoolWithDelay(string poolID, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Instance.ReturnToPool(poolID, obj);
    }

    public void AddNewPoolAtRuntime(PoolConfig newConfig)
    {
        if (poolDictionary.ContainsKey(newConfig.poolID))
        {
            Debug.LogWarning($"Pool ID {newConfig.poolID} already exists!");
            return;
        }

        poolConfigs.Add(newConfig);
        CreatePool(newConfig);
    }
}
public interface IPoolable
{
    void OnSpawnFromPool();
    void OnReturnToPool();
}

// Component tự động trả về pool
public class ReturnToPool : MonoBehaviour
{
    public string poolID;

    private void OnDisable()
    {
        if (AdvancedObjectPooler.Instance != null && !string.IsNullOrEmpty(poolID))
        {
            AdvancedObjectPooler.Instance.ReturnToPool(poolID, gameObject);
        }
    }
}