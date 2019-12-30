using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpellChargeItem : Item
{
    public int spellCharge = 3000;

    protected override void PickUp()
    {
        GameManager.instance.AddSpellEnergy(spellCharge);
    }
}
