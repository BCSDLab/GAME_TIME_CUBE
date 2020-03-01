using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExplodingHomingBullet : Bullet
{
    public GameObject target = null;
    public float speed = 5f;
    [SerializeField]
    private float m_explodeDistance = 3f;
    [SerializeField]
    private float m_alpha = 200f;
    //[SerializeField]
    //private int m_hp = 70;

    private BulletPattern m_bulletPattern;
    private SpriteRenderer m_spriteRenderer;
    private Rigidbody2D m_rigidbody;
    private Collider2D m_collider;
    private AudioSource m_explosionAudio;

    private bool m_isExploding = false;

    void Awake()
    {
        m_bulletPattern = GetComponent<RadialMulti>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        m_explosionAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        m_spriteRenderer.enabled = true;
        m_collider.enabled = true;
        m_isExploding = false;
    }

    void FixedUpdate()
    {
        if (target == null) return;
        if (m_isExploding) return;

        Vector2 direction = transform.position - target.transform.position;
        direction.Normalize();
        float crossZ = Vector3.Cross(direction, transform.right).z;

        m_rigidbody.velocity = -transform.right * speed;
        m_rigidbody.angularVelocity = -m_alpha * crossZ;

        if (Vector2.Distance(transform.position, target.transform.position) < m_explodeDistance)
        {
            StartCoroutine("Explode");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            speed *= collision.GetComponent<TimeControlArea>().velocityMultiplier;
        }
        //else if (collision.CompareTag("PlayerBullet"))
        //{
        //    m_hp -= collision.GetComponent<PlayerBullet>().damage;
        //    PoolManager.instance.PushToPool(collision.gameObject);

        //    if (m_hp <= 0)
        //    {
        //        PoolManager.instance.PushToPool(gameObject);
        //    }
        //}
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
        m_isExploding = true;
        StopAllCoroutines();
        m_collider.enabled = true;
        m_spriteRenderer.enabled = false;
        m_bulletPattern.StartPattern();
        m_explosionAudio.Play();
        yield return new WaitForSeconds(m_explosionAudio.clip.length + 0.1f);
        PoolManager.instance.PushToPool(gameObject);
    }
}
