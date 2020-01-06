using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalAimedRandom : AimedBulletPattern
{
    public int count = 10;
    public float minSpeed = 3f;
    public float maxSpeed = 6f;
    public float inDelay = 0.05f;
    public float outDelay = 0.5f;
    public float angleRange = 60f;

    private Vector3 m_targetPos;
    private float m_dx;
    private float m_dy;
    private float m_angleRange;
    private float m_speed;

    private void Start()
    {
        angleRange *= Mathf.Deg2Rad / 2f;
    }

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            Vector2 direction = target.position - m_bulletSpawn.position;
            float atan = Mathf.Atan2(direction.y, direction.x);

            GetComponentInParent<AudioSource>().PlayOneShot(audioclip);

            for (int i = 0; i < count; i++)
            {
                m_angleRange = Random.Range(-angleRange, angleRange);
                m_speed = Random.Range(minSpeed, maxSpeed);

                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_bulletSpawn.position;
                bulletInst.SetActive(true);

                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(m_speed * Mathf.Cos(atan + m_angleRange), m_speed * Mathf.Sin(atan + m_angleRange));

                yield return new WaitForSeconds(inDelay);
            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}

