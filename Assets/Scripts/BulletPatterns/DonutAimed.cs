using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutAimed : AimedBulletPattern
{
    public int count;
    public float speed = 200f;
    public float delay = 1f;
    public float radius = 1f;

    private Vector3 m_targetPos;
    private float m_dx;
    private float m_dy;
    private float m_xForce;
    private float m_yForce;

    protected override IEnumerator Fire()
    {
        float twoPiPerCount = 2f * Mathf.PI / count;
        float theta = 360f / count;
        yield return new WaitForSeconds(m_startDelay);

        while (true)
        {
            yield return new WaitForSeconds(1f);
            m_targetPos = target.position;
            m_dx = m_bulletSpawn.position.x - m_targetPos.x;
            m_dy = m_bulletSpawn.position.y - m_targetPos.y;
            float atan = Mathf.Atan(m_dy / m_dx);
            float cos = Mathf.Cos(atan);
            float sin = Mathf.Sin(atan);

            m_audioSource.Play();

            for (int i = 0; i < count; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_bulletSpawn.position + new Vector3(radius * Mathf.Cos(twoPiPerCount * i), radius * Mathf.Sin(twoPiPerCount * i), 0f);
                bulletInst.transform.rotation = Quaternion.Euler(Vector3.forward * (theta * i - 90f));
                bulletInst.SetActive(true);

                if (m_dx != 0f)
                {
                    if (m_targetPos.x <= m_bulletSpawn.position.x)
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector3(-speed * cos, -speed * sin);
                    else
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector3(speed * cos, speed * sin);
                }
                else if (m_dx == 0f)
                {
                    if (m_dy < 0f)
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, -speed);
                    else if (m_dy > 0f)
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, speed);
                }
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
