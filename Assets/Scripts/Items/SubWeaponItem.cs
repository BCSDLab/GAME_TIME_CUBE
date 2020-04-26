using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(ParticleSystem))]
public class SubWeaponItem : Item
{
    public enum SubWeapon
    {
        Orbitor,
        Follower
    }

    public GameObject[] subWeapon;

    protected override void PickUp()
    {
        int swIdx = Random.Range(0, subWeapon.Length);

        GameManager.instance.subWeaponCount[swIdx]++;
        Instantiate(subWeapon[swIdx], transform.position, Quaternion.identity).name = subWeapon[swIdx].name + "_" + GameManager.instance.subWeaponCount[swIdx];
        InitSubWeaponPosition(swIdx);
    }
    public void PickUpWithName(string swName)
    {
        int swIdx = (int)System.Enum.Parse(typeof(SubWeapon), swName);
        GameManager.instance.subWeaponCount[swIdx]++;
        Instantiate(subWeapon[swIdx], PlayerController.instance.transform.position, Quaternion.identity).name = subWeapon[swIdx].name + "_" + GameManager.instance.subWeaponCount[swIdx];
        InitSubWeaponPosition(swIdx);
    }

    #region Position
    public void InitSubWeaponPosition(int swIdx)
    {
        switch ((SubWeapon)swIdx)
        {
            case SubWeapon.Orbitor:
                InitOrbitorPosition();
                break;
            case SubWeapon.Follower:
                InitFollowerPosition();
                break;
            default:
                Debug.Log("Invalid SubWeapon");
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
    #endregion
}
