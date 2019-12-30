using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUpItem : Item
{
    [Header("이미 라이프가 최대치일 때 얻는 점수")]
    public int score = 5000;

    protected override void PickUp()
    {
        GameManager.instance.LifeUp(scoreWhenMaxLife: score);
    }
}
