using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;
    private List<GameObject> _pool;
    public GameObject _poolContainer;


    private void Awake()
    {
        _pool = new List<GameObject>();
        _poolContainer = new GameObject($"Pool - {prefab.name}");

        CreatePool();
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
        GameObject instance = Instantiate(prefab);
        if (prefab.CompareTag("Enemy"))
        {
            _poolContainer.transform.position = new Vector3(-9.8f, 6f, 0f);
        }
        instance.transform.SetParent(_poolContainer.transform);
        instance.SetActive(false);
        return instance;
    }
    public GameObject GetInstanceFromPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (!_pool[i].activeInHierarchy)
            {
                return _pool[i];
            }
        }
        return CreateInstance();
    }


    public void ReturnToPool(GameObject instance)
    {
        instance.SetActive(false);
    }

    public static IEnumerator ReturnToPoolInHierarchy(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }
}
