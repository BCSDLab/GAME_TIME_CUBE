using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AimedBulletPattern : BulletPattern
{
    public Transform target;

    protected void OnEnable()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
