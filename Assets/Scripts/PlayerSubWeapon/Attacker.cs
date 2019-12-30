using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어와 함께 공격하는 보조 장비 (추상)
public abstract class Attacker : MonoBehaviour
{
    public GameObject bullet;
    public float attackDelay = 0.1f;
    public float attackDelayRandomRange = 0f;

    protected bool m_hasAttacked = false;
    protected float m_attackTimer = 0f;
    
    private AudioSource m_audioSource = null;

    private GameManager m_gameManager;

    void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        m_gameManager = GameManager.instance;
    }
    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if (m_gameManager.isPlayerAttacking)
        {
            if (m_hasAttacked && m_attackTimer < Time.time)
            {
                m_hasAttacked = false;
            }

            if (!m_hasAttacked)
            {
                Shoot();
                m_hasAttacked = true;
                m_attackTimer = Time.time + attackDelay + Random.Range(-attackDelayRandomRange, attackDelayRandomRange);
            }
        }
    }

    protected virtual void Shoot()
    {
        m_audioSource.Play();
    }
}
