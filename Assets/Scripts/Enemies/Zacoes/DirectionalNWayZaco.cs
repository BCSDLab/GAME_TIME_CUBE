using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalNWayZaco : ShootingZaco
{
    protected override void Start()
    {
        base.Start();
        m_bulletPattern = GetComponent<DirectionalAimedNWay>();
    }
}
