using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class BulletGrazer : MonoBehaviour
{
    public int spellCharge = 100;

    private ParticleSystem m_particleSystem;
    private AudioSource m_audioSource;

    void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        m_audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            // 그레이즈 처리
            GameManager.instance.AddSpellEnergy(spellCharge);
            GameManager.instance.AddScore(spellCharge);

            // 파티클 재생
            m_particleSystem.Play();
            m_audioSource.Play();
        }
    }
}
