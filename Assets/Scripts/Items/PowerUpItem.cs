using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : Item
{
    public int power = 50;
    public int score = 150;

    protected override void PickUp()
    {
        GameManager.instance.AddPower(power);

        if(GameManager.instance.playerPower >= 4000)
        {
            GameManager.instance.AddScore(score);
        }
    }
}
