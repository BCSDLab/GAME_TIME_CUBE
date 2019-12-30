using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutZaco : ShootingZaco
{
    private Cross m_cross;

    protected override void Start()
    {
        base.Start();
        m_bulletPattern = GetComponent<DonutAimed>();
        m_cross = GetComponent<Cross>();
    }

    public override void Die()
    {
        m_cross.StartPattern();

        base.Die();
    }
}
