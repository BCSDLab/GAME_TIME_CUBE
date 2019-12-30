using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineBullet : Bullet
{
    public float CurveSpeed;
    public float fTime = 2;

    private Rigidbody2D m_rigidbody;
    private Vector3 m_direction;
    private Vector3 m_orthogonal;
    
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_direction = m_rigidbody.velocity;
    }
    
    void FixedUpdate()
    {
        fTime += Time.deltaTime * CurveSpeed;

        Vector3 vSin = new Vector3(Mathf.Sin(fTime), -Mathf.Sin(fTime), 0);
        Vector3 vLin = m_direction;
        transform.localPosition += (vSin + vLin) * Time.deltaTime;
    }
}