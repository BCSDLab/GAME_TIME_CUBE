using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosToPos : BulletPattern
{
    public int count = 5;
    public float speed = 5f;
    public float inDelay = 0.1f;
    public float outDelay = 1f;

    [SerializeField]
    private Transform[] m_bulletSpawnArr;
    [SerializeField]
    private Transform[] m_bulletArriveArr;

    protected override IEnumerator Fire()
    {

        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            for (int i = 0; i < m_bulletSpawnArr.Length; i++)
            {
                float atan = Mathf.Atan2(m_bulletArriveArr[i].position.y - m_bulletSpawnArr[i].position.y, m_bulletArriveArr[i].position.x - m_bulletSpawnArr[i].position.x);
                float cos = Mathf.Cos(atan);
                float sin = Mathf.Sin(atan);
                float angle = atan * Mathf.Rad2Deg;

                GetComponentInParent<AudioSource>().PlayOneShot(audioclip);
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_bulletSpawnArr[i].position;
                bulletInst.SetActive(true);

                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * cos, speed * sin);

                yield return new WaitForSeconds(inDelay);

            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}
