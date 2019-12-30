using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 보조 장비
public abstract class PlayerSubWeapon : MonoBehaviour
{
    protected Transform m_target;

    public abstract void MultiplySpeed(float velocityMultiplier);
    public abstract void ResetSpeed();

    void OnEnable()
    {
        m_target = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
