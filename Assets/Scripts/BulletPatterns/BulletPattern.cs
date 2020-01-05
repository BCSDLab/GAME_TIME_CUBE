using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletPattern : MonoBehaviour
{
    public GameObject bullet;
    public AudioClip audioclip;

    [Tooltip("아무것도 없을 시 해당 오브젝트 위치")]
    public Transform bulletSpawn;

    [SerializeField]
    protected float startDelay;

    public void StartPattern()
    {
        if(bulletSpawn == null)
        {
            bulletSpawn = this.transform;
        }

        StartCoroutine("Fire");
    }

    public void StopPattern()
    {
        StopCoroutine("Fire");
    } 

    protected abstract IEnumerator Fire();

}
