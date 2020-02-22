using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalNWayZaco : ShootingZaco
{
    private Cross m_cross;
    protected override void Start()
    {
        base.Start();
        m_bulletPattern = GetComponent<DirectionalAimedNWay>();
        m_cross = GetComponent<Cross>();
    }
}
