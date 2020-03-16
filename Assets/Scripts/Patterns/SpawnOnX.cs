using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 특정 X 좌표에 탄 생성
public class SpawnOnX: BulletPattern
{
    public int count = 10;
    public float speed = 5f;
    public float inDelay = 0.02f;
    public float outDelay = 1f;
    [SerializeField]
    private float m_spawnX = 7f;
    [SerializeField]
    private float m_minY = -4.7f;
    [SerializeField]
    private float m_maxY = 3.9f;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);

        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                m_audioSource.PlayOneShot(m_ShotSFX, m_ShotSFXVolum);

                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                Vector3 pos = Vector3.right * m_spawnX + Vector3.up * Random.Range(m_minY, m_maxY);
                bulletInst.transform.position = pos;
                bulletInst.SetActive(true);

                yield return new WaitForSeconds(inDelay);
            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}
