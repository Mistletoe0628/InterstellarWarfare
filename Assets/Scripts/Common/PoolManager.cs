using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField]
    Pool[] m_projectilePools;

    private static Dictionary<GameObject, Pool> m_dicPools = new Dictionary<GameObject, Pool>();

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        foreach (var pool in m_projectilePools)
        {
            if (m_dicPools.ContainsKey(pool.Prefab))
            {
                continue;
            }

            //��ʼ�������ʱ�����úø����ڵ�
            var poolParent = new GameObject("Pool:" + pool.Prefab.name).transform;

            poolParent.parent = transform;
            pool.Initialize(poolParent);
            m_dicPools.Add(pool.Prefab, pool);
        }
    }

    /// <summary>
    /// ˵�����Ӷ�Ӧ�ĳ����ͷŶ���
    /// ���ߣ�wk
    /// ���ڣ�2023-08-09
    /// </summary>
    /// <param name="prefab">prefab</param>
    /// <param name="position">position</param>
    /// <param name="rotation">rotation</param>
    /// <param name="localScale">localScale</param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab,Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        if (m_dicPools.TryGetValue(prefab, out Pool pool))
        {
            return pool.PrepareGameObject(position, rotation, localScale);
        }

        return new GameObject();
    }

    /// <summary>
    /// ˵�����Ӷ�Ӧ�ĳ����ͷŶ���
    /// ���ߣ�wk
    /// ���ڣ�2023-08-09
    /// </summary>
    /// <param name="prefab">prefab</param>
    /// <param name="position">position</param>
    /// <param name="rotation">rotation</param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (m_dicPools.TryGetValue(prefab, out Pool pool))
        {
            return pool.PrepareGameObject(position, rotation);
        }

        return new GameObject();
    }

#if UNITY_EDITOR

    private void OnDestroy()
    {
        CheckPoolSize(m_projectilePools);
    }

#endif

    /// <summary>
    /// ˵�������ص�����
    /// ���ߣ�wk
    /// ���ڣ�2023-08-09
    /// </summary>
    /// <param name="pools">pools</param>
    /// <returns></returns>
    void CheckPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if (pool.RunTimeSize > pool.Size)
            {
                Debug.LogWarning($"Pool:{pool.Prefab.name} has a runtime size{pool.RunTimeSize} bigger than it initial size {pool.Size}");
            }
        }
    }
}
