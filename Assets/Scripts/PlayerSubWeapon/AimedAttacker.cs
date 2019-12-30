using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어와 함께 공격하는 보조 장비 : 조준 공격
public class AimedAttacker : Attacker
{
    [SerializeField]
    private Vector3 instantiateOffset = Vector3.zero;

    private Transform m_target;
    private GameObject[] m_enemies;

    protected override void Shoot()
    {
        base.Shoot();

        if ((m_target = ChooseTarget()) == null)
            return;

        m_hasAttacked = true;
        m_attackTimer = Time.time + attackDelay + Random.Range(-attackDelayRandomRange, attackDelayRandomRange);

        float speed = bullet.GetComponent<PlayerBullet>().speed;
        Vector2 direction = m_target.position - transform.position;
        float atan = Mathf.Atan2(direction.y, direction.x);
        float angle = atan * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);

        bulletInst.transform.position = transform.position + instantiateOffset;
        bulletInst.transform.rotation = rotation;
        bulletInst.SetActive(true);

        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(atan), speed * Mathf.Sin(atan));
    }

    Transform ChooseTarget()
    {
        Transform target = null;
        float minDistance = Mathf.Infinity;
        float distance = Mathf.Infinity;
        m_enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in m_enemies)
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
