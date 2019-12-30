using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CubeChargeItem : Item
{
    public int cubeCharge = 3000;

    protected override void PickUp()
    {
        GameManager.instance.AddCubeEnergy(cubeCharge);
    }
}
