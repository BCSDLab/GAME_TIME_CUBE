using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RadialMulti), typeof(AudioSource))]
public class LockOnExplodingBullet : Bullet
{
    public GameObject target = null;
    public float speed = 4f;
    [SerializeField]
    private GameObject m_lockOnMark = null;
    [SerializeField]
    private float m_lockOnDistance = 3f;
    [SerializeField]
    private float m_explodeDistance = 0.3f;
    [SerializeField]
    private float m_alpha = 200f;

    private BulletPattern m_bulletPattern;
    private AudioSource m_audioSource;
    private Rigidbody2D m_rigidbody;
    private bool m_isLocked = false;
    private Vector3 m_lockedPos;
    private GameObject m_lockOnMarkInst = null;
    private bool m_isExploding = false;

    void Awake()
    {
        m_bulletPattern = GetComponent<RadialMulti>();
        m_audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        m_audioSource.pitch = Random.Range(0.9f, 1.1f);
        m_isLocked = false;
        m_isExploding = false;
    }

    void FixedUpdate()
    {
        if (target == null || m_isExploding) return;
        Vector3 targetPos = (m_isLocked) ? m_lockedPos : target.transform.position;

        Vector2 direction = transform.position - targetPos;
        direction.Normalize();
        float crossZ = Vector3.Cross(direction, transform.right).z;

        m_rigidbody.velocity = -transform.right * speed;
        m_rigidbody.angularVelocity = -m_alpha * crossZ;

        if (!m_isLocked && Vector2.Distance(transform.position, targetPos) < m_lockOnDistance)
        {
            LockOn();
        }
        if (m_isLocked && Vector2.Distance(transform.position, m_lockedPos) < m_explodeDistance)
        {
            StartCoroutine("Explode");
        }
    }

    void LockOn()
    {
        m_isLocked = true;
        m_lockedPos = target.transform.position;
        m_lockOnMarkInst = Instantiate(m_lockOnMark, m_lockedPos, Quaternion.identity);
        StartCoroutine("PlayLockOnSFX");
    }

    IEnumerator PlayLockOnSFX()
    {
        for (int i = 0; i < 2; i++)
        {
            m_audioSource.Play();
            yield return new WaitForSeconds(0.1f);
        }
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

    IEnumerator Explode()
    {
        Destroy(m_lockOnMarkInst);
        m_bulletPattern.StartPattern();
        yield return new WaitForSeconds(0.01f);
        PoolManager.instance.PushToPool(gameObject);
    }

    void OnDisable()
    {
        Destroy(m_lockOnMarkInst);
    }
}
