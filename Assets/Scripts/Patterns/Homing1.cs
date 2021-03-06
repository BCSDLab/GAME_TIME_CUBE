﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing1 : BulletPattern
{
    public int count = 1;
    public float delay = 0.5f;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            m_audioSource.PlayOneShot(m_ShotSFX, m_ShotSFXVolum);

            for (int i = 0; i < count; i++)
            {
                GameObject obj = PoolManager.instance.PopFromPool(bullet.name);
                obj.transform.position = m_spawnPos.position;
                obj.SetActive(true);             
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
