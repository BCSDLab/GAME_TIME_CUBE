using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer), typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public abstract class Item : MonoBehaviour
{
    private const float FALLING_FORCE = 3f;
    private const float AUTO_ACHIEVE_SPEED = 10f;

    private Rigidbody2D m_rigidbody;
    private SpriteRenderer m_spriteRenderer;
    private ParticleSystem m_particleSystem;
    private AudioSource m_audioSource;

    private Transform m_target = null;
    private bool m_isPickedUp = false;
    private bool m_isAutoAchieved = false;


    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_particleSystem = GetComponent<ParticleSystem>();
        m_audioSource = GetComponent<AudioSource>();

        m_target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (m_isAutoAchieved)
        {
            AutoAchieve();
            return;
        }

        // 화면 왼쪽으로 움직임
        m_rigidbody.AddForce(Vector2.left * FALLING_FORCE);
        if (m_rigidbody.velocity.x < -3f)
        {
            m_rigidbody.AddForce(Vector2.right * FALLING_FORCE);
        }
        if (m_rigidbody.velocity.y < -2f)
        {
            m_rigidbody.AddForce(Vector2.up);
        }
        else if (m_rigidbody.velocity.y > 2f)
        {
            m_rigidbody.AddForce(Vector2.down);
        }

        if (!m_isAutoAchieved && GameManager.instance.isPlayerSpelling || GameManager.instance.isDialogueOn)
        {
            m_isAutoAchieved = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_isPickedUp && collision.CompareTag("PlayerSprite"))
        {
            m_isPickedUp = true;

            PickUp();
            m_spriteRenderer.enabled = false;
            m_particleSystem.Play();
            m_audioSource.Play();

            float duration = m_particleSystem.main.duration + m_particleSystem.main.startLifetime.constant;
            Destroy(gameObject, duration);
        }
    }

    void AutoAchieve()
    {
        if (m_target == null) return;

        Vector2 direction = m_target.position - transform.position;
        float atan = Mathf.Atan2(direction.y, direction.x);
        float cos = Mathf.Cos(atan);
        float sin = Mathf.Sin(atan);
        m_rigidbody.velocity = new Vector2(AUTO_ACHIEVE_SPEED * cos, AUTO_ACHIEVE_SPEED * sin);
    }

    protected abstract void PickUp();
}
