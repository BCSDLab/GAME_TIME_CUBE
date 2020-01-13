using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertical : BulletPattern
{
    public int count = 5;
    public float speed = 5f;
    public float inDelay = 0.1f;
    public float outDelay = 1f;
    [SerializeField]
    private Vector2[] m_spawnPositions = null;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);

        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                m_audioSource.Play();

                foreach (Vector2 m_bulletSpawnPos in m_spawnPositions)
                {
                    GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                    bulletInst.transform.position = m_bulletSpawnPos;
                    bulletInst.SetActive(true);

                    if (m_bulletSpawnPos.y < 0)
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, speed);
                    else
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -speed);
                }

                yield return new WaitForSeconds(inDelay);
            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}
