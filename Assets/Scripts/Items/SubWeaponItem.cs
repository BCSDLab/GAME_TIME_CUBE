using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(ParticleSystem))]
public class SubWeaponItem : Item
{
    public GameObject[] subWeapon;

    public enum SubWeapon
    {
        Orbitor,
        Follower
    }

    protected override void PickUp() // 추가 작업 필요
    {
        SubWeapon sw = (SubWeapon)Random.Range(0, subWeapon.Length);
        int swIdx = (int)sw;

        GameManager.instance.subWeaponCount[swIdx]++;
        Instantiate(subWeapon[swIdx], transform.position, Quaternion.identity).name = subWeapon[swIdx].name + "_" + GameManager.instance.subWeaponCount[swIdx];

        switch (sw)
        {
            case SubWeapon.Orbitor:
                InitOrbitorPosition();
                break;
            case SubWeapon.Follower:
                InitFollowerPosition();
                break;
            default:
                break;
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
    public void InitFollowerPosition() // 추가 작업 필요
    {

    }
}
