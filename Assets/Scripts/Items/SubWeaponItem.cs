using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(ParticleSystem))]
public class SubWeaponItem : Item
{
    public GameObject subWeapon;

    protected override void PickUp()
    {
        Instantiate(subWeapon, transform.position, Quaternion.identity);
    }
}
