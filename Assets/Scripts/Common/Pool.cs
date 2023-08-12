using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����δ�̳�monobehavivor,��ҪSystem.Serializable����
/// </summary>
[System.Serializable]
public class Pool
{
    [SerializeField]
    private GameObject m_prefab;

    /// <summary>
    /// ����ط���Ķ���
    /// </summary>
    public GameObject Prefab
    {
        get { return m_prefab; }
    }

    [SerializeField]
    int m_size = 1;

    /// <summary>
    /// ���Ƴص�����
    /// </summary>
    public int Size => m_size;

    /// <summary>
    /// ����ʱ��ʵ�ʵ�����
    /// </summary>
    public int RunTimeSize => m_queue.Count;

    /// <summary>
    /// ����
    /// </summary>
    private Queue<GameObject> m_queue;

    /// <summary>
    /// �س�ʼ��ʱ���õĸ�������
    /// </summary>
    private Transform m_parent;

    /// <summary>
    /// ˵������ʼ��
    /// ���ߣ�wk
    /// ���ڣ�2023-08-09
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
    /// ˵�������ؿ���������ӵ������ʹ��
    /// ���ߣ�wk
    /// ���ڣ�2023-08-09
    /// </summary>
    private GameObject Copy()
    {
        var copy = GameObject.Instantiate(m_prefab, m_parent);
        copy.SetActive(false);
        return copy;
    }

    /// <summary>
    /// ˵����ȡ������
    /// ���ߣ�wk
    /// ���ڣ�2023-08-09
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
    /// ˵�����������
    /// ���ߣ�wk
    /// ���ڣ�2023-08-09
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
    /// ˵�����������
    /// ���ߣ�wk
    /// ���ڣ�2023-08-09
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
