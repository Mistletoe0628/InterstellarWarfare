using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_movementDirection;

    [SerializeField]
    private float m_moveSpeed = 10f;

    private void OnEnable()
    {
        StartCoroutine(MoveDirectly());
    }

    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            transform.Translate(m_movementDirection * m_moveSpeed * Time.fixedDeltaTime);
            yield return null;
        }
    }
}
