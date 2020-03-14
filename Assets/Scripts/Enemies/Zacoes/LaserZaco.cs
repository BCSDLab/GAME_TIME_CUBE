using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Laser))]
public class LaserZaco : Zaco
{
    Pattern m_pattern;

    protected override void Start()
    {
        base.Start();
        m_pattern = GetComponent<Laser>();
    }

    void OnBecameVisible()
    {
        if (!m_isZacoInvincible) m_isInvincible = false;

        if (m_pattern != null)
        {
            m_pattern.StartPattern();
        }
    }
}
