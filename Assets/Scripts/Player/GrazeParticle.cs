using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GrazeParticle : MonoBehaviour
{
    void Start()
    {
        //TODO: 오브젝트 풀링
        //Invoke("PushToPool", 0.5f);
        Destroy(gameObject, 0.5f);
    }

    void PushToPool()
    {
        PoolManager.instance.PushToPool(gameObject);
    }
}
