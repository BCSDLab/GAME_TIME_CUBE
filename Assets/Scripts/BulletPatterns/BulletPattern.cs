using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletPattern : MonoBehaviour
{
    public GameObject bullet;
    public AudioClip audioclip;

    [Tooltip("미지정 시 해당 오브젝트 위치")]
    [SerializeField]
    protected Transform m_bulletSpawn = null;
    [SerializeField]
    protected float m_startDelay;

    public void StartPattern()
    {
        if (m_bulletSpawn == null)
        {
            m_bulletSpawn = transform;
        }

        StartCoroutine("Fire");
    }

    public void StopPattern()
    {
        StopCoroutine("Fire");
    }

    protected abstract IEnumerator Fire();

}
