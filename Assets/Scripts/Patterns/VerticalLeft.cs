using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalLeft : BulletPattern
{
    public int count = 2;
    public float speed = 3f;
    public float inDelay = 0.15f;
    public float outDelay = 5f;
    [SerializeField]
    private float m_x = 9f;
    [SerializeField]
    private float m_top = 5f;
    [SerializeField]
    private float m_bottom = -5f;
    [SerializeField]
    private int m_bulletCount = 20;

    private float dist;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        dist = Mathf.Abs(m_top - m_bottom) / m_bulletCount;
    }

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);

        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                m_audioSource.PlayOneShot(m_ShotSFX);

                for (int j = 0; j <= m_bulletCount; j++)
                {
                    GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                    bulletInst.transform.position = new Vector3(m_x, m_bottom + dist * j);
                    bulletInst.SetActive(true);
                    bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0f);
                }
                
                yield return new WaitForSeconds(inDelay);
            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}
