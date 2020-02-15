using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalAimedNWay : AimedBulletPattern
{
    public int count = 5;
    public float speed = 3f;
    public float delay = 0.5f;
    public float angleRange = 60f;

    private float m_angle;
    private float m_omega;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        angleRange *= Mathf.Deg2Rad;
    }

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);

        while (true)
        {
            m_angle = (count == 1) ? 0f : -angleRange / 2;
            m_omega = (count == 1) ? 0f : angleRange / (count - 1);

            Vector2 direction = target.position - m_spawnPos.position;
            float atan = Mathf.Atan2(direction.y, direction.x);
            float angle = atan * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            m_audioSource.PlayOneShot(m_ShotSFX);

            for (int i = 0; i < count; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_spawnPos.position;
                bulletInst.transform.rotation = Quaternion.AngleAxis(angle + (m_angle * Mathf.Rad2Deg), Vector3.forward);
                bulletInst.SetActive(true);

                float cos = Mathf.Cos(atan + m_angle);
                float sin = Mathf.Sin(atan + m_angle);
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * cos, speed * sin);

                m_angle += m_omega;
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
