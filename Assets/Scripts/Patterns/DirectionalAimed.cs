using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalAimed : AimedBulletPattern
{
    public int count = 5;
    public float speed = 5f;
    public float inDelay = 0.1f;
    public float outDelay = 1f;

    private Vector3 m_targetPos;

    protected override IEnumerator Fire()
    {
        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            m_targetPos = target.position;
            Vector2 direction = m_targetPos - m_spawnPos.position;
            float atan = Mathf.Atan2(direction.y, direction.x);
            float cos = Mathf.Cos(atan);
            float sin = Mathf.Sin(atan);
            float angle = atan * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            for (int i = 0; i < count; i++)
            {
                m_audioSource.PlayOneShot(m_ShotSFX);
                GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                bulletInst.transform.position = m_spawnPos.position;
                bulletInst.transform.rotation = rotation;
                bulletInst.SetActive(true);

                bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * cos, speed * sin);

                yield return new WaitForSeconds(inDelay);
            }

            yield return new WaitForSeconds(outDelay);
        }
    }
}
