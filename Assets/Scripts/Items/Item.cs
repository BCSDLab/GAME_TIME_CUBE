using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer), typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public abstract class Item : MonoBehaviour
{
    protected bool m_isPickedUp = false;

    private const float FALLING_FORCE = 3f;
    private const float AUTO_ACHIEVE = 10f;
    private const float ALPHA = 4000f;

    private Rigidbody2D m_rigidbody;
    private SpriteRenderer m_spriteRenderer;
    private ParticleSystem m_particleSystem;
    private AudioSource m_audioSource;

    public GameObject target;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_particleSystem = GetComponent<ParticleSystem>();
        m_audioSource = GetComponent<AudioSource>();

        target = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
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

        if (GameManager.instance.isPlayerSpelling || GameManager.instance.isDialogueOn)
        {
            AutoAchieve();
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
        m_rigidbody.velocity = new Vector2(0, 0);

        Vector2 direction = transform.position - target.transform.position;
        direction.Normalize();
        float crossZ = Vector3.Cross(direction, transform.right).z;

        m_rigidbody.velocity = -transform.right * AUTO_ACHIEVE;
        m_rigidbody.angularVelocity = -ALPHA * crossZ;
    }

    protected abstract void PickUp();
}
