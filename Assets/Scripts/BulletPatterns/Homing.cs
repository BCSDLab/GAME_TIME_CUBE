using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : BulletPattern
{
    public int count = 1;
    public float delay = 0.5f;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            GetComponentInParent<AudioSource>().PlayOneShot(audioclip);

            for (int i = 0; i < count; i++)
            {
                GameObject obj = PoolManager.instance.PopFromPool(bullet.name);
                obj.transform.position = bulletSpawn.position;
                obj.SetActive(true);             
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
