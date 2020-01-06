using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bi_DirectionalZaco : ShootingZaco
{
    protected override void Start()
    {
        base.Start();
        m_bulletPattern = GetComponent<BiDirectional>();
    }
}
