using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToXBullet : Bullet
{
    public float speed = 5f;
    [SerializeField]
    private float[] m_toXes;
    [SerializeField]
    private float m_minY = -4.7f;
    [SerializeField]
    private float m_maxY = 3.9f;

    private Rigidbody2D m_rigidbody;
    private bool m_isMoving = false;
    private int xIdx = 0;
    private bool m_hasReachedDestination = false;
    private Vector3 m_nextPos;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        m_isMoving = false;
        xIdx = 0;
        m_hasReachedDestination = false;
        m_nextPos = Vector3.right * m_toXes[xIdx] + Vector3.up * Random.Range(m_minY, m_maxY);
    }

    void FixedUpdate()
    {
        if (!m_isMoving)
        {
            MoveTo(m_nextPos);
            m_isMoving = true;
        }
        if (m_hasReachedDestination) return;

        float distance = Mathf.Abs(m_toXes[xIdx] - transform.position.x);
        if (distance < 0.01f)
        {
            xIdx++;
            if (xIdx >= m_toXes.Length)
            {
                m_hasReachedDestination = true;
                return;
            }
            m_nextPos = Vector3.right * m_toXes[xIdx] + Vector3.up * Random.Range(m_minY, m_maxY);
            MoveTo(m_nextPos);
        }
    }

    void MoveTo(Vector2 pos)
    {
        Vector2 direction = pos - (Vector2)transform.position;
        float atan = Mathf.Atan2(direction.y, direction.x);
        float cos = Mathf.Cos(atan);
        float sin = Mathf.Sin(atan);
        float angle = atan * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        m_rigidbody.velocity = new Vector2(speed * cos, speed * sin);
        transform.rotation = rotation;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            speed *= collision.GetComponent<TimeControlArea>().velocityMultiplier;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            speed *= collision.GetComponent<TimeControlArea>().reverseMultiplier;
        }
    }
}
