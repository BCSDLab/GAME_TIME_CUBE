using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreItem : Item
{
    public int score = 1000;

    protected override void PickUp()
    {
        GameManager.instance.AddScore(score);
    }
}
