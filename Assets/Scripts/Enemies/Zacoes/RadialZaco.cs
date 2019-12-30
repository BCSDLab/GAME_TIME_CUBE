using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialZaco : ShootingZaco
{
    protected override void Start()
    {
        base.Start();
        m_bulletPattern = GetComponent<RadialMulti>();
    }
}
