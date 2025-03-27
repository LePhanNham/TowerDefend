using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;
    private List<GameObject> _pool;
    private GameObject _poolContainer;
    private Waypoint _waypoint;


    private void Awake()
    {
        _pool = new List<GameObject>();
        _poolContainer = new GameObject($"Pool - {prefab.name}");
        _waypoint = GetComponent<Waypoint>();
        CreatePool();
    }
    private void Start()
    {
        _poolContainer.transform.position = new Vector3(-9.8f, 6, 0);
    }
    public void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            _pool.Add(CreateInstance());
        }
    }
    public GameObject CreateInstance()
    {
        GameObject instance = Instantiate( prefab );
        instance.transform.SetParent(_poolContainer.transform);
        instance.SetActive(false);
        return instance;
    }
    public GameObject GetInstanceFromPool()
    {
        for (int i = 0;i < poolSize;i++)
        {
            if (!_pool[i].activeInHierarchy) 
            {
                return _pool[i];
            }
        }
        return CreateInstance();
    }


    public static void ReturnToPool(GameObject instance)
    {
        instance.SetActive(true);
    }

    public static IEnumerator ReturnToPoolInHierarchy(GameObject instance,float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }
}
