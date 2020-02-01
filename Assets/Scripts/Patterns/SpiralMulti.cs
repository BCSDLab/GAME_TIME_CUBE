﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralMulti : BulletPattern
{
    public int count = 3;
    public float speed = 3f;
    public float inDelay = 0.1f;
    [SerializeField]
    [Tooltip("초기 발사각")]
    private float m_angle = 0f;
    [SerializeField]
    [Tooltip("각속도")]
    private float m_omega = 10f;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_angle *= Mathf.Deg2Rad;
        m_omega *= Mathf.Deg2Rad;
    }

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            m_audioSource.Play();

            for (int i = 0; i < count; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_spawnPos.position;
                bulletInst.SetActive(true);

                float a = 2f * Mathf.PI * i / count;
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(a + m_angle), speed * Mathf.Sin(a + m_angle));
                //obj.transform.Rotate(new Vector3(0f, 0f, 360f * i / SpiralShooting - 90f));

                m_angle += m_omega;
                //TODO: 오버플로 예방
            }

            yield return new WaitForSeconds(inDelay);
        }
    }
}