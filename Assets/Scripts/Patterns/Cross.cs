using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : BulletPattern
{
    public int count;
    public float speed;
    public float delay = 1f;

    [SerializeField]
    private bool m_hasDestroyedSound = false;

    protected override IEnumerator Fire()
    {
        float theta = 360f / count;
        m_audioSource.PlayOneShot(m_ShotSFX, m_ShotSFXVolum);

        for (int i = 0; i < count; i++)
        {
            GameObject bulletInst = PoolManager.instance.PopFromPool(bullet.name);
            bulletInst.transform.position = m_spawnPos.position;
            bulletInst.transform.rotation = Quaternion.Euler(Vector3.forward * (theta * i));
            bulletInst.SetActive(true);

            if (m_hasDestroyedSound && i == 0)
            {
                bulletInst.GetComponent<AudioSource>().PlayOneShot(m_ShotSFX, m_ShotSFXVolum/8f);
            }

            bulletInst.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(Mathf.PI * 2f * i / count), speed * Mathf.Sin(Mathf.PI * 2f * i / count));
        }

        yield return new WaitForSeconds(delay);
    }
}
