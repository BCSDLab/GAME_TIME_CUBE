﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutZaco : ShootingZaco
{
    protected override void Start()
    {
        base.Start();
        m_bulletPattern = GetComponent<DonutAimed>();
    }
}
