using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 타깃 주변의 특정 위치를 사수하는 보조 장비
public class Follower : PlayerSubWeapon
{
    [SerializeField][Tooltip("타깃으로부터의 변위 (얼마나 떨어져 있는가?)")]
    private Vector2 m_positionOffset = new Vector2(-1f, -1f);
    [SerializeField][Tooltip("부드럽게 이동하는 정도")]
    private float m_smoothTime = 0.2f;

    private Vector2 m_velocity = Vector2.zero;
    private float m_baseSmoothTime;

    void Start()
    {
        m_baseSmoothTime = m_smoothTime;
    }

    void FixedUpdate()
    {
        Vector2 targetPos = (Vector2)(m_target.position) + m_positionOffset;
        transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref m_velocity, m_smoothTime);
    }

    public override void MultiplySpeed(float velocityMultiplier)
    {
        m_smoothTime *= 2f;
    }

    public override void ResetSpeed()
    {
        m_smoothTime = m_baseSmoothTime;
    }
}
