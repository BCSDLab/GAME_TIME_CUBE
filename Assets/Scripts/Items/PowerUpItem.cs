using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : Item
{
    public int power = 50;

    protected override void PickUp()
    {
        GameManager.instance.AddPower(power);
    }
}
