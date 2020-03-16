using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인터페이스를 활용했다면...
public class TraceHomingBullet : Bullet
{
    [Header("Homing")]
    public GameObject target;
    public float speed = 4f;
    [SerializeField]
    private float m_alpha =100f;
    [SerializeField]
    private int m_hp = 100;
    [SerializeField]
    private float m_lifeTime = 15f;
    [Header("Trace")]
    [SerializeField]
    private float m_traceTime = 1f;
    [SerializeField]
    private float m_traceLifeTime = 3f;
    [SerializeField]
    private GameObject m_traceBullet = null;

    private Rigidbody2D m_rigidbody;
    private AudioSource m_audioSource;
    private float m_time = 0f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        m_time = 0f;
    }

    void FixedUpdate()
    {
        m_time += Time.deltaTime;

        if (m_lifeTime < m_time)
        {
            Blow();
            PoolManager.instance.PushToPool(gameObject);
        }

        if (m_traceTime < m_time)
        {
            LeaveTrace();
        }

        if (target == null) return;

        Vector2 direction = transform.position - target.transform.position;
        direction.Normalize();
        float crossZ = Vector3.Cross(direction, transform.right).z;

        m_rigidbody.velocity = -transform.right * speed;
        m_rigidbody.angularVelocity = -m_alpha * crossZ;
    }

    void LeaveTrace()
    {
        m_time = 0f;

        GameObject bulletInst = PoolManager.instance.PopFromPool(m_traceBullet.name);
        bulletInst.transform.position = transform.position;
        bulletInst.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            return;
            m_hp -= collision.GetComponent<PlayerBullet>().damage;
            m_audioSource.Play();
            
            PoolManager.instance.PushToPool(collision.gameObject);
            Blow();
            if (m_hp <= 0)
            {
                PoolManager.instance.PushToPool(gameObject);
            }
        }
        else if (collision.CompareTag("TimeControlArea"))
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
