using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalWall : BulletPattern
{
    public int count = 2;
    public float speed = 3f;
    public float inDelay = 0.15f;
    public float outDelay = 5f;
    [SerializeField]
    private bool m_isFromLeft = false;
    [SerializeField]
    private float m_x = 9f;
    [SerializeField]
    private float m_minY = -5f;
    [SerializeField]
    private float m_maxY = 5f;
    [SerializeField]
    private int m_bulletCount = 20;

    private float dist;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        dist = Mathf.Abs(m_maxY - m_minY) / m_bulletCount;
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
                    bulletInst.transform.position = new Vector3(m_x, m_minY + dist * j);
                    bulletInst.SetActive(true);

                    if (m_isFromLeft)
                    {
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0f);
                    }
                    else
                    {
                        bulletInst.transform.rotation = Quaternion.Euler(0, 0, 180f);
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0f);
                    }
                }
                
                yield return new WaitForSeconds(inDelay);
            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}
