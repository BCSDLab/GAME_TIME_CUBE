using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalZaco : ShootingZaco
{
    protected override void Start()
    {
        base.Start();
        m_bulletPattern = GetComponent<DirectionalNormal>();
    }
}