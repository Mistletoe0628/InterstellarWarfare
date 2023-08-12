using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : BaseProjectile
{
    private TrailRenderer m_trail;

    private void Awake()
    {
        m_trail = this.GetComponentInChildren<TrailRenderer>();
    }

    private void OnDisable()
    {
        m_trail.Clear();
    }
}
