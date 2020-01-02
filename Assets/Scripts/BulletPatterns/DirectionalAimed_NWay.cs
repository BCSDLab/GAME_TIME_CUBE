using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalAimed_NWay : AimedBulletPattern
{
    [Header("※ 본 패턴은 Bullet의 속도를 덮어씁니다.")]
    public int count = 3;
    public float Speed = 4f;
    public float outDelay = 0.5f;
    public float angleRange = 0.1f;

    private Vector3 m_targetPos;
    private float m_angleRange;
    private float m_angleAlpha;
    private float m_speed;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            Vector2 direction = target.position - transform.position;
            float atan = Mathf.Atan(direction.y / direction.x);

            m_angleAlpha = 2 * angleRange / (count - 1);
            m_angleRange = angleRange;

            GetComponentInParent<AudioSource>().PlayOneShot(audioclip);

            for (int i = 0; i < count; i++)
            {
                m_speed = Speed;
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = transform.position;
                bulletInst.SetActive(true);

                if (direction.x < 0)  // 몹이 오른쪽
                    bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(-m_speed * Mathf.Cos(atan + m_angleRange), -m_speed * Mathf.Sin(atan + m_angleRange));
                else  // 플레이어가 오른쪽
                    bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(m_speed * Mathf.Cos(atan + m_angleRange), m_speed * Mathf.Sin(atan + m_angleRange));

                m_angleRange -= m_angleAlpha;
            }

            yield return new WaitForSeconds(outDelay);

        }
    }
}
