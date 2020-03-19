using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiDirectional : BulletPattern
{
    public float speed = 5f;
    public float angle1 = 0f;
    public float angle2 = 180f;
    public float omega1 = 30f;
    public float omega2 = 60f;
    public int spiral = 1;
    public float delay = 0.3f;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        angle1 *= Mathf.Deg2Rad;
        angle2 *= Mathf.Deg2Rad;
        omega1 *= Mathf.Deg2Rad;
        omega2 *= Mathf.Deg2Rad;
    }

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            m_audioSource.PlayOneShot(m_ShotSFX, m_ShotSFXVolum);

            for (int i = 0; i < spiral; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_spawnPos.position;
                bulletInst.SetActive(true);
                float a = Mathf.PI * 2f * i / spiral + angle1;
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(a), speed * Mathf.Sin(a));
                bulletInst.transform.Rotate(Vector3.forward * a * Mathf.Rad2Deg);

                bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_spawnPos.position;
                bulletInst.SetActive(true);
                a = Mathf.PI * 2f * i / spiral + angle2;
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(a), speed * Mathf.Sin(a));
                bulletInst.transform.Rotate(Vector3.forward * a * Mathf.Rad2Deg);

                angle1 += omega1;
                angle2 += omega2;

                //TODO: 오버플로 예방
                Debug.Log("BiDirectional : angle1 = " + angle1);
                Debug.Log("BiDirectional : omega1 = " + omega1);
                Debug.Log("BiDirectional : angle2 = " + angle2);
                Debug.Log("BiDirectional : omega2 = " + omega2);
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
