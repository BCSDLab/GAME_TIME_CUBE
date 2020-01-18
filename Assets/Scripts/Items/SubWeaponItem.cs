using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(ParticleSystem))]
public class SubWeaponItem : Item
{
    public GameObject subWeapon;

    protected override void PickUp()
    {
        GameManager.instance.subWeaponNum++;
        Instantiate(subWeapon, transform.position, Quaternion.identity).name = subWeapon.name + "_" + GameManager.instance.subWeaponNum;

        if (subWeapon.name == "Orbitor")
        {
            Debug.Log("Found!");
            InitOrbitorPosition();
        }
    }

    public void InitOrbitorPosition()
    {
        Orbitor[] orbitors = FindObjectsOfType(typeof(Orbitor)) as Orbitor[];
        float dist = 360 / orbitors.Length;

        for(int i = 0; i < orbitors.Length; i++)
        {
            orbitors[i].m_alpha = dist * i;
        }
    }
}
