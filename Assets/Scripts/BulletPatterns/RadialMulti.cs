using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMulti : BulletPattern
{
    public int count = 3;
    public float speed = 3;
    public float inDelay = 0.2f;
    [SerializeField]
    private bool m_isCountChange = false;

    private float m_theta;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_theta = 360f / count;
    }

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            m_audioSource.Play();

            for (int i = 0; i < count; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_bulletSpawn.position;
                bulletInst.transform.rotation = Quaternion.Euler(Vector3.forward * (m_theta * i - 90f));
                bulletInst.SetActive(true);

                float a = 2f * Mathf.PI * i / count;
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(a), speed * Mathf.Sin(a));
            }

            if (m_isCountChange)
                count += (count % 2 == 0) ? 1 : -1;

            yield return new WaitForSeconds(inDelay);
        }
    }
}
