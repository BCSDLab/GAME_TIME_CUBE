using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineBullet : Bullet
{
    public float curveSpeed;

    private float m_fTime = 0f;

    private Rigidbody2D m_rigidbody;
    private Vector3 m_direction;
    
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_direction = Vector3.zero;
    }

    void OnEnable()
    {
        m_fTime = 0f;
    }

    void FixedUpdate()
    {
        m_fTime += Time.deltaTime * curveSpeed;

        Vector3 vSin = new Vector3(Mathf.Sin(m_fTime), -Mathf.Sin(m_fTime), 0);
        Vector3 vLin = m_direction;
        transform.localPosition += (vSin + vLin) * Time.deltaTime;
    }
}