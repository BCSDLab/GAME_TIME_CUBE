using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBullet : Bullet
{
    [Tooltip("이동하는 동안 피격 판정 존재 여부")]
    [SerializeField]
    private bool m_hitDuringMove = false;
    [Tooltip("뒤로 이동")]
    [SerializeField]
    private bool m_moveBackward = false;
    [Tooltip("Move Backward가 false일 때만 지정")]
    [SerializeField]
    private Vector2 m_moveDirection;
    [SerializeField]
    private float m_moveSpeed = 1f;
    [Tooltip("다른 방향으로 움직이기 시작하는 타이밍")]
    [SerializeField]
    private float m_moveTime = 3f;
    [SerializeField]
    private float m_moveDuration = 1.5f;

    private Rigidbody2D m_rigidbody;
    private Collider2D m_collider;
    private SpriteRenderer m_spriteRenderer = null;

    private bool m_isInCubeArea = false;
    private float m_cubeReverseMultiplier;
    private bool m_isMoving = false;
    private float m_time = 0f;
    private float m_speed;
    private Color m_transparentColor;
    private Color m_originalColor;
    private Vector2 m_originalDirection;

    void OnEnable()
    {
        m_isInCubeArea = false;
        m_isMoving = false;
        m_time = 0f;
        if (m_spriteRenderer != null) m_spriteRenderer.color = m_originalColor;
    }

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_originalColor = m_spriteRenderer.color;
        m_transparentColor = m_originalColor;
        m_transparentColor.a = 0.3f;
    }

    void FixedUpdate()
    {
        m_time += Time.deltaTime;

        if (!m_isMoving && m_moveTime < m_time)
        {
            Move();
        }
        else if (m_isMoving && m_moveDuration < m_time)
        {
            MoveForward();
        }
    }

    void Move()
    {
        m_time = 0f;
        m_isMoving = true;

        // 원래 방향과 속도 저장
        m_originalDirection = m_rigidbody.velocity.normalized;
        m_speed = m_rigidbody.velocity.magnitude;
        if (m_isInCubeArea)
            m_speed *= m_cubeReverseMultiplier;

        // 지정한 방향으로 이동
        if (m_moveBackward)
            m_rigidbody.velocity = -m_originalDirection * m_moveSpeed;
        else
            m_rigidbody.velocity = m_moveDirection.normalized * m_moveSpeed;

        if (!m_hitDuringMove)
        {
            m_collider.enabled = false;
            m_spriteRenderer.color = m_transparentColor;
        }
    }

    void MoveForward()
    {
        m_time = 0f;
        m_isMoving = false;

        // 원래 방향으로 진행
        m_rigidbody.velocity = m_originalDirection * m_speed;

        if (!m_hitDuringMove)
        {
            m_collider.enabled = true;
            m_spriteRenderer.color = m_originalColor;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            m_isInCubeArea = true;
            m_cubeReverseMultiplier = collision.GetComponent<TimeControlArea>().reverseMultiplier;

            if (m_isMoving)
                m_rigidbody.velocity *= collision.GetComponent<TimeControlArea>().reverseMultiplier;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            m_isInCubeArea = false;

            if (m_isMoving)
                m_rigidbody.velocity *= collision.GetComponent<TimeControlArea>().velocityMultiplier;
        }
    }
}
