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
    private Transform[] m_spawnPositions = null;
    [SerializeField]
    private Transform[] m_arrivePositions = null;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);

        while (true)
        {
            for (int i = 0; i < m_spawnPositions.Length; i++)
            {
                float atan = Mathf.Atan2(m_arrivePositions[i].position.y - m_spawnPositions[i].position.y, m_arrivePositions[i].position.x - m_spawnPositions[i].position.x);
                float cos = Mathf.Cos(atan);
                float sin = Mathf.Sin(atan);
                float angle = atan * Mathf.Rad2Deg;

                //m_audioSource.Play();

                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_spawnPositions[i].position;
                bulletInst.SetActive(true);

                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * cos, speed * sin);

                yield return new WaitForSeconds(inDelay);
            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}
