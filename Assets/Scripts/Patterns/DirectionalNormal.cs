using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalNormal : BulletPattern
{
    public int count = 5;
    public float speed = 5f;
    public float inDelay = 0.1f;
    public float outDelay = 1f;

    public Transform[] spawnPosArr = null;

    protected override IEnumerator Fire()
    {
        if (spawnPosArr[0] == null)
        {
            spawnPosArr[0] = m_spawnPos;
        }
        float angle = -180f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < spawnPosArr.Length; j++)
                {
                    m_audioSource.PlayOneShot(m_ShotSFX);
                    GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                    bulletInst.transform.position = spawnPosArr[j].position;
                    bulletInst.transform.rotation = rotation;
                    bulletInst.SetActive(true);
                    bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0f);

                    yield return new WaitForSeconds(inDelay);
                }
            }
            yield return new WaitForSeconds(outDelay);
        }
    }
}
