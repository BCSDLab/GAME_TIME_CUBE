using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public bool m_spellDestroyed = false;
    protected void Blow()
    {
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            GameObject particleInst = Instantiate(particleSystem.gameObject, transform.position, Quaternion.identity, null);
            particleInst.GetComponent<ParticleSystem>().Play();
            Destroy(particleInst, particleSystem.main.duration + particleSystem.main.startLifetime.constant);
        }
    }
}
