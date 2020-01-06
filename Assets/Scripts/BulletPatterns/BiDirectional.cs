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
    public float inDelay = 0.3f;

    void Start()
    {
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
            GetComponentInParent<AudioSource>().PlayOneShot(audioclip);

            for (int i = 0; i < spiral; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_bulletSpawn.position;
                bulletInst.SetActive(true);
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos((Mathf.PI * 2 * i / spiral) + angle1), speed * Mathf.Sin((Mathf.PI * i * 2 / spiral) + angle1));

                bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_bulletSpawn.position;
                bulletInst.SetActive(true);
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos((Mathf.PI * 2 * i / spiral) + angle2), speed * Mathf.Sin((Mathf.PI * i * 2 / spiral) + angle2));
                //obj.transform.Rotate(new Vector3(0f, 0f, 360 * i / SpiralShooting - 90));

                angle1 += omega1;
                angle2 += omega2;
                //TODO: 오버플로 예방
            }

            yield return new WaitForSeconds(inDelay);
        }
    }
}
