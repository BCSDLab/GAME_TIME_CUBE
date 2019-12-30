using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(AudioSource))]
public abstract class ZacoController : EnemyController
{
    private AudioSource[] m_audioSources;

    void Awake()
    {
        m_audioSources = GetComponents<AudioSource>();

        m_isInvincible = true;
    }

    void OnBecameVisible()
    {
        m_isInvincible = false;
    }

    public override void Damage(int damage)
    {
        if (m_isInvincible) return;

        m_hp -= damage;

        if (m_hp <= 0)
        {
            m_isInvincible = true;
            StartCoroutine("Die");
            return;
        }

        m_audioSources[0].Play();
    }

    public override void SpellDamage(int damage)
    {
        if (m_isInvincible) return;

        m_isInvincible = true;
        StartCoroutine("Die");
    }

    public override void Die()
    {
        //m_isInvincible = true;

        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            GameObject particleInst = Instantiate(particleSystem.gameObject, transform.position, Quaternion.identity, null);
            particleInst.GetComponent<ParticleSystem>().Play();
            Destroy(particleInst, particleSystem.main.duration + particleSystem.main.startLifetime.constant);
        }

        Destroy(gameObject);
    }
}
