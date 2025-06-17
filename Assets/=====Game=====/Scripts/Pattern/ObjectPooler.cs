using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxPoolSize = 100; // Giới hạn tối đa của pool
    private List<GameObject> _pool;
    public GameObject _poolContainer;

    private void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError($"Prefab is null in {gameObject.name}");
            return;
        }

        _pool = new List<GameObject>();
        _poolContainer = new GameObject($"Pool - {prefab.name}");

        CreatePool();
    }

    public void CreatePool()
    {
        if (prefab == null) return;

        for (int i = 0; i < poolSize; i++)
        {
            _pool.Add(CreateInstance());
        }
    }

    public GameObject CreateInstance()
    {
        if (prefab == null) return null;

        GameObject instance = Instantiate(prefab);
        if (instance == null) return null;

        if (prefab.CompareTag("Enemy"))
        {
            _poolContainer.transform.position = new Vector3(-8.470572f, 3.28324f, 0f);
        }
        instance.transform.SetParent(_poolContainer.transform);
        instance.SetActive(false);
        return instance;
    }

    public GameObject GetInstanceFromPool()
    {
        // Kiểm tra và tạo instance mới nếu pool rỗng
        if (_pool == null || _pool.Count == 0)
        {
            return CreateInstance();
        }

        // Tìm instance không active
        for (int i = 0; i < _pool.Count; i++)
        {
            if (_pool[i] != null && !_pool[i].activeInHierarchy)
            {
                return _pool[i];
            }
        }

        // Nếu pool chưa đạt giới hạn, tạo thêm instance mới
        if (_pool.Count < maxPoolSize)
        {
            GameObject newInstance = CreateInstance();
            if (newInstance != null)
            {
                _pool.Add(newInstance);
                return newInstance;
            }
        }

        // Nếu đã đạt giới hạn, tái sử dụng instance đầu tiên
        if (_pool.Count > 0)
        {
            GameObject instance = _pool[0];
            _pool.RemoveAt(0);
            _pool.Add(instance);
            return instance;
        }

        return null;
    }

    public void ReturnToPool(GameObject instance)
    {
        if (instance == null) return;
        
        // Reset position and rotation
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        
        // Disable the instance
        instance.SetActive(false);
        
        // Add back to pool if not already in it
        if (!_pool.Contains(instance))
        {
            _pool.Add(instance);
        }
    }

    public static IEnumerator ReturnToPoolInHierarchy(GameObject instance, float delay)
    {
        if (instance == null) yield break;
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }
}
