using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMulti : BulletPattern
{
    public int count = 10;
    public float speed = 200f;
    public float delay = 0.5f;

    protected override IEnumerator Fire()
    {
        float theta = 360f / count;
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            GetComponentInParent<AudioSource>().PlayOneShot(audioclip);

            for (int i = 0; i < count; i++)
            {
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = transform.position;
                bulletInst.transform.rotation = Quaternion.Euler(Vector3.forward * (theta * i - 90f));
                bulletInst.SetActive(true);

                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(Mathf.PI * 2f * i / count), speed * Mathf.Sin(Mathf.PI * 2f * i / count));
            }

            count += (count % 2 == 0) ? 1 : -1;

            yield return new WaitForSeconds(delay);
        }
    }
}
