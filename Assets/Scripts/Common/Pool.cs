using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 由于未继承monobehavivor,需要System.Serializable特性
/// </summary>
[System.Serializable]
public class Pool
{
    [SerializeField]
    private GameObject m_prefab;

    /// <summary>
    /// 对象池服务的对象
    /// </summary>
    public GameObject Prefab
    {
        get { return m_prefab; }
    }

    [SerializeField]
    int m_size = 1;

    /// <summary>
    /// 控制池的容量
    /// </summary>
    public int Size => m_size;

    /// <summary>
    /// 运行时，实际的容量
    /// </summary>
    public int RunTimeSize => m_queue.Count;

    /// <summary>
    /// 队列
    /// </summary>
    private Queue<GameObject> m_queue;

    /// <summary>
    /// 池初始化时设置的父级对象
    /// </summary>
    private Transform m_parent;

    /// <summary>
    /// 说明：初始化
    /// 作者：wk
    /// 日期：2023-08-09
    /// </summary>
    public void Initialize(Transform parent)
    {
        m_queue = new Queue<GameObject>();

        m_parent = parent;
        for (int i = 0; i < m_size; i++)
        {
            m_queue.Enqueue(Copy());
        }
    }

    /// <summary>
    /// 说明：返回拷贝对象，添加到对象池使用
    /// 作者：wk
    /// 日期：2023-08-09
    /// </summary>
    private GameObject Copy()
    {
        var copy = GameObject.Instantiate(m_prefab, m_parent);
        copy.SetActive(false);
        return copy;
    }

    /// <summary>
    /// 说明：取出对象
    /// 作者：wk
    /// 日期：2023-08-09
    /// </summary>
    /// <returns></returns>
    private GameObject AvailabeObject()
    {
        GameObject obj = null;
        if (m_queue.Count > 0 && !m_queue.Peek().activeSelf)
        {
            obj = m_queue.Dequeue();
        }
        else
        {
            obj = Copy();
        }

        m_queue.Enqueue(obj);
        return obj;
    }

    /// <summary>
    /// 说明：激活对象
    /// 作者：wk
    /// 日期：2023-08-09
    /// </summary>
    /// <returns></returns>
    public GameObject PrepareGameObject()
    {
        var gameObject = AvailabeObject();

        gameObject.SetActive(true);
        return gameObject;
    }

    public GameObject PrepareGameObject(Vector3 position, Quaternion rotation)
    {
        var gameObject = AvailabeObject();

        gameObject.SetActive(true);
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        return gameObject;
    }

    /// <summary>
    /// 说明：激活对象
    /// 作者：wk
    /// 日期：2023-08-09
    /// </summary>
    /// <returns></returns>
    public GameObject PrepareGameObject(Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        var gameObject = AvailabeObject();

        gameObject.SetActive(true);
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        gameObject.transform.localScale = localScale;
        return gameObject;
    }
}
