using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralMulti : BulletPattern
{
    public int count = 5;
    public float speed = 200f;
    public float delay = 0.1f;
    [Tooltip("초기 발사각")]
    public float angle = 0f;
    [Tooltip("각속도")]
    public float alpha = 1f;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            GetComponentInParent<AudioSource>().PlayOneShot(audioclip);

            for (int i = 0; i < count; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = bulletSpawn.position;
                bulletInst.SetActive(true);

                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos((Mathf.PI * 2f * i / count) + angle), speed * Mathf.Sin((Mathf.PI * 2f * i / count) + angle));
                //obj.transform.Rotate(new Vector3(0f, 0f, 360f * i / SpiralShooting - 90f));

                angle += alpha;
                if (angle > 3600f) angle -= 3600f;
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
