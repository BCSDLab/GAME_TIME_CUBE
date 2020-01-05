using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bi_Directional : BulletPattern
{
    public float speed = 0;
    public float angle1 = 0;
    public float angle2 = 0;
    public float angleRate1 = 0;
    public float angleRate2 = 0;

    public int SpiralShooting;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            GetComponentInParent<AudioSource>().PlayOneShot(audioclip);

            for (int i = 0; i < SpiralShooting; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = bulletSpawn.position;
                bulletInst.SetActive(true); 
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos((Mathf.PI * 2 * i / SpiralShooting) + angle1), speed * Mathf.Sin((Mathf.PI * i * 2 / SpiralShooting) + angle1));

                bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = bulletSpawn.position;
                bulletInst.SetActive(true);
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos((Mathf.PI * 2 * i / SpiralShooting) - angle2), speed * Mathf.Sin((Mathf.PI * i * 2 / SpiralShooting) - angle2));
                //obj.transform.Rotate(new Vector3(0f, 0f, 360 * i / SpiralShooting - 90));

                angle1 += angleRate1;
                angle2 += angleRate2;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
