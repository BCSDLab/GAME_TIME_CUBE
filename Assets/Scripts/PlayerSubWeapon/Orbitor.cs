using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 타깃 주위를 공전하는 보조 장비
public class Orbitor : PlayerSubWeapon
{
    [SerializeField][Tooltip("각속도")]
    private float m_omega = 2f;
    [SerializeField][Tooltip("긴 축")]
    private readonly float m_semiMajor = 2f;
    [SerializeField][Tooltip("짧은 축")]
    private readonly float m_semiMinor = 2f;
    [SerializeField][Tooltip("부드럽게 이동하는 정도")]
    private float m_smoothTime = 0.2f;

    private float m_alpha = 0f;  // 현재 각도
    private Vector2 m_velocity = Vector2.zero;
    private float m_baseOmega;
    private float m_baseSmoothTime;

    void Start()
    {
        m_baseOmega = m_omega;
        m_baseSmoothTime = m_smoothTime;
    }

    void FixedUpdate()
    {
        Vector2 targetPos = new Vector2(m_target.position.x + (m_semiMajor * Mathf.Sin(Mathf.Deg2Rad * m_alpha)), m_target.position.y + (m_semiMinor * Mathf.Cos(Mathf.Deg2Rad * m_alpha)));
        transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref m_velocity, m_smoothTime);

        m_alpha += m_omega;
        if (m_alpha > 360000f) { m_alpha %= 360f; }
    }

    public override void MultiplySpeed(float velocityMultiplier)
    {
        m_omega *= velocityMultiplier;
        m_smoothTime *= 2f;
    }

    public override void ResetSpeed()
    {
        m_omega = m_baseOmega;
        m_smoothTime = m_baseSmoothTime;
    }
}
