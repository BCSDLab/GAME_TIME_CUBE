using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralMulti : BulletPattern
{
    public int count = 3;
    public float speed = 3f;
    public float inDelay = 0.1f;
    [Tooltip("초기 발사각")]
    public float angle = 0f;
    [Tooltip("각속도")]
    public float omega = 10f;

    void Start()
    {
        angle *= Mathf.Deg2Rad;
        omega *= Mathf.Deg2Rad;
    }

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            GetComponentInParent<AudioSource>().PlayOneShot(audioclip);

            for (int i = 0; i < count; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_bulletSpawn.position;
                bulletInst.SetActive(true);

                float a = 2f * Mathf.PI * i / count;
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(a + angle), speed * Mathf.Sin(a + angle));
                //obj.transform.Rotate(new Vector3(0f, 0f, 360f * i / SpiralShooting - 90f));

                angle += omega;
                //TODO: 오버플로 예방
            }

            yield return new WaitForSeconds(inDelay);
        }
    }
}
