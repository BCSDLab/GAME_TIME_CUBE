using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerHomingBullet : PlayerBullet
{
    public float alpha = 300f;

    private Transform m_target;
    private Rigidbody2D m_rigidbody;
    private GameObject[] enemies;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        m_target = ChooseTarget();

        if (m_target == null)
        {
            if (m_rigidbody.velocity == Vector2.zero)
                m_rigidbody.velocity = transform.right * speed;

            m_rigidbody.angularVelocity = 0f;
            return;
        }

        if (GameManager.instance.isBossDefeated)
        {
            m_rigidbody.velocity = transform.right * speed;
            m_rigidbody.angularVelocity = 0f;
            return;
        }

        Vector2 direction = transform.position - m_target.transform.position;
        direction.Normalize();
        float crossZ = Vector3.Cross(direction, transform.right).z;

        m_rigidbody.velocity = transform.right * speed;
        m_rigidbody.angularVelocity = alpha * crossZ;
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
            }

            PoolManager.instance.PushToPool(gameObject);
            //Destroy(this.gameObject);  // 풀링 성능 테스트용
        }
    }

    Transform ChooseTarget()
    {
        Transform target = null;
        float minDistance = Mathf.Infinity;
        float distance = Mathf.Infinity;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = enemy.transform;
            }
        }
        return target;
    }
}
