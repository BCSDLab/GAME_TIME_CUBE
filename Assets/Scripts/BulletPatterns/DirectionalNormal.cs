using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalNormal : BulletPattern
{
    public int count = 5;
    public float speed = 5f;
    public float inDelay = 0.1f;
    public float outDelay = 1f;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                GetComponentInParent<AudioSource>().PlayOneShot(audioclip);
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = transform.position;
                bulletInst.SetActive(true);
                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0f);

                yield return new WaitForSeconds(inDelay);
            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}
