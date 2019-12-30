using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletPattern : MonoBehaviour
{
    public GameObject bullet;
    public AudioClip audioclip;

    [SerializeField]
    protected float startDelay;

    public void StartPattern()
    {
        StartCoroutine("Fire");
    }

    public void StopPattern()
    {
        StopCoroutine("Fire");
    }

    protected abstract IEnumerator Fire();

}
