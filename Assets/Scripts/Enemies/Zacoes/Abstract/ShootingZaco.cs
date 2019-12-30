using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class ShootingZaco : ZacoController
{
    protected BulletPattern m_bulletPattern = null;

    void OnBecameVisible()
    {
        m_isInvincible = false;

        if (m_bulletPattern != null)
        {
            m_bulletPattern.StartPattern();
        }
    }
}
