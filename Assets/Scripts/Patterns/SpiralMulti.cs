using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralMulti : BulletPattern
{
    public int count = 3;
    public float speed = 3f;
    public float inDelay = 0.1f;
    [SerializeField]
    [Tooltip("초기 발사각")]
    private float m_angle = 0f;
    [SerializeField]
    [Tooltip("각속도")]
    private float m_omega = 10f;

    private const float circumference = 2f * Mathf.PI;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();

        m_angle = (m_angle % 360) * Mathf.Deg2Rad;
        m_omega = (m_omega % 360) * Mathf.Deg2Rad;
    }

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            m_audioSource.PlayOneShot(m_ShotSFX, m_ShotSFXVolum);

            for (int i = 0; i < count; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_spawnPos.position;
                bulletInst.SetActive(true);

                float a = circumference * i / count;
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(a + m_angle), speed * Mathf.Sin(a + m_angle));
                //obj.transform.Rotate(new Vector3(0f, 0f, 360f * i / SpiralShooting - 90f));

                //TODO: 오버플로 예방
                //Debug.Log("SprialMulti : m_omega = " + m_omega);
            }
            //m_angle += m_omega;
            //m_angle %= 2f * Mathf.PI;

            m_angle = (m_angle + m_omega) % circumference;
            //m_angle = m_angle % 2f * Mathf.PI + m_omega % 2f * Mathf.PI;
            Debug.Log("SprialMulti : m_angle = " + m_angle);

            yield return new WaitForSeconds(inDelay);
        }
    }
}
