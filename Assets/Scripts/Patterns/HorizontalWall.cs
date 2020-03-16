using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalWall : BulletPattern
{
    public int count = 2;
    public float speed = 3f;
    public float inDelay = 0.15f;
    public float outDelay = 5f;
    [SerializeField]
    private bool m_isRising = false;
    [SerializeField]
    private float m_y = 5f;
    [SerializeField]
    private float m_minX = -9f;
    [SerializeField]
    private float m_maxX = 9f;
    [SerializeField]
    private int m_bulletCount = 20;

    private float dist;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        dist = Mathf.Abs(m_maxX - m_minX) / m_bulletCount;
    }

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);

        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                m_audioSource.PlayOneShot(m_ShotSFX, m_ShotSFXVolum);

                for (int j = 0; j <= m_bulletCount; j++)
                {
                    GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                    bulletInst.transform.position = new Vector3(m_minX + dist * j, m_y);
                    bulletInst.SetActive(true);

                    if (m_isRising)
                    {
                        bulletInst.transform.rotation = Quaternion.Euler(0, 0, 90f);
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, speed);
                    }
                    else
                    {
                        bulletInst.transform.rotation = Quaternion.Euler(0, 0, -90f);
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -speed);
                    }
                }
                
                yield return new WaitForSeconds(inDelay);
            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}
