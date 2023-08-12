using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeActiveSelf : MonoBehaviour
{
    /// <summary>
    /// 生命周期
    /// </summary>
    [SerializeField]
    private float m_lifeTime = 3f;

    /// <summary>
    /// 是否摧毁or隐藏
    /// </summary>
    [SerializeField]
    private bool m_isDestroy;

    private WaitForSeconds m_waitForSeconds;

    void Awake()
    {
        m_waitForSeconds = new WaitForSeconds(m_lifeTime);
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(DeactiveSelfCor));
    }

    IEnumerator DeactiveSelfCor()
    {
        yield return m_waitForSeconds;

        if (m_isDestroy)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
