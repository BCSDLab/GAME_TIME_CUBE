using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalUp : BulletPattern
{
    public int count = 5;
    public float speed = 5f;
    public float inDelay = 0.1f;
    public float outDelay = 1f;

    [SerializeField]
    private bool m_isShootDown = false;

    public Transform[] spawnPosArr = null;

    protected override IEnumerator Fire()
    {
        float angle = -180f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        yield return new WaitForSeconds(m_startDelay);
        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                    m_audioSource.PlayOneShot(m_ShotSFX, m_ShotSFXVolum);
                    GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
                    bulletInst.transform.position = m_spawnPos.position;
                    bulletInst.transform.rotation = rotation;
                    bulletInst.SetActive(true);
                    if(!m_isShootDown)
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, speed);
                    else
                        bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -speed);

                    yield return new WaitForSeconds(inDelay);
            }
            yield return new WaitForSeconds(outDelay);
        }
    }
}
