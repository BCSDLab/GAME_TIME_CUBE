using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerBullet : MonoBehaviour
{
    public int baseDamage = 15;
    [HideInInspector]
    public int damage;
    public float speed = 20f;
    public int spellCharge = 10;

    protected const float TIME_TO_MAX_TRANSPARENCY = 0.1f;
    protected SpriteRenderer m_spriteRenderer;
    protected float m_life;
    protected bool m_isMaxTransparency;

    protected void Awake()
    {
        damage = baseDamage;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void OnEnable()
    {
        m_life = 0f;
        m_isMaxTransparency = false;
        Color newColor = m_spriteRenderer.color;
        newColor.a = 0f;
        m_spriteRenderer.color = newColor;
    }

    protected void Update()
    {
        if (!m_isMaxTransparency)
        {
            Color newColor = m_spriteRenderer.color;
            newColor.a = m_life / TIME_TO_MAX_TRANSPARENCY;
            m_spriteRenderer.color = newColor;

            m_life += Time.deltaTime;
            if (m_life >= TIME_TO_MAX_TRANSPARENCY)
            {
                m_isMaxTransparency = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemyController = collision.GetComponent<Enemy>();
            if (!enemyController.IsInvincible())
            {
                enemyController.Damage(damage);
                GameManager.instance.AddSpellEnergy(spellCharge);
                GameManager.instance.AddScore(damage);
                Blow();
            }

            PoolManager.instance.PushToPool(gameObject);
        }
    }

    protected void Blow()
    {
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            GameObject particleInst = Instantiate(particleSystem.gameObject, transform.position, Quaternion.identity, null);
            particleInst.GetComponent<ParticleSystem>().Play();
            Destroy(particleInst, particleSystem.main.duration + particleSystem.main.startLifetime.constant);
        }
    }
}
